using Puffix.Rest.Base;

namespace Puffix.Rest;

public class QueryInformation<TokenT>(HttpMethod httpMethod, TokenT? token, IDictionary<string, IEnumerable<string>> headers, string baseUri, string queryPath, IDictionary<string, string> queryParameters, IQueryContent queryContent) :
    BaseQueryInformation<TokenT, IQueryContent>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IQueryInformation<TokenT>
        where TokenT : IToken
{
    public override HttpContent GetQueryContent()
    {
        return queryContent.GetQueryContent();
    }
}
