using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ovh;

public class OvhApiBasicHttpRepository(IHttpClientFactory httpClientFactory) :
    BasicRestHttpRepository<IOvhApiBasicQueryInformation, IOvhApiToken>(httpClientFactory),
    IOvhApiBasicHttpRepository
{
    public override IOvhApiBasicQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        return OvhApiBasicQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOvhApiBasicQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return OvhApiBasicQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOvhApiBasicQueryInformation BuildAuthenticatedQuery(IOvhApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        return OvhApiBasicQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOvhApiBasicQueryInformation BuildAuthenticatedQuery(IOvhApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return OvhApiBasicQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}