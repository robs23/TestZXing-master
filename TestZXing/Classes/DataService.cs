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
using TestZXing.Static;

namespace TestZXing.Classes
{
    public class DataService
    {
        public DataService()
        {

        }

        public async Task<String> readStream(HttpResponseMessage response)
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
