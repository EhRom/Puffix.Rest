using System.Net;

namespace Puffix.Rest;

public interface IResultInformation<ResultT>
{
    IDictionary<string, IEnumerable<string>> Headers { get; }

    HttpStatusCode ResultCode { get; }

    ResultT? ResultContent { get; }

    bool IsSuccess { get; }

    string ErrorContent { get; }
}
