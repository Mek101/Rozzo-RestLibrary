﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
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
            private const string STATUS_MESSAGE = "status_message";
            private const string DATA = "data";

            public HttpStatusCode StatusCode { private set; get; }
            public string Message { private set; get; }
            public TData Data { private set; get; }

            public JsonContent(string json)
            {
                JObject jObject = JObject.Parse(json);

                if (json.Contains(STATUS))                
                    StatusCode = (HttpStatusCode)jObject[STATUS].ToObject<int>();
                else                
                    StatusCode = HttpStatusCode.InternalServerError;                

                if (json.Contains(STATUS_MESSAGE))
                    Message = jObject[STATUS_MESSAGE].ToObject<string>();
                else
                    Message = string.Empty;

                if (json.Contains(DATA) && StatusCode == HttpStatusCode.OK)
                    Data = jObject[DATA].ToObject<TData>();
                else
                    Data = default(TData);
            }           
        }


        protected UriBuilder _remoteUrlBuilder;
        protected static HttpClient _client;

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


        private async Task<IReadOnlyResponse<T>> QueryAsync<T>(string query, CancellationToken cancellationToken)
        {
            // Re-assigns the query statement.
            _remoteUrlBuilder.Query = query;
            Uri targetUrl = _remoteUrlBuilder.Uri;

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
            string query = GetServiceName(Service.QuantityOfIn) + "&category=" + category.ToString() + "&repart=" + repart;

            return QueryAsync<int>(query, cancellationToken);
        }

        public Task<IReadOnlyResponse<ReadOnlyBook[]>> EnumerateAllCategoryAsync(Category category, CancellationToken cancellationToken = default(CancellationToken))
        {
            string query = GetServiceName(Service.EnumAllCatagory) + "&category=" + category.ToString();

            return QueryAsync<ReadOnlyBook[]>(query, cancellationToken);
        }

        public Task<IReadOnlyResponse<ReadOnlyBook[]>> EnumerateDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default(CancellationToken))
        {
            string query = GetServiceName(Service.EnumDateRange) + "&start=" + start.ToUniversalTime().ToShortDateString() + "&end=" + end.ToUniversalTime().ToShortDateString();

            return QueryAsync<ReadOnlyBook[]>(query, cancellationToken);
        }

        public Task<IReadOnlyResponse<ReadOnlyBook[]>> EnumerateFromCartAsync(int cartCode, CancellationToken cancellationToken = default(CancellationToken))
        {
            string query = GetServiceName(Service.EnumFromCart) + "&cart_code=" + cartCode.ToString();

            return QueryAsync<ReadOnlyBook[]>(query, cancellationToken);
        }
        #endregion
    }
}
