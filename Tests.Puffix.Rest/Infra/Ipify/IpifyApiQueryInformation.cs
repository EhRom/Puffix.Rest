using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ipify;

public class IpifyApiQueryInformation(HttpMethod httpMethod, IIpifyApiToken? token, IDictionary<string, IEnumerable<string>> headers, string baseUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent) :
    QueryInformation<IIpifyApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IIpifyApiQueryInformation
{
    public static IIpifyApiQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new IpifyApiQueryInformation(httpMethod, default, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public static IIpifyApiQueryInformation CreateNewAuthenticatedQuery(IIpifyApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new IpifyApiQueryInformation(httpMethod, token, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}