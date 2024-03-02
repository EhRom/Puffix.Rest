using Microsoft.Extensions.Configuration;
using Puffix.IoC.Configuration;
using System.Globalization;
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

    [Test]
    public async Task LoadLocationsTest()
    {
        try
        {
            IOpenWeatherApiToken token = container.Resolve<IOpenWeatherApiToken>();
            IOpenWeatherApiHttpRepository httpRepository = container.Resolve<IOpenWeatherApiHttpRepository>();
            string owWeatherApiBaseUri = (container.ConfigurationRoot[nameof(owWeatherApiBaseUri)] ?? string.Empty).TrimEnd('/');
            IOpenWeatherApiQueryInformation queryInformation = BuildGetMeteoQueryInformation(httpRepository, token, owWeatherApiBaseUri);

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

    private IOpenWeatherApiQueryInformation BuildGetMeteoQueryInformation(IOpenWeatherApiHttpRepository httpRepository, IOpenWeatherApiToken token, string apiBaseUri)
    {
        string queryParameters = $"lat={Latitude.ToString(CultureInfo.InvariantCulture.NumberFormat)}&lon={Longitude.ToString(CultureInfo.InvariantCulture.NumberFormat)}&units=metric";
        IOpenWeatherApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, apiBaseUri, string.Empty, queryParameters, string.Empty);

        return queryInformation;
    }
}
