using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Tests.Puffix.Rest.Infra.Ovh;

public class OvhApiToken(IConfiguration configuration) : IOvhApiToken
{
    private readonly string ovhApplicationKey = configuration[nameof(ovhApplicationKey)] ?? string.Empty;
    private readonly string ovhApplicationSecret = configuration[nameof(ovhApplicationSecret)] ?? string.Empty;
    private readonly string ovhConsumerKey = configuration[nameof(ovhConsumerKey)] ?? string.Empty;

    private long currentDeltaTimeWithOvh = 0;

    public void SetReferenceUnixTime(string referenceUnixTime)
    {
        long currentUnixTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        currentDeltaTimeWithOvh = long.Parse(referenceUnixTime) - currentUnixTimestamp;
    }

    public (string signature, long currentTimestamp) GenerateSignature(HttpMethod httpMethod, string targetUri, string? queryData)
    {
        long currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() + currentDeltaTimeWithOvh;

        using SHA1 sha1Hacher = SHA1.Create();

        string stringToSign = $"{ovhApplicationSecret}+{ovhConsumerKey}+{httpMethod}+{targetUri}+{queryData ?? string.Empty}+{currentTimestamp}";
        byte[] bytesToSign = Encoding.UTF8.GetBytes(stringToSign);
        byte[] binaryHash = sha1Hacher.ComputeHash(bytesToSign);

        string signature = string.Join("", binaryHash.Select(x => x.ToString("X2"))).ToLower();

        return ($"$1${signature}", currentTimestamp);
    }

    public IDictionary<string, string> GetHeaders()
    {
        return new Dictionary<string, string>()
        {
            { IOvhApiToken.OVH_APP_HEADER, ovhApplicationKey },
            { IOvhApiToken.OVH_CONSUMER_HEADER, ovhConsumerKey }
        };
    }
}