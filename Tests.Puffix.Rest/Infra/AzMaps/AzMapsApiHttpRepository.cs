using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.AzMaps;

public class AzMapsApiHttpRepository(IHttpClientFactory httpClientFactory) :
    BasicRestHttpRepository<IAzMapsApiQueryInformation, IAzMapsApiToken>(httpClientFactory),
    IAzMapsApiHttpRepository
{
    public override IAzMapsApiQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        return AzMapsApiQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IAzMapsApiQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return AzMapsApiQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IAzMapsApiQueryInformation BuildAuthenticatedQuery(IAzMapsApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        IDictionary<string, IEnumerable<string>> headers = new Dictionary<string, IEnumerable<string>>();
        return AzMapsApiQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IAzMapsApiQueryInformation BuildAuthenticatedQuery(IAzMapsApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return AzMapsApiQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, headers, apiUri, queryPath, queryParameters, queryContent);
    }
}
