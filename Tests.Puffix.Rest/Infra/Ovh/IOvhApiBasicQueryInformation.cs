using Puffix.Rest;

namespace Tests.Puffix.Rest.Infra.Ovh;

public interface IOvhApiBasicQueryInformation : IQueryInformation<IOvhApiToken>
{
    public static IDictionary<string, string> EmptyQueryParameters = new Dictionary<string, string>();
}