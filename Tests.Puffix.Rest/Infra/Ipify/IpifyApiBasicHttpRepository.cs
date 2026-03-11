using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ipify;

public class IpifyApiBasicHttpRepository(IHttpClientFactory httpClientFactory) :
    BasicRestHttpRepository<IIpifyApiBasicQueryInformation, IIpifyApiToken>(httpClientFactory),
    IIpifyApiBasicHttpRepository
{
    public override IIpifyApiBasicQueryInformation BuildAuthenticatedQuery(IIpifyApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        return IpifyApiBasicQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IIpifyApiBasicQueryInformation BuildAuthenticatedQuery(IIpifyApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return IpifyApiBasicQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IIpifyApiBasicQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        return IpifyApiBasicQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IIpifyApiBasicQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return IpifyApiBasicQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}