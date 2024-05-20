using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ovh;

public class OvhApiHttpRepository(IHttpClientFactory httpClientFactory) :
    RestHttpRepository<IOvhApiQueryInformation, IOvhApiToken>(httpClientFactory),
    IOvhApiHttpRepository
{
    public override IOvhApiQueryInformation BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return OvhApiQueryInformation.CreateNewUnauthenticatedQuery(httpMethod, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IOvhApiQueryInformation BuildAuthenticatedQuery(IOvhApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return OvhApiQueryInformation.CreateNewAuthenticatedQuery(token, httpMethod, apiUri, queryPath, queryParameters, queryContent);
    }
}