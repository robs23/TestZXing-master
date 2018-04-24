using ModernHttpClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TestZXing.Models;

namespace TestZXing.Classes
{
    public class DataService
    {
        public DataService()
        {

        }

        public async Task<string> ReloadUsers()
        {
            //using (var client = new HttpClient())
            //{
            //    try
            //    {
            //        var response = await client.GetStringAsync(url);
            //        return JsonConvert.DeserializeObject<List<User>>(response);
            //    }
            //    catch (Exception ex)
            //    {
            //        Debug.WriteLine(ex.Message);
            //        throw;
            //    }
            //}
            string url = "https://jsonplaceholder.typicode.com/posts";

            try
            {
                HttpClient httpClient = new HttpClient(new NativeMessageHandler() { Timeout = new TimeSpan(0, 0, 20), EnableUntrustedCertificates = true, DisableCaching = true });
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                return await readStream(await httpClient.SendAsync(request));
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        async Task<String> readStream(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                {
                    string text = reader.ReadToEnd();
                    Debug.WriteLine("RECEIVED: " + text);
                    return text;

                }
            }

            return null;
        }

    }
}
