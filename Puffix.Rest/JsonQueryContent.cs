namespace Puffix.Rest;

public class JsonQueryContent(string queryContent) : StringQueryContent(queryContent), IQueryContent
{
    protected override string MimeType => "application/json";

    public static IQueryContent CreateNew(string queryContent)
    {
        return new JsonQueryContent(queryContent);
    }
}