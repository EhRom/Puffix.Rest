using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using NUnit.Framework.Internal;
using Puffix.IoC;
using Puffix.IoC.Configuration;
using Puffix.Rest;
using System.Net;
using System.Text;
using Tests.Puffix.Rest.Infra;
using Tests.Puffix.Rest.Infra.Ipify;

namespace Tests.Puffix.Rest;

public class IpifyApiTests
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

    [Test] // > Sample method, only for local use
    public async Task LoadLocationsTest()
    {
        try
        {
            IIpifyApiToken token = container.Resolve<IIpifyApiToken>();
            IIpifyApiHttpRepository httpRepository = container.Resolve<IIpifyApiHttpRepository>();

            string ipifyUri = (container.Configuration[nameof(ipifyUri)] ?? string.Empty).TrimEnd('/');
            IDictionary<string, string> queryParameters = new Dictionary<string, string>();

            IIpifyApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, ipifyUri, string.Empty, queryParameters, string.Empty);
            string actualResult = await httpRepository.HttpAsync(queryInformation);

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult, Is.Not.Empty);
            Assert.That(actualResult, Does.Match(@"\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}"));
        }
        catch (Exception error)
        {
            Assert.Fail(error.Message);
        }
    }

    [Test] // > Sample method, only for local use
    public async Task LoadLocationsWithStatusTest()
    {
        try
        {
            IIpifyApiToken token = container.Resolve<IIpifyApiToken>();
            IIpifyApiHttpRepository httpRepository = container.Resolve<IIpifyApiHttpRepository>();

            string ipifyUri = (container.Configuration[nameof(ipifyUri)] ?? string.Empty).TrimEnd('/');
            IDictionary<string, string> queryParameters = new Dictionary<string, string>();

            IIpifyApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, ipifyUri, string.Empty, queryParameters, string.Empty);
            IResultInformation<string> actualResult = await httpRepository.HttpWithStatusAsync(queryInformation);

            Assert.That(actualResult, Is.Not.Null);
            Assert.That(actualResult.IsSuccess, Is.True);
            Assert.That(actualResult.ResultContent, Is.Not.Null);
            Assert.That(actualResult.ResultContent, Is.Not.Empty);
            Assert.That(actualResult.ResultContent, Does.Match(@"\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}"));
            Assert.That(actualResult.ErrorContent, Is.Empty.Or.Null);
            Assert.That(actualResult.ResultCode, Is.EqualTo(HttpStatusCode.OK));
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
            BuildMocks(container, out IIpifyApiToken token, out Mock<IHttpClientFactory> httpClientFactoryMock, out IIpifyApiHttpRepository httpRepository);

            // Register HTTP Calls
            using HttpContent expectedHttpContent = new StringContent(sampleResponse ?? string.Empty, Encoding.UTF8, "text/^plain");

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
            string ipifyUri = (container.Configuration[nameof(ipifyUri)] ?? string.Empty).TrimEnd('/');
            IDictionary<string, string> queryParameters = new Dictionary<string, string>();

            // TODO test JSON format >> ?format=json
            //string expectedQueryParameters = BuildExpectedQueryParameters(queryParameters);

            IIpifyApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, ipifyUri, string.Empty, queryParameters, string.Empty);

            // TODO test HttpWithStatusAsync
            string actualResult = await httpRepository.HttpAsync(queryInformation);

            // Check calls
            httpClientFactoryMock.Verify(httpClientFactory => httpClientFactory.CreateClient(It.IsAny<string>()), Times.Once());
            mockHttpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Between(0, 1, Moq.Range.Inclusive),
                ItExpr.Is<HttpRequestMessage>(req => req.Method == HttpMethod.Get && req.RequestUri!.AbsoluteUri.StartsWith(ipifyUri)),
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

    private static string BuildExpectedQueryParameters(IDictionary<string, string> queryParameters)
    {
        return string.Join('&', queryParameters.Select(kv => $"{kv.Key}={kv.Value}").ToList());
    }

    private static void BuildMocks(IIoCContainer container,
        out IIpifyApiToken token,
        out Mock<IHttpClientFactory> httpClientFactoryMock,
        out IIpifyApiHttpRepository httpRepository
    )
    {
        // Create authentication token.
        token = container.Resolve<IIpifyApiToken>();

        // Build HTTP repository and HTTP client factory mock
        httpClientFactoryMock = new Mock<IHttpClientFactory>(MockBehavior.Strict);

        httpRepository = container.Resolve<IIpifyApiHttpRepository>
        (
            IoCNamedParameter.CreateNew("httpClientFactory", httpClientFactoryMock.Object)
        );
    }

    private const string sampleResponse = "192.168.1.10";
}
