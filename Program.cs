using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebAPIClient
{
    class Program
    {
        private static HttpClient client = new HttpClient();
        private static string apiUrl = "http://magazinestore.azurewebsites.net/api/";

        static void Main(string[] args)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var token = GetToken(apiUrl + "token").Result.token;
            var subscribers = GetSubscribers(apiUrl + "subscribers/" + token).data;

            // Get subscriber Ids

            List<string> subscriberIds = new List<string>();
            foreach (var s in subscribers)
            { subscriberIds.Add(s.id); }

            // create answer model

            var answer = new Answer { subscribers = subscriberIds };
            string json = JsonConvert.SerializeObject(answer);

            // Post answer, it returns answerCorrect = false with should be list

            var resp = PostAnswer(token, json);

            // repost with should be list
            answer = new Answer { subscribers = resp.data.shouldBe };
            json = JsonConvert.SerializeObject(answer);

            // will return answerCorrect = true with should be list = null
            resp = PostAnswer(token, json);

            Console.ReadLine();
        }
        public static Response PostAnswer(string token, string json)
        {
            string URL = apiUrl + "answer/" + token;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpWebRequest.ContentType = "application/json";

            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                Console.WriteLine("json: " + json);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Console.WriteLine("result: " + result.ToString());
                return JsonConvert.DeserializeObject<Response>(result.ToString());
            }
        }
        private static Subscriber GetSubscribers(string apiUrl)
        {
            var serializer = new DataContractJsonSerializer(typeof(Subscriber));
            var streamTask = client.GetStreamAsync(apiUrl).Result;
            return serializer.ReadObject(streamTask) as Subscriber;
        }

        private static async Task<Token> GetToken(string apiUrl)
        {
            var serializer = new DataContractJsonSerializer(typeof(Token));
            var streamTask = client.GetStreamAsync(apiUrl).Result;
            return serializer.ReadObject(streamTask) as Token;
        }
    }
}
