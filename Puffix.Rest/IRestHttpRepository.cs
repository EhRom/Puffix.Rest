namespace Puffix.Rest;

public interface IRestHttpRepository<QueryInformationContainerT, TokenT>
    where QueryInformationContainerT : IQueryInformation<TokenT>
    where TokenT : IToken
{
    QueryInformationContainerT BuildUnauthenticatedQuery(HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent);

    QueryInformationContainerT BuildAuthenticatedQuery(TokenT token, HttpMethod httpMethod, string apiUri, string queryPath, string queryParameters, string queryContent);

    Task<string> HttpAsync(QueryInformationContainerT queryInformation);

    Task<Stream> HttpStreamAsync(QueryInformationContainerT queryInformation);

    Task<ResultT> HttpJsonAsync<ResultT>(QueryInformationContainerT queryInformation);
}
