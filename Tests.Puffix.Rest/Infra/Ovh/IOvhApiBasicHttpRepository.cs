using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ovh;

public interface IOvhApiBasicHttpRepository : IBasicRestHttpRepository<IOvhApiBasicQueryInformation, IOvhApiToken> { }