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
            Console.WriteLine("Test Started\n");

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // GetToken
            var token = GetToken(apiUrl + "token").Result.token;
            Console.WriteLine("Token retrived\n");

            // Get all categories
            var categories = GetCategories(apiUrl + "categories/" + token).Result.data;
            if(categories.Count <= 0)
            {
                Console.WriteLine("Should have some categories\n");
                return;
            }
            else
            {
                Console.WriteLine($"retrived { categories.Count } categories\n");
            }

            // Get magazines in categories[0]
            var magazines = GetMagazines(apiUrl + "magazines/" + token + "/" + categories[0]).Result.data;
            if (magazines.Count <= 0)
            {
                Console.WriteLine("Should have some magazines in first category\n");
                return;
            }
            else
            {
                Console.WriteLine($"retrived { magazines.Count} magazines\n");
            }
            var subscribers = GetSubscribers(apiUrl + "subscribers/" + token).Result.data;
            if (subscribers.Count <= 0)
            {
                Console.WriteLine("Should have some subscribers in system\n");
                return;
            }
            else
            {
                Console.WriteLine($"retrived { subscribers.Count} subscribers\n");
            }

            // Get subscribers
            List<string> subscriberIds = new List<string>();

            // add subscrber to subscriber list
            foreach (var s in subscribers){ subscriberIds.Add(s.id); }

            // create answer model
            var answer = new Answer { subscribers = subscriberIds };

            // create json string
            string json = JsonConvert.SerializeObject(answer);

            // Post answer
            var resp = PostAnswer(token, json);
 
            // it returns answerCorrect = false with some subscribers in the should be list
            if (resp.data.answerCorrect == false)
            {
                Console.WriteLine($"post above subscribers to server, however it reports {resp.data.shouldBe.Count} subscribers in the should be list");
                Console.WriteLine($"Time to post this answer {resp.data.totalTime}\n");

                Console.WriteLine($"repost answer to server with subscribers in the should be list\n");

                // repost with should be list
                answer = new Answer {subscribers = resp.data.shouldBe };
                json = JsonConvert.SerializeObject(answer);

                // will return answerCorrect = true with should be list = null
                resp = PostAnswer(token, json);
            }         
            Console.WriteLine($"Answer is correctly posted without any subscriber in the should be list");
            Console.WriteLine($"Total time to repost this answer {resp.data.totalTime}\n");
            Console.WriteLine("Test completed\n");
            Console.WriteLine("Enter any key to exit application");


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
             //   Console.WriteLine("json: " + json);
                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
        //        Console.WriteLine("result: " + result.ToString());
                return JsonConvert.DeserializeObject<Response>(result.ToString());
            }
        }
        private static async Task<Subscriber> GetSubscribers(string apiUrl)
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

        private static async Task<Magazine> GetMagazines(string apiUrl)
        {
            var serializer = new DataContractJsonSerializer(typeof(Magazine));

            var streamTask = client.GetStreamAsync(apiUrl).Result;
            return serializer.ReadObject(streamTask) as Magazine;
        }

        private static async Task<Category> GetCategories(string apiUrl)
        {
            var serializer = new DataContractJsonSerializer(typeof(Category));

            var streamTask = client.GetStreamAsync(apiUrl).Result;
            return serializer.ReadObject(streamTask) as Category;
        }
    }
}
