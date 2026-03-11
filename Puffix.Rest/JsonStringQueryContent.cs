using System.Text;

namespace Puffix.Rest;

public class JsonStringQueryContent(string queryContent) : IQueryContent
{
    private readonly string queryContent = queryContent;

    public static IQueryContent CreateNew(string queryContent)
    {
        return new JsonStringQueryContent(queryContent);
    }

    public static IQueryContent EmptyContent => CreateNew(string.Empty);

    public HttpContent GetQueryContent()
    {
        return new StringContent(queryContent ?? string.Empty, Encoding.UTF8, "application/text");
    }
}