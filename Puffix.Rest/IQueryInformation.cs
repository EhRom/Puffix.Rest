namespace Puffix.Rest;

public interface IQueryInformation<TokenT>
    where TokenT : IToken
{
    HttpMethod QuerytHttpMethod { get; }

    TokenT? Token { get; }

    bool IsHeaderToken { get; }

    IDictionary<string, IEnumerable<string>> Headers { get; }

    Uri GetUriWithParameters();

    HttpContent GetQueryContent();

    IDictionary<string, IEnumerable<string>> GetAuthenticationHeader();
}