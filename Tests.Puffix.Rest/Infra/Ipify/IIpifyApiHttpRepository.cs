using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ipify;

public interface IIpifyApiHttpRepository : IRestHttpRepository<IIpifyApiQueryInformation, IIpifyApiToken> { }