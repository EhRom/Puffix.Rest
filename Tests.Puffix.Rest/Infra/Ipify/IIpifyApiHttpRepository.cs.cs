using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ipify;

public interface IIpifyApiHttpRepository : IBasicRestHttpRepository<IIpifyApiQueryInformation, IIpifyApiToken> { }