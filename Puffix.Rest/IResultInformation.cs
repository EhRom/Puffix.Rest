using System.Net;

namespace Puffix.Rest;

public interface IResultInformation<ResultT>
{
    HttpStatusCode ResultCode { get; }

    ResultT? ResultContent { get; }

    bool IsSuccess { get; }

    string ErrorContent { get; }
}
