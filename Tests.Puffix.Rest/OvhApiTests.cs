using Microsoft.Extensions.Configuration;
using Puffix.IoC.Configuration;
using Tests.Puffix.Rest.Infra;
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

    //[Test] > Sample method
    public async Task LoadLocationsTest()
    {
        const string baseUri = "https://eu.api.ovh.com/1.0";
        const string authenticationUri = "auth/time";
        const string accountUri = "me";
        const string logsUri = "me/api/logs/self";

        try
        {
            IOvhApiToken token = container.Resolve<IOvhApiToken>();
            IOvhApiHttpRepository httpRepository = container.Resolve<IOvhApiHttpRepository>();

            IOvhApiQueryInformation authQueryInformation = BuildAuthQueryInformation(httpRepository, baseUri, authenticationUri);

            string referenceUnixTime = await httpRepository.HttpAsync(authQueryInformation);            
            token.SetReferenceUnixTime(referenceUnixTime);

            IOvhApiQueryInformation queryInformation = BuildQueryInformation(httpRepository, token, baseUri, accountUri);
            string result = await httpRepository.HttpAsync(queryInformation);

            IOvhApiQueryInformation queryInformationBis = BuildQueryInformation(httpRepository, token, baseUri, logsUri);
            long[] resultBis = await httpRepository.HttpJsonAsync<long[]>(queryInformationBis);

            string logUri = resultBis.Any() ? $"{logsUri}/{resultBis.First()}" : logsUri;
            IOvhApiQueryInformation queryInformationTer = BuildQueryInformation(httpRepository, token, baseUri, logUri);
            string resultTer = await httpRepository.HttpAsync(queryInformationTer);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
        }
        catch (Exception error)
        {
            Assert.Fail(error.Message);
        }
    }

    private IOvhApiQueryInformation BuildAuthQueryInformation(IOvhApiHttpRepository httpRepository, string apiBaseUri, string queryPath)
    {
        IOvhApiQueryInformation queryInformation = httpRepository.BuildUnauthenticatedQuery(HttpMethod.Get, apiBaseUri, queryPath, string.Empty, string.Empty);
        return queryInformation;
    }

    private IOvhApiQueryInformation BuildQueryInformation(IOvhApiHttpRepository httpRepository, IOvhApiToken token, string apiBaseUri, string queryPath)
    {
        IOvhApiQueryInformation queryInformation = httpRepository.BuildAuthenticatedQuery(token, HttpMethod.Get, apiBaseUri, queryPath, string.Empty, string.Empty);
        return queryInformation;
    }
}
