using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ovh;

public interface IOvhApiHttpRepository : IBasicRestHttpRepository<IOvhApiQueryInformation, IOvhApiToken> { }