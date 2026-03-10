namespace Puffix.Rest;

public interface IRestHttpRepository<QueryInformationContainerT, TokenT> : IBaseRestHttpRepository<QueryInformationContainerT, TokenT, IQueryContent>
    where QueryInformationContainerT : IQueryInformation<TokenT>
    where TokenT : IToken
{ }