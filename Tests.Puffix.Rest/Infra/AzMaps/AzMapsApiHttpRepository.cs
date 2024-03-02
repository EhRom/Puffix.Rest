using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.AzMaps;

public class AzMapsApiHttpRepository(IHttpClientFactory httpClientFactory) :
    RestHttpRepository<IAzMapsApiQueryInformation, IAzMapsApiToken>(httpClientFactory),
    IAzMapsApiHttpRepository
{
    public override IAzMapsApiQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return AzMapsApiQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IAzMapsApiQueryInformation BuildAuthenticatedQuery(IAzMapsApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return AzMapsApiQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, apiUri, queryPath, queryParameters, queryContent);
    }
}
