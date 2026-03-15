using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBookCloud.Core.Api.Connectors.Impl
{
    public class GoogleBookApiConnector
    {
        public void GetVolumeInfo()
        {

        }

        private HttpClient GetGoogleBookApiHttpClient(Uri baseUri, string token, int timeout = 180)
        {
            var httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeout)
            };

            httpClient.BaseAddress = baseUri;

            return httpClient;
        }
    }
}
