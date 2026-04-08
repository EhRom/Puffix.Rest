namespace Puffix.Rest;

public class TextQueryContent(string queryContent) : StringQueryContent(queryContent), IQueryContent
{
    protected override string MimeType => "application/text";

    public static IQueryContent CreateNew(string queryContent)
    {
        return new TextQueryContent(queryContent);
    }
}
