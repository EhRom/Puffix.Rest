using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ovh;

public class OvhApiHttpRepository(IHttpClientFactory httpClientFactory) :
    RestHttpRepository<IOvhApiQueryInformation, IOvhApiToken>(httpClientFactory),
    IOvhApiHttpRepository
{
    public override IOvhApiQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        return OvhApiQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOvhApiQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return OvhApiQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOvhApiQueryInformation BuildAuthenticatedQuery(IOvhApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        return OvhApiQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOvhApiQueryInformation BuildAuthenticatedQuery(IOvhApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return OvhApiQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}