using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.OpenWeather;

public class OpenWeatherApiQueryInformation(HttpMethod httpMethod, IOpenWeatherApiToken? token, IDictionary<string, IEnumerable<string>> headers, string baseUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent) :
    BasicQueryInformation<IOpenWeatherApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IOpenWeatherApiQueryInformation
{
    public static IOpenWeatherApiQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new OpenWeatherApiQueryInformation(httpMethod, default, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public static IOpenWeatherApiQueryInformation CreateNewAuthenticatedQuery(IOpenWeatherApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new OpenWeatherApiQueryInformation(httpMethod, token, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}