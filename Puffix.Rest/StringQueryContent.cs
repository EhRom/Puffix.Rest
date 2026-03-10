using System.Text;

namespace Puffix.Rest;

public class StringQueryContent(string queryContent) : IQueryContent
{
    private readonly string queryContent = queryContent;

    public static IQueryContent CreateNew(string queryContent)
    {
        return new StringQueryContent(queryContent);
    }

    public static IQueryContent EmptyContent => CreateNew(string.Empty);

    public HttpContent GetQueryContent()
    {
        return new StringContent(queryContent ?? string.Empty, Encoding.UTF8, "application/text");
    }
}
