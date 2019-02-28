using System.Net;

namespace Rozzo_RestClient
{
    public interface IReadOnlyResponse<T>
    {
        /// <summary>
        /// The operation result code.
        /// </summary>
        HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Message with operation execution details.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Operation results.
        /// </summary>
        T Data { get; }
    }
}
