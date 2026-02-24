using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.AzMaps;

public class AzMapsApiQueryInformation(HttpMethod httpMethod, IAzMapsApiToken? token, IDictionary<string, IEnumerable<string>> headers, string baseUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent) :
    BasicQueryInformation<IAzMapsApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IAzMapsApiQueryInformation
{
    public static IAzMapsApiQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new AzMapsApiQueryInformation(httpMethod, default, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public static IAzMapsApiQueryInformation CreateNewAuthenticatedQuery(IAzMapsApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new AzMapsApiQueryInformation(httpMethod, token, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}
