using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.AzMaps;

public class AzMapsApiQueryInformation(HttpMethod httpMethod, IAzMapsApiToken? token, IDictionary<string, string> headers, string baseUri, string queryPath, string queryParameters, string queryContent) :
    QueryInformation<IAzMapsApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IAzMapsApiQueryInformation
{
    public static IAzMapsApiQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return new AzMapsApiQueryInformation(httpMethod, default, new Dictionary<string, string>(), apiUri, queryPath, queryParameters, queryContent);
    }

    public static IAzMapsApiQueryInformation CreateNewAuthenticatedQuery(IAzMapsApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return new AzMapsApiQueryInformation(httpMethod, token, new Dictionary<string, string>(), apiUri, queryPath, queryParameters, queryContent);
    }
}
