using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.OpenWeather;

public interface IOpenWeatherApiBasicHttpRepository : IBasicRestHttpRepository<IOpenWeatherApiBasicQueryInformation, IOpenWeatherApiToken> { }