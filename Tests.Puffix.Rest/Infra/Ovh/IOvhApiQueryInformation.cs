using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ovh;

public interface IOvhApiQueryInformation : IBasicQueryInformation<IOvhApiToken>
{
    public static IDictionary<string, string> EmptyQueryParameters = new Dictionary<string, string>();
}