using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using NUnit.Framework.Internal;
using Puffix.IoC;
using Puffix.IoC.Configuration;
using System.Globalization;
using System.Net;
using System.Text;
using Tests.Puffix.Rest.Infra;
using Tests.Puffix.Rest.Infra.OpenWeather;

namespace Tests.Puffix.Rest;

public class OpenWeatherApiTests
{
    private const string LocationName = "Villeurbanne";
    private const double Latitude = 45.7733573;
    private const double Longitude = 4.8868454;

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
        try
        {
            IOpenWeatherApiToken token = container.Resolve<IOpenWeatherApiToken>();
            IOpenWeatherApiHttpRepository httpRepository = container.Resolve<IOpenWeatherApiHttpRepository>();

            string owWeatherApiBaseUri = (container.ConfigurationRoot[nameof(owWeatherApiBaseUri)] ?? string.Empty).TrimEnd('/');
            string queryParameters = $"lat={Latitude.ToString(CultureInfo.InvariantCulture.NumberFormat)}&lon={Longitude.ToString(CultureInfo.InvariantCulture.NumberFormat)}&units=metric";
            
            IOpenWeatherApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, owWeatherApiBaseUri, string.Empty, queryParameters, string.Empty);
            string coreMeteo = await httpRepository.HttpAsync(queryInformation);

            Assert.That(coreMeteo, Is.Not.Null);
            Assert.That(coreMeteo, Is.Not.Empty);
            Assert.That(coreMeteo.Contains($@"""name"":""{LocationName}"",""cod"":200"), Is.True);
        }
        catch (Exception error)
        {
            Assert.Fail(error.Message);
        }
    }

    [Test]
    public async Task UnitTest()
    {
        try
        {
            BuildMocks(container, out IOpenWeatherApiToken token, out Mock<IHttpClientFactory> httpClientFactoryMock, out IOpenWeatherApiHttpRepository httpRepository);

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
            string owWeatherApiBaseUri = (container.ConfigurationRoot[nameof(owWeatherApiBaseUri)] ?? string.Empty).TrimEnd('/');
            string queryParameters = $"lat={Latitude.ToString(CultureInfo.InvariantCulture.NumberFormat)}&lon={Longitude.ToString(CultureInfo.InvariantCulture.NumberFormat)}&units=metric";

            IOpenWeatherApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, owWeatherApiBaseUri, string.Empty, queryParameters, string.Empty);
            string actualResult = await httpRepository.HttpAsync(queryInformation);

            // Check calls
            httpClientFactoryMock.Verify(httpClientFactory => httpClientFactory.CreateClient(It.IsAny<string>()), Times.Once());
            mockHttpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri!.AbsoluteUri.StartsWith(owWeatherApiBaseUri) && req.RequestUri!.AbsoluteUri.EndsWith(queryParameters)),
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
            Assert.Fail($"Error while testing {nameof(Test)}: {error.Message}");
        }
    }

    private static void BuildMocks(IIoCContainer container,
        out IOpenWeatherApiToken token,
        out Mock<IHttpClientFactory> httpClientFactoryMock,
        out IOpenWeatherApiHttpRepository httpRepository
    )
    {
        // Create authentication token.
        token = container.Resolve<IOpenWeatherApiToken>();

        // Build HTTP repository and HTTP client factory mock
        httpClientFactoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);

        httpRepository = container.Resolve<IOpenWeatherApiHttpRepository>
        (
            IoCNamedParameter.CreateNew("httpClientFactory", httpClientFactoryMock.Object)
        );
    }

    private const string sampleResponse = """
                {
            "coord": {
                "lon": 4.8868,
                "lat": 45.7734
            },
            "weather": [
                {
                    "id": 803,
                    "main": "Clouds",
                    "description": "broken clouds",
                    "icon": "04d"
                }
            ],
            "base": "stations",
            "main": {
                "temp": 8.95,
                "feels_like": 7.92,
                "temp_min": 7.81,
                "temp_max": 10.51,
                "pressure": 1017,
                "humidity": 64
            },
            "visibility": 10000,
            "wind": {
                "speed": 2.06,
                "deg": 300
            },
            "clouds": {
                "all": 75
            },
            "dt": 1709650765,
            "sys": {
                "type": 1,
                "id": 6505,
                "country": "FR",
                "sunrise": 1709619093,
                "sunset": 1709659951
            },
            "timezone": 3600,
            "id": 2968254,
            "name": "Villeurbanne",
            "cod": 200
        }
        """;
}
