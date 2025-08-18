using System.Net;
using System.Net.Http.Headers;

namespace Puffix.Rest;

public class ResultInformation<ResultT>(IDictionary<string, IEnumerable<string>> headers, HttpStatusCode responseCode, ResultT? responseContent, bool isSuccess, string errorContent) : IResultInformation<ResultT>
{
    public IDictionary<string, IEnumerable<string>> Headers { get; } = headers;

    public HttpStatusCode ResultCode { get; } = responseCode;

    public ResultT? ResultContent { get; } = responseContent;

    public bool IsSuccess { get; } = isSuccess;

    public string ErrorContent { get; } = errorContent;

    public static IDictionary<string, IEnumerable<string>> ExtractHttpHeaders(HttpResponseHeaders headers)
    {
        IDictionary<string, IEnumerable<string>> extractedHeaders = new Dictionary<string, IEnumerable<string>>();

        foreach (KeyValuePair<string, IEnumerable<string>> currentHeader in headers)
        {
            extractedHeaders[currentHeader.Key] = currentHeader.Value;
        }

        return extractedHeaders;
    }
}
