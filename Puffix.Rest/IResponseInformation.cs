using System.Net;

namespace Puffix.Rest;

public interface IResponseInformation<ResponseT>
{
    HttpStatusCode ResponseCode { get; }

    ResponseT ResponseContent { get; }
}
