using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ipify;

public class IpifyApiBasicQueryInformation(HttpMethod httpMethod, IIpifyApiToken? token, IDictionary<string, IEnumerable<string>> headers, string baseUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent) :
    BasicQueryInformation<IIpifyApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IIpifyApiBasicQueryInformation
{
    public static IIpifyApiBasicQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new IpifyApiBasicQueryInformation(httpMethod, default, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public static IIpifyApiBasicQueryInformation CreateNewAuthenticatedQuery(IIpifyApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new IpifyApiBasicQueryInformation(httpMethod, token, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}