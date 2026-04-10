namespace Puffix.Rest;

public class FormQueryContent(IDictionary<string, string> queryContent) : IQueryContent
{
    private readonly IDictionary<string, string> queryContent = queryContent;

    public static IQueryContent CreateNew(IDictionary<string, string> queryContent)
    {
        return new FormQueryContent(queryContent);
    }

    public HttpContent? GetQueryContent()
    {
        FormUrlEncodedContent content = new FormUrlEncodedContent(queryContent);
        return content;
    }
}

