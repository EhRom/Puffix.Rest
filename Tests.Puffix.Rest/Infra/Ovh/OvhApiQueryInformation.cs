using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ovh;

public class OvhApiQueryInformation(HttpMethod httpMethod, IOvhApiToken? token, IDictionary<string, string> headers, string uri, string queryParameters, string queryContent) :
    QueryInformation<IOvhApiToken>(httpMethod, token, headers, uri, queryParameters, queryContent),
    IOvhApiQueryInformation
{
    public static IOvhApiQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        string uri = BuildUriWithPath(apiUri, queryPath);
        return new OvhApiQueryInformation(httpMethod, default, new Dictionary<string, string>(), uri, queryParameters, queryContent);
    }

    public static IOvhApiQueryInformation CreateNewAuthenticatedQuery(IOvhApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        string uri = BuildUriWithPath(apiUri, queryPath);
        return new OvhApiQueryInformation(httpMethod, token, new Dictionary<string, string>(), uri, queryParameters, queryContent);
    }

    public override IDictionary<string, string> GetAuthenticationHeader()
    {
        IDictionary<string, string> authHeaders = base.GetAuthenticationHeader();

        if (Token is not null)
        {
            (string signature, long currentTimestamp) = Token!.GenerateSignature(QuerytHttpMethod, uri, queryContent);
            authHeaders[IOvhApiToken.OVH_TIME_HEADER] = currentTimestamp.ToString();
            authHeaders[IOvhApiToken.OVH_SIGNATURE_HEADER] = signature;
        }

        return authHeaders;
    }
}