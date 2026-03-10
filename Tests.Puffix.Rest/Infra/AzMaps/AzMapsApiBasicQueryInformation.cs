using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.AzMaps;

public class AzMapsApiBasicQueryInformation(HttpMethod httpMethod, IAzMapsApiToken? token, IDictionary<string, IEnumerable<string>> headers, string baseUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent) :
    BasicQueryInformation<IAzMapsApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IAzMapsApiBasicQueryInformation
{
    public static IAzMapsApiBasicQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new AzMapsApiBasicQueryInformation(httpMethod, default, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public static IAzMapsApiBasicQueryInformation CreateNewAuthenticatedQuery(IAzMapsApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new AzMapsApiBasicQueryInformation(httpMethod, token, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}
