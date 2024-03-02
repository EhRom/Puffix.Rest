using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ovh;

public interface IOvhApiToken : IHeaderToken
{
    public const string OVH_APP_HEADER = "X-Ovh-Application";
    public const string OVH_CONSUMER_HEADER = "X-Ovh-Consumer";
    public const string OVH_TIME_HEADER = "X-Ovh-Timestamp";
    public const string OVH_SIGNATURE_HEADER = "X-Ovh-Signature";

    void SetReferenceUnixTime(string referenceUnixTime);

    (string signature, long currentTimestamp) GenerateSignature(HttpMethod httpMethod, string targetUri, string? queryData);
}