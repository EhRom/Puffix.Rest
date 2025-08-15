namespace Puffix.Rest;

public interface IHeaderToken : IToken
{
    IDictionary<string, IEnumerable<string>> GetHeaders();
}
