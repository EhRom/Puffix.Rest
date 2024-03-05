using System.Text;

namespace Puffix.Rest;

public abstract class QueryInformation<TokenT>(HttpMethod httpMethod, TokenT? token, IDictionary<string, string> headers, string baseUri, string queryPath, string queryParameters, string queryContent) :
    IQueryInformation<TokenT>
        where TokenT : IToken
{
    protected readonly string baseUri = baseUri;
    protected readonly string queryPath = queryPath;
    protected readonly string queryParameters = queryParameters;
    protected readonly string queryContent = queryContent;

    public HttpMethod QuerytHttpMethod { get; } = httpMethod;

    public TokenT? Token { get; } = token;

    public bool IsHeaderToken => Token is not null && Token is IHeaderToken;

    public IDictionary<string, string> Headers { get; } = headers;

    //protected static string BuildUriWithPath(string apiUri, string queryPath)
    //{
    //    string builtUri = apiUri.TrimEnd('/');
    //    builtUri = string.IsNullOrEmpty(queryPath) ? builtUri : $"{builtUri}/{queryPath.TrimStart('/')}";

    //    return builtUri;
    //}

    public virtual Uri GetUriWithParameters()
    {
        string processedQueryParameter = BuildQueryParameters();
        string uriWithPath = BuildUriWithPath();

        string completeUri = string.IsNullOrEmpty(processedQueryParameter) ? uriWithPath : $"{uriWithPath}?{processedQueryParameter}";

        return new Uri(completeUri);
    }

    protected virtual string BuildUriWithPath()
    {
        string builtUri = baseUri.TrimEnd('/');

        string builtQueryPath = (Token is not null && Token is IQueryPathToken) ?
                                            (Token as IQueryPathToken)!.GetQueryPath() :
                                            string.Empty;

        builtQueryPath = string.IsNullOrEmpty(queryPath) ?
                    builtQueryPath :
                        (string.IsNullOrEmpty(builtQueryPath) ?
                            queryPath.Trim('/') :
                            $"{builtQueryPath.Trim('/')}/{queryPath.Trim('/')}");

        builtUri = string.IsNullOrEmpty(builtQueryPath) ? builtUri : $"{builtUri}/{builtQueryPath}";

        return builtUri;
    }

    protected virtual string BuildQueryParameters()
    {
        string processedQueryParameter = (Token is not null && Token is IQueryParameterToken) ?
                                            (Token as IQueryParameterToken)!.GetQueryParameter() :
                                            string.Empty;

        processedQueryParameter = string.IsNullOrEmpty(queryParameters) ?
                    processedQueryParameter :
                    (string.IsNullOrEmpty(processedQueryParameter) ?
                        queryParameters.TrimStart('?') :
                        $"{processedQueryParameter}&{queryParameters.TrimStart('?')}");

        return processedQueryParameter;
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
