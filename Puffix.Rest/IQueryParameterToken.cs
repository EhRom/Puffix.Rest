namespace Puffix.Rest;

public interface IQueryParameterToken : IToken
{
    string GetQueryParameterName();

    string GetQueryParameterValue();
}