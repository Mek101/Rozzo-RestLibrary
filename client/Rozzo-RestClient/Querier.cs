using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace Rozzo_RestClient
{
    class Querier : IDisposable
    {
        #region Fields
        enum Service : byte { QuantityOfIn = 1, EnumAllCatagory, EnumDateRange, EnumFromCart }

        protected sealed class JsonContent<TData> : IReadOnlyResponse<TData>
        {
            public HttpStatusCode StatusCode { private set; get; }
            public string Message { private set; get; }
            public TData Data { private set; get; }

            public JsonContent(string json)
            {
                JObject jObject = JObject.Parse(json);
                
                StatusCode = (HttpStatusCode)jObject["status"].ToObject<int>();
                Message = jObject["status_message"].ToObject<string>();
                
                if(StatusCode == HttpStatusCode.OK)
                    Data = jObject["data"].ToObject<TData>();
                else
                    Data = default(TData);
            }           
        }


        protected readonly Uri _remoteUrl;
        protected readonly HttpClient _client;

        public event EventHandler<string> OnDebuggingLog;
        #endregion


        #region Constructors
        public Querier(string remoteUrl, int port) : this(new UriBuilder(remoteUrl), port) { }

        public Querier(Uri remoteUrl, int port) : this(new UriBuilder(remoteUrl), port) { }

        private Querier(UriBuilder builder, int port)
        {
            builder.Port = port;
            _remoteUrl = builder.Uri;
            _client = new HttpClient();
        }

        public void Dispose() { _client.Dispose(); }
        #endregion


        #region Utilities
        private void Log(string log)
        {
            if (OnDebuggingLog != null)
                OnDebuggingLog(this, log);
        }

        private async Task<IReadOnlyResponse<T>> GetJsonResponseAsync<T>(string query, CancellationToken cancellationToken)
        {
            UriBuilder builder = new UriBuilder(_remoteUrl);
            builder.Query = query;
            Uri targetUrl = builder.Uri;

            Log("Targeting uri: " + targetUrl.ToString());

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
        public Task<IReadOnlyResponse<int>> QuantityOfInAsync(Category category, string repart, CancellationToken cancellationToken = default(CancellationToken))
        {
            string query = "name=" + ((byte)Service.QuantityOfIn).ToString() + "&category=" + category.ToString() + "&repart=" + repart;

            return GetJsonResponseAsync<int>(query, cancellationToken);
        }

        public Task<IReadOnlyResponse<Book[]>> EnumerateAllCategoryAsync(Category category, CancellationToken cancellationToken = default(CancellationToken))
        {
            string query = "name=" + ((byte)Service.EnumAllCatagory).ToString() + "&category=" + category.ToString();

            return GetJsonResponseAsync<Book[]>(query, cancellationToken);
        }

        public Task<IReadOnlyResponse<Book[]>> EnumerateDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default(CancellationToken))
        {
            string query = "name=" + ((byte)Service.EnumDateRange).ToString() + "&start=" + start.ToUniversalTime().ToShortTimeString() + "&end=" + end.ToUniversalTime().ToShortTimeString();

            return GetJsonResponseAsync<Book[]>(query, cancellationToken);
        }

        public Task<IReadOnlyResponse<Book[]>> EnumerateFromCartAsync(int cartCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            string query = "name=" + ((byte)Service.EnumFromCart).ToString() + "&cart_code=" + cartCode.ToString();

            return GetJsonResponseAsync<Book[]>(query, cancellationToken);
        }
        #endregion
    }
}
