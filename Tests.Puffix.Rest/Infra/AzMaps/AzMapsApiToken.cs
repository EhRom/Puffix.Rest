using Microsoft.Extensions.Configuration;

namespace Tests.Puffix.Rest.Infra.AzMaps;

public class AzMapsApiToken(IConfiguration configuration) : IAzMapsApiToken
{
    private readonly string token = configuration["azmapsApiToken"] ?? string.Empty;

    public string GetQueryParameter()
    {
        return $"subscription-key={token}";
    }
}
