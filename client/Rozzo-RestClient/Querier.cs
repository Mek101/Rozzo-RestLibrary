using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Rozzo_RestClient
{
    class Querier
    {
        #region Fields
        enum Service : byte { QuantityOfIn = 1, EnumAllCatagory, EnumDateRange, EnumFromCart }

        protected sealed class JsonContent<TData> : IReadOnlyResponse<TData>
        {
            private const string STATUS = "status";
            private const string STATUS_MESSAGE = "message";
            private const string DATA = "data";

            public HttpStatusCode StatusCode { private set; get; }
            public string Message { private set; get; }
            public TData Data { private set; get; }

            public JsonContent(string json)
            {
                JObject jObject = null;
                string parseError = null;

                // Attemps to parse the string to json.
                try { jObject = JObject.Parse(json); }
                catch (Exception e) { parseError = e.Message; }

                if(jObject != null && json.Contains(STATUS))                
                    StatusCode = (HttpStatusCode)jObject[STATUS].ToObject<int>();
                else                
                    StatusCode = HttpStatusCode.InternalServerError;

                if (jObject != null && json.Contains(STATUS_MESSAGE))
                    Message = jObject[STATUS_MESSAGE].ToObject<string>();
                else if (!string.IsNullOrEmpty(parseError))
                    Message = parseError;
                else
                    Message = string.Empty;

                if (jObject != null && json.Contains(DATA) && StatusCode == HttpStatusCode.OK)
                    Data = jObject[DATA].ToObject<TData>();
                else
                    Data = default(TData);
            }           
        }


        protected UriBuilder _remoteUrlBuilder;
        protected static HttpClient _client;

        /// <summary>
        /// Raised when a method outputs a log message.
        /// </summary>
        public event EventHandler<string> OnDebuggingLog;
        #endregion


        #region Constructors
        static Querier() { _client = new HttpClient(); }

        public Querier(string remoteUrl, int port) : this(new UriBuilder(remoteUrl), port) { }

        public Querier(Uri remoteUrl, int port) : this(new UriBuilder(remoteUrl), port) { }

        private Querier(UriBuilder builder, int port)
        {
            builder.Port = port;
            _remoteUrlBuilder = builder;
        }

        ~Querier() { OnDebuggingLog = null; }

        public Querier(string remoteUrl) : this(new UriBuilder(remoteUrl)) { }

        public Querier(Uri remoteUrl) : this(new UriBuilder(remoteUrl)) { }

        private Querier(UriBuilder builder) { _remoteUrlBuilder = builder; }
        #endregion


        #region Utilities
        private void Log(string log)
        {
            if (OnDebuggingLog != null)
                OnDebuggingLog(this, log);
        }


        private string GetServiceName(Service service) { return "name=" + ((byte)service).ToString(); }

        private string GetCategory(Category category) { return "&category=" + category.ToString(); }

        private string FormatDate(DateTime date)
        {
            DateTime universalDate = date.ToUniversalTime();

            return universalDate.Year + "-" + universalDate.Month + "-" + universalDate.Day;
        }


        private async Task<IReadOnlyResponse<T>> QueryAsync<T>(string query, CancellationToken cancellationToken)
        {
            // Re-assigns the query statement.
            _remoteUrlBuilder.Query = query;
            Uri targetUrl = _remoteUrlBuilder.Uri;

            Log("Target uri: " + targetUrl.ToString());

            using(HttpResponseMessage httpResponse = await _client.GetAsync(targetUrl, cancellationToken))
            {
                using(HttpContent content = httpResponse.Content)
                {
                    string responseContent = await content.ReadAsStringAsync();

                    Log("Raw received response: " + responseContent);

                    return new JsonContent<T>(responseContent);
                }
            }
        }
        #endregion


        #region Public interface
        /// <summary>
        /// Returns the number of books of a given category in a given repart.
        /// </summary>
        /// <param name="category">The book category.</param>
        /// <param name="repart">The book repart.</param>
        /// <param name="cancellationToken">Token to cancel the query.</param>
        /// <returns>The number of books.</returns>
        public Task<IReadOnlyResponse<int>> QuantityOfInAsync(Category category, string repart, CancellationToken cancellationToken = default(CancellationToken))
        {
            string query = GetServiceName(Service.QuantityOfIn) + GetCategory(category) + "&repart=" + repart;

            return QueryAsync<int>(query, cancellationToken);
        }


        /// <summary>
        /// Enumerates all the books in a given category.
        /// </summary>
        /// <param name="category">The books category.</param>
        /// <param name="cancellationToken">Token to cancel the query.</param>
        /// <returns>All the books in the category.</returns>
        public Task<IReadOnlyResponse<ReadOnlyBook[]>> EnumerateAllCategoryAsync(Category category, CancellationToken cancellationToken = default(CancellationToken))
        {
            string query = GetServiceName(Service.EnumAllCatagory) + GetCategory(category);

            return QueryAsync<ReadOnlyBook[]>(query, cancellationToken);
        }


        /// <summary>
        /// Get the titles of all books withing the given range of time.
        /// </summary>
        /// <param name="start">The range starting date.</param>
        /// <param name="end">The range end date.</param>
        /// <param name="cancellationToken">Token to cancel the query.</param>
        /// <returns>The titles of the books.</returns>
        public Task<IReadOnlyResponse<string[]>> EnumerateDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default(CancellationToken))
        {
            string query = GetServiceName(Service.EnumDateRange) + "&start=" + FormatDate(start) + "&end=" + FormatDate(end);

            return QueryAsync<string[]>(query, cancellationToken);
        }


        /// <summary>
        /// Enumerates all the books in the given cart.
        /// </summary>
        /// <param name="cartCode">The target cart code.</param>
        /// <param name="cancellationToken">Token to cancel the query.</param>
        /// <returns>All the books in the cart.</returns>
        public Task<IReadOnlyResponse<BookWithUser>> EnumerateFromCartAsync(int cartCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            string query = GetServiceName(Service.EnumFromCart) + "&cart=" + cartCode.ToString();

            return QueryAsync<BookWithUser>(query, cancellationToken);
        }
        #endregion
    }
}
