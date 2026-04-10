using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ovh;

public class OvhApiQueryInformation(HttpMethod httpMethod, IOvhApiToken? token, IDictionary<string, IEnumerable<string>> headers, string baseUri, string queryPath, IDictionary<string, string> queryParameters, IQueryContent queryContent) :
    QueryInformation<IOvhApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IOvhApiQueryInformation
{
    public static IOvhApiQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, IQueryContent queryContent)
    {
        return new OvhApiQueryInformation(httpMethod, default, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public static IOvhApiQueryInformation CreateNewAuthenticatedQuery(IOvhApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, IQueryContent queryContent)
    {
        return new OvhApiQueryInformation(httpMethod, token, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}