using System.Text;

namespace Puffix.Rest;

public abstract class StringQueryContent(string queryContent) : IQueryContent
{
    private readonly string queryContent = queryContent;
    
    protected abstract string MimeType { get; }


    public HttpContent GetQueryContent()
    {
        return new StringContent(queryContent ?? string.Empty, Encoding.UTF8, MimeType);
    }
}
