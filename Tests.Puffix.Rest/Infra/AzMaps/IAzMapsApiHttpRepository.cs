using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.AzMaps;

public interface IAzMapsApiHttpRepository : IRestHttpRepository<IAzMapsApiQueryInformation, IAzMapsApiToken> { }
