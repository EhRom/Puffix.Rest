//using System.Text;

using Puffix.Rest.Base;
using System.Text;

namespace Puffix.Rest;

public abstract class BasicQueryInformation<TokenT>(HttpMethod httpMethod, TokenT? token, IDictionary<string, IEnumerable<string>> headers, string baseUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent) :
    BaseQueryInformation<TokenT, string>(httpMethod, token, headers, baseUri, queryPath, queryParameters, queryContent),
    IQueryInformation<TokenT>
        where TokenT : IToken
{
    public override HttpContent GetQueryContent()
    {
        return new StringContent(queryContent ?? string.Empty, Encoding.UTF8, "application/json");
    }
}