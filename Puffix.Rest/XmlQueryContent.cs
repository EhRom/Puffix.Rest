namespace Puffix.Rest;

public class XmlQueryContent(string queryContent) : StringQueryContent(queryContent), IQueryContent
{
    protected override string MimeType => "application/xml";

    public static IQueryContent CreateNew(string queryContent)
    {
        return new XmlQueryContent(queryContent);
    }
}