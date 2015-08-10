using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WufooExporter
{
    public class RestApiCaller
    {
        public static async Task<string> GetJsonFromApi(int page)
        {
            using (var l_client = new HttpClient())
            {
                l_client.BaseAddress = new Uri("https://rpalaniappan.wufoo.com");
                l_client.DefaultRequestHeaders.Accept.Clear();
                l_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var l_byteArray = Encoding.ASCII.GetBytes("K5C1-8QS2-MCOU-LHKL:dummy");
                l_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(l_byteArray));

                // 0 os the CNS form. update accordingly
                HttpResponseMessage l_response = await l_client.GetAsync("api/v3/forms/0/entries.json?pageSize=100&pageStart="+page);
                if (l_response.IsSuccessStatusCode)
                {
                    var l_json = await l_response.Content.ReadAsStringAsync();
                    return l_json;


                }
            }
            return String.Empty;
        }
    }
}
