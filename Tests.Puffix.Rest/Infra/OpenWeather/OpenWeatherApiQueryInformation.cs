using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.OpenWeather;

public class OpenWeatherApiQueryInformation(HttpMethod httpMethod, IOpenWeatherApiToken? token, IDictionary<string, string> headers, string uri, string queryParameters, string queryContent) :
    QueryInformation<IOpenWeatherApiToken>(httpMethod, token, headers, uri, queryParameters, queryContent),
    IOpenWeatherApiQueryInformation
{
    public static IOpenWeatherApiQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        string uri = BuildUriWithPath(apiUri, queryPath);
        return new OpenWeatherApiQueryInformation(httpMethod, default, new Dictionary<string, string>(), uri, queryParameters, queryContent);
    }

    public static IOpenWeatherApiQueryInformation CreateNewAuthenticatedQuery(IOpenWeatherApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        string uri = BuildUriWithPath(apiUri, queryPath);
        return new OpenWeatherApiQueryInformation(httpMethod, token, new Dictionary<string, string>(), uri, queryParameters, queryContent);
    }
}