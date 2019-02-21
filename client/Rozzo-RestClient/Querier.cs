using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;


namespace Rozzo_RestClient
{
    class Querier
    {
        #region Fields
        private class JsonContent<TData> : IReadOnlyResponse<TData>
        {
            public HttpStatusCode StatusCode { private set; get; }
            public string Message { private set; get; }
            public TData Data { private set; get; }

            public JsonContent(string json)
            {
                JObject jObject = JObject.Parse(json);
                
                StatusCode = (HttpStatusCode)jObject["status"].ToObject<int>();
                Message = jObject["status_message"].ToObject<string>();
                
                if(StatusCode == HttpStatusCode.Success)
                    Data = jObject["data"].ToObject<TData>();
                else
                    Data = default(TData);
            }           
        }
        
        enum Service : byte { QuantityOfIn = 1, EnumAllCatagory, EnumDateRange, EnumFromCart }

        private readonly Uri _remoteUrl;

        public event EventHandler<string> OnDebuggingLog;
        #endregion


        #region Constructors
        public Querier(string remoteUrl, int port) : this(new UriBuilder(remoteUrl), port) { }

        public Querier(Uri remoteUrl, int port) : this(new UriBuilder(remoteUrl), port) { }

        private Querier(UriBuilder builder, int port)
        {
            builder.Port = port;
            _remoteUrl = builder.Uri;
        }
        #endregion


        #region Utilities
        private void Log(string log)
        {
            if (OnDebuggingLog != null)
                OnDebuggingLog(this, log);
        }

        private async Task<IReadOnlyResponse<T>> GetJsonResponseAsync<T>(string query)
        {
            Log("Creating query: " + query);
            using(HttpClient client = new HttpClient())
            {
                UriBuilder builder = new UriBuilder(_remoteUrl);
                builder.Query = query;
                Uri targetUrl = builder.Uri;

                Log("Targeting uri: " + targetUrl.ToString());

                using(HttpResponseMessage httpResponse = await client.GetAsync(targetUrl))
                {
                    using(HttpContent content = httpResponse.Content)
                    {
                        string responseContent = await content.ReadAsStringAsync();

                        Log("Raw received response: " + responseContent);

                        return new JsonContent<T>(responseContent);
                    }
                }
            }
        }
        #endregion


        #region Public interface
        public Task<IReadOnlyResponse<int>> QuantityOfIn(Category category, string repart)
        {
            string query = "name=" + ((byte)Service.QuantityOfIn).ToString() + "&category=" + category.ToString() + "&repart=" + repart;

            return GetJsonResponseAsync<int>(query);
        }

        public Task<IReadOnlyResponse<Book[]>> EnumAllCategory(Category category)
        {
            string query = "name=" + ((byte)Service.EnumAllCatagory).ToString() + "&category=" + category.ToString();

            return GetJsonResponseAsync<Book[]>(query);
        }

        public Task<IReadOnlyResponse<Book[]>> EnumDateRange(DateTime start, DateTime end)
        {
            string query = "name=" + ((byte)Service.EnumDateRange).ToString() + "&start=" + start.ToUniversalTime().ToShortTimeString() + "&end=" + end.ToUniversalTime().ToShortTimeString();

            return GetJsonResponseAsync<Book[]>(query);
        }

        public Task<IReadOnlyResponse<Book[]>> EnumFromCart(int cartCode)
        {
            string query = "name=" + ((byte)Service.EnumFromCart).ToString() + "&cart_code=" + cartCode.ToString();

            return GetJsonResponseAsync<Book[]>(query);
        }
        #endregion
    }
}
