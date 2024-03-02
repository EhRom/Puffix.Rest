using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.AzMaps;

public class AzMapsApiQueryInformation(HttpMethod httpMethod, IAzMapsApiToken? token, IDictionary<string, string> headers, string uri, string queryParameters, string queryContent) :
    QueryInformation<IAzMapsApiToken>(httpMethod, token, headers, uri, queryParameters, queryContent),
    IAzMapsApiQueryInformation
{
    public static IAzMapsApiQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        string uri = BuildUriWithPath(apiUri, queryPath);
        return new AzMapsApiQueryInformation(httpMethod, default, new Dictionary<string, string>(), uri, queryParameters, queryContent);
    }

    public static IAzMapsApiQueryInformation CreateNewAuthenticatedQuery(IAzMapsApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        string uri = BuildUriWithPath(apiUri, queryPath);
        return new AzMapsApiQueryInformation(httpMethod, token, new Dictionary<string, string>(), uri, queryParameters, queryContent);
    }
}
