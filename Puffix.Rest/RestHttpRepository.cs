using Puffix.Rest.Base;

namespace Puffix.Rest;

public abstract class RestHttpRepository<QueryInformationContainerT, TokenT>(IHttpClientFactory httpClientFactory) :
    BaseRestHttpRepository<QueryInformationContainerT, TokenT, IQueryContent>(httpClientFactory),
    IRestHttpRepository<QueryInformationContainerT, TokenT>
    where QueryInformationContainerT : IQueryInformation<TokenT>
    where TokenT : IToken
{ }