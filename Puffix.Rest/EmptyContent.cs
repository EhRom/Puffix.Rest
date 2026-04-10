namespace Puffix.Rest;

public class EmptyContent : IQueryContent
{
    public static IQueryContent CreateNew()
    {
        return new EmptyContent();
    }

    public HttpContent? GetQueryContent()
    {
        return null;
    }
}
