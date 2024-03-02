using Microsoft.Extensions.Configuration;
using Puffix.IoC.Configuration;
using Tests.Puffix.Rest.Infra;
using Tests.Puffix.Rest.Infra.AzMaps;
using Tests.Puffix.Rest.Infra.OpenWeather;

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

    //[Test] > Sample method
    public async Task Test()
    {
        const string azMapsBaseUri = "https://atlas.microsoft.com";
        const string azMapsSearchAddressQeuryPath = $"search/address/json";
        try
        {
            IAzMapsApiToken token = container.Resolve<IAzMapsApiToken>();
            IAzMapsApiHttpRepository httpRepository = container.Resolve<IAzMapsApiHttpRepository>();

            string queryParameters = "api-version=1.0&language=en-US&query=Villeurbanne";
            // subscription-key={Your-Azure-Maps-Subscription-key}&api-version=1.0&language=en-US&query=400 Broad St, Seattle, WA 98109

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
}
