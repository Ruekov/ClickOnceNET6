using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClickOnceNet6.Methods
{
    internal static class WebRequest
    {
        public static string ReturnString(HttpClient client, string url) {

            using (HttpResponseMessage response = client.GetAsync(url).Result)
            {
                using (HttpContent content = response.Content)
                {
                   return content.ReadAsStringAsync().Result;
                }
            }
        }
    }
}
