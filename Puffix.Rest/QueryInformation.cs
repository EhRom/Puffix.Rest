using System.Text;

namespace Puffix.Rest;

public abstract class QueryInformation<TokenT>(HttpMethod httpMethod, TokenT? token, IDictionary<string, string> headers, string uri, string queryParameters, string queryContent) :
    IQueryInformation<TokenT>
        where TokenT : IToken
{
    protected readonly string uri = uri;
    protected readonly string queryParameters = queryParameters;
    protected readonly string queryContent = queryContent;

    public HttpMethod QuerytHttpMethod { get; } = httpMethod;

    public TokenT? Token { get; } = token;

    public bool IsHeaderToken => Token is not null && Token is IHeaderToken;

    public IDictionary<string, string> Headers { get; } = headers;

    protected static string BuildUriWithPath(string apiUri, string queryPath)
    {
        string builtUri = apiUri.TrimEnd('/');
        builtUri = string.IsNullOrEmpty(queryPath) ? builtUri : $"{builtUri}/{queryPath.TrimStart('/')}";

        return builtUri;
    }

    public virtual Uri GetUriWithParameters()
    {
        string processedQueryParameter = (Token is not null && Token is IQueryParameterToken) ?
                (Token as IQueryParameterToken)!.GetQueryParameter() :
                string.Empty;

        processedQueryParameter = string.IsNullOrEmpty(queryParameters) ?
                    processedQueryParameter :
                    (string.IsNullOrEmpty(processedQueryParameter) ?
                        queryParameters.TrimStart('?') :
                        $"{processedQueryParameter}&{queryParameters.TrimStart('?')}");

        string builtUri = string.IsNullOrEmpty(processedQueryParameter) ? uri : $"{uri}?{processedQueryParameter}";

        return new Uri(builtUri);
    }

    public virtual HttpContent GetQueryContent()
    {
        return new StringContent(queryContent ?? string.Empty, Encoding.UTF8, "application/json");
    }

    public virtual IDictionary<string, string> GetAuthenticationHeader()
    {
        return IsHeaderToken ? (Token as IHeaderToken)!.GetHeaders() : new Dictionary<string, string>();
    }
}
