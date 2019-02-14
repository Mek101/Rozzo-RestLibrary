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
            private static JavaScriptSerializer _deserializer = new JavaScriptSerializer();

            public static JsonContent<TData> GetContent(string jsonContent)
            {
                System.Windows.MessageBox.Show(jsonContent);
                return _deserializer.Deserialize<JsonContent<TData>>(jsonContent);
            }

            public HttpStatusCode StatusCode { get; set; }
            public string Message { get; set; }
            public TData Data { get; set; }
        }
        
        enum Service : byte { QuantityOfIn = 1, EnumAllCatagory, EnumDateRange, EnumFromCart }

        private Uri _remoteUrl;
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
        private string BuildRequestQuery(Service service, params object[] arguments)
        {
            string query = "?name=" + ((byte)service).ToString();
            foreach (object arg in arguments)
                query += "&" + arg.ToString().ToLower() + "=" + arg.ToString().ToLower();

            return query;           
        }

        private async Task<IReadOnlyResponse<T>> GetJsonResponseAsync<T>(string query)
        {
            using (HttpClient client = new HttpClient())
            {
                UriBuilder builder = new UriBuilder(_remoteUrl);
                builder.Path += query;

                using (HttpResponseMessage response = await client.GetAsync(builder.Uri))
                {
                    using(HttpContent content = response.Content)
                    {
                        string jsonResponse = await content.ReadAsStringAsync();

                        return JsonContent<T>.GetContent(jsonResponse);
                    }
                }
            }
        }
        #endregion


        #region Public interface
        public Task<IReadOnlyResponse<int>> QuantityOfIn(Category category, string repart)
        {
            string query = BuildRequestQuery(Service.QuantityOfIn, category, repart);

            return GetJsonResponseAsync<int>(query);
        }
        #endregion
    }
}
