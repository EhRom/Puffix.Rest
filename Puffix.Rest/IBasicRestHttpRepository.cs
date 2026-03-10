using Puffix.Rest.Base;

namespace Puffix.Rest;

public interface IBasicRestHttpRepository<BasicQueryInformationContainerT, TokenT> : IBaseRestHttpRepository<BasicQueryInformationContainerT, TokenT, string>
    where BasicQueryInformationContainerT : IQueryInformation<TokenT>
    where TokenT : IToken
{ }
