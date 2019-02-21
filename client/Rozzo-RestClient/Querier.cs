using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                StatusCode = (HttpStatusCode)GetValueOf<int>(json, "status");
                Message = GetValueOf<string>(json, "status_message");
                Data = GetValueOf<TData>(json, "data");
            }

            private TValue GetValueOf<TValue>(string json , string name)
            {
                int start = json.IndexOf(name) + 2 + name.Length;
                if (json[start] != '\"')
                    throw new Exception("Not a \"!");

                string res = json.Substring()

            }            
        }
        
        enum Service : byte { QuantityOfIn = 1, EnumAllCatagory, EnumDateRange, EnumFromCart }

        private Uri _remoteUrl;

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


        #region Query utilities
        private async Task<IReadOnlyResponse<T>> GetJsonResponseAsync<T>(string query)
        {
            Log("Creating query: " + query);
            using (HttpClient client = new HttpClient())
            {
                UriBuilder builder = new UriBuilder(_remoteUrl);
                builder.Query += query;

                Log("Targeting uri: " + builder.Uri.ToString());

                using (HttpResponseMessage response = await client.GetAsync(builder.Uri))
                {
                    using(HttpContent content = response.Content)
                    {
                        string jsonResponse = await content.ReadAsStringAsync();

                        Log("Received response: " + jsonResponse);

                        return new JsonContent<T>(jsonResponse);
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
        #endregion


        private void Log(string log)
        {
            if (OnDebuggingLog != null)
                OnDebuggingLog(this, log);
        }
    }
}
