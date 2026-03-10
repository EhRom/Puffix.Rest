using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ovh;

public class OvhApiBasicQueryInformation(HttpMethod httpMethod, IOvhApiToken? token, IDictionary<string, IEnumerable<string>> headers, string baseUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent) :
    BasicQueryInformation<IOvhApiToken>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IOvhApiBasicQueryInformation
{
    public static IOvhApiBasicQueryInformation CreateNewUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new OvhApiBasicQueryInformation(httpMethod, default, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public static IOvhApiBasicQueryInformation CreateNewAuthenticatedQuery(IOvhApiToken token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent)
    {
        return new OvhApiBasicQueryInformation(httpMethod, token, headers, apiUri, queryPath, queryParameters, queryContent);
    }

    public override IDictionary<string, IEnumerable<string>> GetAuthenticationHeader()
    {
        IDictionary<string, IEnumerable<string>> authHeaders = base.GetAuthenticationHeader();

        if (Token is not null)
        {
            string targetUri = BuildUriWithPath();
            (string signature, long currentTimestamp) = Token!.GenerateSignature(QuerytHttpMethod, targetUri, queryContent);
            authHeaders[IOvhApiToken.OVH_TIME_HEADER] = [currentTimestamp.ToString()];
            authHeaders[IOvhApiToken.OVH_SIGNATURE_HEADER] = [signature];
        }

        return authHeaders;
    }
}