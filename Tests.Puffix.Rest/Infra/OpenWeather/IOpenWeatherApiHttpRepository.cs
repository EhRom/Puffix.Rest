using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.OpenWeather;

public interface IOpenWeatherApiHttpRepository : IBasicRestHttpRepository<IOpenWeatherApiQueryInformation, IOpenWeatherApiToken> { }