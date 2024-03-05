using Microsoft.Extensions.Configuration;
using Moq.Protected;
using Moq;
using Puffix.IoC;
using Puffix.IoC.Configuration;
using System.Net;
using System.Text;
using Tests.Puffix.Rest.Infra;
using Tests.Puffix.Rest.Infra.AzMaps;
using Tests.Puffix.Rest.Infra.Ovh;

namespace Tests.Puffix.Rest;

public class OvhApiTests
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
    public async Task LoadLocationsTest()
    {
        const string baseUri = "https://eu.api.ovh.com/1.0";
        const string authenticationUriPath = "auth/time";
        const string accountUriPath = "me";
        const string logsUriPath = "me/api/logs/self";

        try
        {
            IOvhApiToken token = container.Resolve<IOvhApiToken>();
            IOvhApiHttpRepository httpRepository = container.Resolve<IOvhApiHttpRepository>();

            IOvhApiQueryInformation authQueryInformation = httpRepository.BuildUnauthenticatedQuery(HttpMethod.Get, baseUri, authenticationUriPath, string.Empty, string.Empty);

            string referenceUnixTime = await httpRepository.HttpAsync(authQueryInformation);
            token.SetReferenceUnixTime(referenceUnixTime);
            IOvhApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, baseUri, accountUriPath, string.Empty, string.Empty);

            string result = await httpRepository.HttpAsync(queryInformation);

            IOvhApiQueryInformation queryInformationBis = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, baseUri, logsUriPath, string.Empty, string.Empty);

            long[] resultBis = await httpRepository.HttpJsonAsync<long[]>(queryInformationBis);

            string logUriPath = resultBis.Any() ? $"{logsUriPath}/{resultBis.First()}" : logsUriPath;
            IOvhApiQueryInformation queryInformationTer = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, baseUri, logUriPath, string.Empty, string.Empty);

            string resultTer = await httpRepository.HttpAsync(queryInformationTer);

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.Not.Empty);

                Assert.That(resultBis, Is.Not.Null);
                Assert.That(resultBis.Count(), Is.GreaterThan(0));

                Assert.That(resultTer, Is.Not.Null);
                Assert.That(resultTer, Is.Not.Empty);
            });
        }
        catch (Exception error)
        {
            Assert.Fail(error.Message);
        }
    }

    [Test]
    public async Task UnitTest()
    {
        const string baseUri = "https://eu.api.ovh.com/1.0";
        const string logsUriPath = "me/api/logs/self";
        try
        {
            BuildMocks(container, out IOvhApiToken token, out Mock<IHttpClientFactory> httpClientFactoryMock, out IOvhApiHttpRepository httpRepository);

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
            IOvhApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, baseUri, logsUriPath, string.Empty, string.Empty);
            //string actualResult = await httpRepository.HttpAsync(queryInformation);
            long[] actualResult = await httpRepository.HttpJsonAsync<long[]>(queryInformation);

            // Check calls
            httpClientFactoryMock.Verify(httpClientFactory => httpClientFactory.CreateClient(It.IsAny<string>()), Times.Once());
            mockHttpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri!.AbsoluteUri.Equals($"{baseUri}/{logsUriPath}")),
                ItExpr.IsAny<CancellationToken>()
            );

            // Check result
            Assert.Multiple(() =>
            {
                Assert.That(actualResult, Is.Not.Null);
                Assert.That(actualResult, Is.Not.Empty);
                Assert.That(actualResult, Is.EqualTo(expextedResponse));
            });
        }
        catch (Exception error)
        {
            Assert.Fail($"Error while testing {nameof(UnitTest)}: {error.Message}");
        }
    }

    private static void BuildMocks(IIoCContainer container,
        out IOvhApiToken token,
        out Mock<IHttpClientFactory> httpClientFactoryMock,
        out IOvhApiHttpRepository httpRepository
    )
    {
        // Create authentication token.
        token = container.Resolve<IOvhApiToken>();

        // Build HTTP repository and HTTP client factory mock
        httpClientFactoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);

        httpRepository = container.Resolve<IOvhApiHttpRepository>
        (
            IoCNamedParameter.CreateNew("httpClientFactory", httpClientFactoryMock.Object)
        );
    }

    private const string sampleResponse = @"[1,2,3,4,5]";
    private static readonly long[] expextedResponse = [1, 2, 3, 4, 5];
}
