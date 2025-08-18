using System.Net;

namespace Puffix.Rest;

public class ResusltInformation(HttpStatusCode responseCode, string responseContent) : IResultInformation<string>
{
    public HttpStatusCode ResultCode { get; } = responseCode;

    public string ResultContent { get; } = responseContent;
}


public class ResusltInformation<ResultT>(HttpStatusCode responseCode, ResultT responseContent) : IResultInformation<ResultT>
{
    public HttpStatusCode ResultCode { get; } = responseCode;

    public ResultT ResultContent { get; } = responseContent;
}
