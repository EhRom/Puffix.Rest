using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.OpenWeather;

public class OpenWeatherApiHttpRepository(IHttpClientFactory httpClientFactory) :
    RestHttpRepository<IOpenWeatherApiQueryInformation, IOpenWeatherApiToken>(httpClientFactory),
    IOpenWeatherApiHttpRepository
{
    public override IOpenWeatherApiQueryInformation BuildAuthenticatedQuery(IOpenWeatherApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return OpenWeatherApiQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOpenWeatherApiQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return OpenWeatherApiQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, apiUri, queryPath, queryParameters, queryContent);
    }
}