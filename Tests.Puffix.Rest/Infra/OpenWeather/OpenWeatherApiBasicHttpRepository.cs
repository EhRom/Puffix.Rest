using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.OpenWeather;

public class OpenWeatherApiBasicHttpRepository(IHttpClientFactory httpClientFactory) :
    BasicRestHttpRepository<IOpenWeatherApiBasicQueryInformation, IOpenWeatherApiToken>(httpClientFactory),
    IOpenWeatherApiBasicHttpRepository
{
    public override IOpenWeatherApiBasicQueryInformation BuildAuthenticatedQuery(IOpenWeatherApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        return OpenWeatherApiBasicQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOpenWeatherApiBasicQueryInformation BuildAuthenticatedQuery(IOpenWeatherApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return OpenWeatherApiBasicQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOpenWeatherApiBasicQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        return OpenWeatherApiBasicQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOpenWeatherApiBasicQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return OpenWeatherApiBasicQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}