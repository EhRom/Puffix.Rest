using System.Net;

namespace Puffix.Rest;

public class ResponseInformation(HttpStatusCode responseCode, string responseContent) : IResponseInformation<string>
{
    public HttpStatusCode ResponseCode { get; } = responseCode;

    public string ResponseContent { get; } = responseContent;
}


public class ResponseInformation<ResponseT>(HttpStatusCode responseCode, ResponseT responseContent) : IResponseInformation<ResponseT>
{
    public HttpStatusCode ResponseCode { get; } = responseCode;

    public ResponseT ResponseContent { get; } = responseContent;
}
