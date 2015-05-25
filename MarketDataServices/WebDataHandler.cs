using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace MarketDataServices
{
    internal static class WebDataHandler
    {
        public static IEnumerable<string> GetData(string url)
        {
            HttpWebRequest request = null;
            var result = new List<string>();
            request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
            request.Timeout = 30000;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (StreamReader input = new StreamReader(
                response.GetResponseStream()))
            {
                while (input.EndOfStream == false)
                {
                    result.Add(input.ReadLine());
                }
            }
            return result;
        }
    }
}
