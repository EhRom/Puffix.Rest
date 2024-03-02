namespace Puffix.Rest;

public interface IHeaderToken : IToken
{
    IDictionary<string, string> GetHeaders();
}
