using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.OpenWeather;

public class OpenWeatherApiHttpRepository(IHttpClientFactory httpClientFactory) :
    BasicRestHttpRepository<IOpenWeatherApiQueryInformation, IOpenWeatherApiToken>(httpClientFactory),
    IOpenWeatherApiHttpRepository
{
    public override IOpenWeatherApiQueryInformation BuildAuthenticatedQuery(IOpenWeatherApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        return OpenWeatherApiQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOpenWeatherApiQueryInformation BuildAuthenticatedQuery(IOpenWeatherApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return OpenWeatherApiQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOpenWeatherApiQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        return OpenWeatherApiQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOpenWeatherApiQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return OpenWeatherApiQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}