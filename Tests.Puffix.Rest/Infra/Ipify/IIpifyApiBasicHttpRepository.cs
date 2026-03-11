using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ipify;

public interface IIpifyApiBasicHttpRepository : IBasicRestHttpRepository<IIpifyApiBasicQueryInformation, IIpifyApiToken> { }