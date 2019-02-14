using System.Net;

namespace Rozzo_RestClient
{
    public interface IReadOnlyResponse<T>
    {
        HttpStatusCode StatusCode { get; }
        string Message { get; }
        T Data { get; }
    }
}
