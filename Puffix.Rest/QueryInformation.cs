using System.Text;

namespace Puffix.Rest;

public abstract class QueryInformation<TokenT>(HttpMethod httpMethod, TokenT? token, IDictionary<string, IEnumerable<string>> headers, string baseUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent) :
    IQueryInformation<TokenT>
        where TokenT : IToken
{
    protected readonly string baseUri = baseUri;
    protected readonly string queryPath = queryPath;
    protected readonly IDictionary<string, string> queryParameters = queryParameters;
    protected readonly string queryContent = queryContent;

    public HttpMethod QuerytHttpMethod { get; } = httpMethod;

    public TokenT? Token { get; } = token;

    public bool IsHeaderToken => Token is not null && Token is IHeaderToken;

    public IDictionary<string, IEnumerable<string>> Headers { get; } = headers;

    public virtual Uri GetUriWithParameters()
    {
        string processedQueryParameter = BuildQueryParameters();
        string uriWithPath = BuildUriWithPath();

        string completeUri = string.IsNullOrEmpty(processedQueryParameter) ? uriWithPath : $"{uriWithPath}?{processedQueryParameter.Trim('?')}";

        return new Uri(completeUri);
    }

    protected virtual string BuildUriWithPath()
    {
        return BuildUriWithPath(false);
    }

    private string BuildUriWithPath(bool skipToken)
    {
        string builtUri = baseUri.TrimEnd('/');

        string builtQueryPath = (!skipToken && Token is not null && Token is IQueryPathToken) ?
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
        return BuildQueryParameters(false);
    }

    private string BuildQueryParameters(bool skipToken)
    {
        StringBuilder queryParameterBuilder = new StringBuilder();
        if (!skipToken && Token is not null && Token is IQueryParameterToken)
            queryParameterBuilder.Append($"{(Token as IQueryParameterToken)!.GetQueryParameterName()}={(Token as IQueryParameterToken)!.GetQueryParameterValue()}");


        foreach (string parameterKey in queryParameters.Keys)
        {
            if (queryParameterBuilder.Length > 0)
                queryParameterBuilder.Append("&");

            queryParameterBuilder.Append($"{parameterKey}={queryParameters[parameterKey]}");
        }

        return queryParameterBuilder.ToString();
    }

    public virtual HttpContent GetQueryContent()
    {
        return new StringContent(queryContent ?? string.Empty, Encoding.UTF8, "application/json");
    }

    public virtual IDictionary<string, IEnumerable<string>> GetAuthenticationHeader()
    {
        return IsHeaderToken ? (Token as IHeaderToken)!.GetHeaders() : new Dictionary<string, IEnumerable<string>>();
    }

    public override string ToString()
    {
        string processedQueryParameter = BuildQueryParameters(true);
        string uriWithPath = BuildUriWithPath(true);

        string completeUri = string.IsNullOrEmpty(processedQueryParameter) ? uriWithPath : $"{uriWithPath}?{processedQueryParameter.Trim('?')}";

        return $"{nameof(QueryInformation<TokenT>)}--{httpMethod}--{completeUri}";
    }
}
