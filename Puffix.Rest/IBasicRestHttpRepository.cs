namespace Puffix.Rest;

public interface IBasicRestHttpRepository<BasicQueryInformationContainerT, TokenT>
    where BasicQueryInformationContainerT : IBasicQueryInformation<TokenT>
    where TokenT : IToken
{
    BasicQueryInformationContainerT BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent);

    BasicQueryInformationContainerT BuildUnauthenticatedQuery(HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent);

    BasicQueryInformationContainerT BuildAuthenticatedQuery(TokenT token, HttpMethod httpMethod, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent);

    BasicQueryInformationContainerT BuildAuthenticatedQuery(TokenT token, HttpMethod httpMethod, IDictionary<string, IEnumerable<string>> headers, string apiUri, string queryPath, IDictionary<string, string> queryParameters, string queryContent);

    Task<string> HttpAsync(BasicQueryInformationContainerT queryInformation);

    Task<Stream> HttpStreamAsync(BasicQueryInformationContainerT queryInformation);

    Task<ResultT> HttpJsonAsync<ResultT>(BasicQueryInformationContainerT queryInformation);

    Task<IResultInformation<string>> HttpWithStatusAsync(BasicQueryInformationContainerT queryInformation);

    Task<IResultInformation<Stream>> HttpStreamWithStatusAsync(BasicQueryInformationContainerT queryInformation);

    Task<IResultInformation<ResultT>> HttpJsonWithStatusAsync<ResultT>(BasicQueryInformationContainerT queryInformation);
}
