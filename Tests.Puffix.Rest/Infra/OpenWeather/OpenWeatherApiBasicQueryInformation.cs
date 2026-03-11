using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.OpenWeather;

public class OpenWeatherApiBasicQueryInformation(HttpMethod httpMethod, IOpenWeatherApiToken? token, IDictionary<string, IEnumerable<string>> headers, string baseUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent) :
    BasicQueryInformation<IOpenWeatherApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IOpenWeatherApiBasicQueryInformation
{
    public static IOpenWeatherApiBasicQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new OpenWeatherApiBasicQueryInformation(httpMethod, default, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public static IOpenWeatherApiBasicQueryInformation CreateNewAuthenticatedQuery(IOpenWeatherApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new OpenWeatherApiBasicQueryInformation(httpMethod, token, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}