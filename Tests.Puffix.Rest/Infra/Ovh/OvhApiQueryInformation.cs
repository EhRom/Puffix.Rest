using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ovh;

public class OvhApiQueryInformation(HttpMethod httpMethod, IOvhApiToken? token, IDictionary<string, string> headers, string baseUri, string queryPath, string queryParameters, string queryContent) :
    QueryInformation<IOvhApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IOvhApiQueryInformation
{
    public static IOvhApiQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return new OvhApiQueryInformation(httpMethod, default, new Dictionary<string, string>(), apiUri, queryPath, queryParameters, queryContent);
    }

    public static IOvhApiQueryInformation CreateNewAuthenticatedQuery(IOvhApiToken token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent)
    {
        return new OvhApiQueryInformation(httpMethod, token, new Dictionary<string, string>(), apiUri, queryPath, queryParameters, queryContent);
    }

    public override IDictionary<string, string> GetAuthenticationHeader()
    {
        IDictionary<string, string> authHeaders = base.GetAuthenticationHeader();

        if (Token is not null)
        {
            string targetUri = BuildUriWithPath();
            (string signature, long currentTimestamp) = Token!.GenerateSignature(QuerytHttpMethod, targetUri, queryContent);
            authHeaders[IOvhApiToken.OVH_TIME_HEADER] = currentTimestamp.ToString();
            authHeaders[IOvhApiToken.OVH_SIGNATURE_HEADER] = signature;
        }

        return authHeaders;
    }
}