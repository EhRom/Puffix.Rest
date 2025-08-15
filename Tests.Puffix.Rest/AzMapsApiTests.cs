using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Puffix.IoC;
using Puffix.IoC.Configuration;
using System.Globalization;
using System.Net;
using System.Text;
using Tests.Puffix.Rest.Infra;
using Tests.Puffix.Rest.Infra.AzMaps;

namespace Tests.Puffix.Rest;

public class AzMapsApiTests
{

    private IIoCContainerWithConfiguration container;

    [SetUp]
    public void Setup()
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appSettings.local.json", optional: true, reloadOnChange: true)
            .Build();

        container = IoCContainer.CreateNew(configuration);
    }

    //[Test] // > Sample method, only for local use
    public async Task Test()
    {
        const string azMapsBaseUri = "https://atlas.microsoft.com";
        const string azMapsSearchAddressQeuryPath = $"search/address/json";
        try
        {
            IAzMapsApiToken token = container.Resolve<IAzMapsApiToken>();
            IAzMapsApiHttpRepository httpRepository = container.Resolve<IAzMapsApiHttpRepository>();

            IDictionary<string, string> queryParameters = new Dictionary<string, string>()
            {
                { "api-version", "1.0" },
                { "language", "language=en-US" },
                { "query", "Villeurbanne" }
            };

            IAzMapsApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, azMapsBaseUri, azMapsSearchAddressQeuryPath, queryParameters, string.Empty);

            string actualResult = await httpRepository.HttpAsync(queryInformation);

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult, Is.Not.Empty);
        }
        catch (Exception error)
        {
            Assert.Fail($"Error while testing {nameof(Test)}: {error.Message}");
        }
    }

    [Test]
    public async Task UnitTest()
    {
        const string azMapsBaseUri = "https://atlas.microsoft.com";
        const string azMapsSearchAddressQueryPath = $"search/address/json";

        try
        {
            IDictionary<string, string> queryParameters = new Dictionary<string, string>()
            {
                { "api-version", "1.0" },
                { "language", "language=en-US" },
                { "query", "Villeurbanne" }
            };
            string expectedQueryParameters = BuildExpectedQueryParameters(queryParameters);

            BuildMocks(container, out IAzMapsApiToken token, out Mock<IHttpClientFactory> httpClientFactoryMock, out IAzMapsApiHttpRepository httpRepository);

            // Register HTTP Calls
            using HttpContent expectedHttpContent = new StringContent(sampleResponse ?? string.Empty, Encoding.UTF8, "application/json");

            Mock<HttpMessageHandler> mockHttpMessageHandlerMock = new Mock<HttpMessageHandler>();
            mockHttpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = expectedHttpContent
                });
             
            using HttpClient httpClient = new HttpClient(mockHttpMessageHandlerMock.Object);

            httpClientFactoryMock.Setup(httpClientFactory => httpClientFactory.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            // Test
            IAzMapsApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, azMapsBaseUri, azMapsSearchAddressQueryPath, queryParameters, string.Empty);
            string actualResult = await httpRepository.HttpAsync(queryInformation);

            // Check calls
            httpClientFactoryMock.Verify(httpClientFactory => httpClientFactory.CreateClient(It.IsAny<string>()), Times.Once());
            mockHttpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Between(0, 1, Moq.Range.Inclusive),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri!.AbsoluteUri.StartsWith($"{azMapsBaseUri}/{azMapsSearchAddressQueryPath}") && req.RequestUri!.AbsoluteUri.EndsWith(expectedQueryParameters)),
                ItExpr.IsAny<CancellationToken>()
            );

            // Check result
            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.Not.Null);
                Assert.That(actualResult, Is.Not.Empty);
                Assert.That(actualResult, Is.EqualTo(sampleResponse));
            });
        }
        catch (Exception error)
        {
            Assert.Fail($"Error while testing {nameof(UnitTest)}: {error.Message}");
        }
    }

    private static string BuildExpectedQueryParameters(IDictionary<string, string> queryParameters)
    {
        return string.Join('&', queryParameters.Select(kv => $"{kv.Key}={kv.Value}").ToList());
    }

    private static void BuildMocks(IIoCContainer container,
        out IAzMapsApiToken token,
        out Mock<IHttpClientFactory> httpClientFactoryMock,
        out IAzMapsApiHttpRepository httpRepository
    )
    {
        // Create authentication token.
        token = container.Resolve<IAzMapsApiToken>();

        // Build HTTP repository and HTTP client factory mock
        httpClientFactoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);

        httpRepository = container.Resolve<IAzMapsApiHttpRepository>
        (
            IoCNamedParameter.CreateNew("httpClientFactory", httpClientFactoryMock.Object)
        );
    }

    private const string sampleResponse = """
                {
            "summary": {
                "query": "villeurbanne",
                "queryType": "NON_NEAR",
                "queryTime": 32,
                "numResults": 4,
                "offset": 0,
                "totalResults": 4,
                "fuzzyLevel": 1
            },
            "results": [
                {
                    "type": "Geography",
                    "id": "ID",
                    "score": 1,
                    "entityType": "Municipality",
                    "matchConfidence": {
                        "score": 1
                    },
                    "address": {
                        "municipality": "Villeurbanne",
                        "countrySecondarySubdivision": "Rhône",
                        "countrySubdivision": "Auvergne-Rhône-Alpes",
                        "countrySubdivisionName": "Auvergne-Rhône-Alpes",
                        "countrySubdivisionCode": "ARA",
                        "countryCode": "FR",
                        "country": "France",
                        "countryCodeISO3": "FRA",
                        "freeformAddress": "Villeurbanne"
                    },
                    "position": {
                        "lat": 45.76467,
                        "lon": 4.8804
                    }
                }
            ]
        }
        """;
}
