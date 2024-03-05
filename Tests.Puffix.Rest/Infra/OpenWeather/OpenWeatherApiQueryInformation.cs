using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.OpenWeather;

public class OpenWeatherApiQueryInformation(HttpMethod httpMethod, IOpenWeatherApiToken? token, IDictionary<string, string> headers, string baseUri, string queryPath, string queryParameters, string queryContent) :
    QueryInformation<IOpenWeatherApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IOpenWeatherApiQueryInformation
{
    public static IOpenWeatherApiQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return new OpenWeatherApiQueryInformation(httpMethod, default, new Dictionary<string, string>(), apiUri, queryPath, queryParameters, queryContent);
    }

    public static IOpenWeatherApiQueryInformation CreateNewAuthenticatedQuery(IOpenWeatherApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return new OpenWeatherApiQueryInformation(httpMethod, token, new Dictionary<string, string>(), apiUri, queryPath, queryParameters, queryContent);
    }
}