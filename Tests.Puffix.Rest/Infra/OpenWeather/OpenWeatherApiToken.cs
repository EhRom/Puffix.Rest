using Microsoft.Extensions.Configuration;

namespace Tests.Puffix.Rest.Infra.OpenWeather;

public class OpenWeatherApiToken(IConfiguration configuration) : IOpenWeatherApiToken
{
    private readonly string token = configuration["owapiToken"] ?? string.Empty;

    public string GetQueryParameterName()
    {
        return "appid";
    }

    public string GetQueryParameterValue()
    {
        return token;
    }
}