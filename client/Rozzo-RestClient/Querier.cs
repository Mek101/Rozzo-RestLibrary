using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rozzo_RestClient
{    
    class Querier
    {
        enum Service : byte { QuantityOfIn, EnumAllCatagory, EnumDateRange, EnumFromCart }

        private Uri _remoteUrl;

        public Querier(string remoteUrl, int port)
        {
            UriBuilder uriBuilder = new UriBuilder(remoteUrl);
            uriBuilder.Port = port;
            _remoteUrl = uriBuilder.Uri;
        }


        private string BuildRequestQuery(byte serviceNum, params object[] arguments)
        {
            string query = "?" + serviceNum.ToString();
            foreach (object arg in arguments)
                query += "&" + arg.GetType().ToString().ToLower() + "=" + arg.ToString();

            return query;           
        }


        public Task<int> QuantityOfIn(Category category, string repart)
        {
            WebRequest request = WebRequest.Create(new Uri(_remoteUrl,  ));



        }
    }
}
