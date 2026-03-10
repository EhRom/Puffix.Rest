namespace Puffix.Rest;

public abstract class BasicRestHttpRepository<QueryInformationContainerT, TokenT>(IHttpClientFactory httpClientFactory) :
    BaseRestHttpRepository<QueryInformationContainerT, TokenT, string>(httpClientFactory),
    IBasicRestHttpRepository<QueryInformationContainerT, TokenT>
    where QueryInformationContainerT : IQueryInformation<TokenT>
    where TokenT : IToken
{ }