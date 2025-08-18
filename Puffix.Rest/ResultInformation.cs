using System.Net;

namespace Puffix.Rest;

public class ResultInformation<ResultT>(HttpStatusCode responseCode, ResultT? responseContent, bool isSuccess, string errorContent) : IResultInformation<ResultT>
{
    public HttpStatusCode ResultCode { get; } = responseCode;

    public ResultT? ResultContent { get; } = responseContent;

    public bool IsSuccess { get; } = isSuccess;

    public string ErrorContent { get; } = errorContent;
}
