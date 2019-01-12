using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace WebAPIClient
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            string apiUrl = "http://magazinestore.azurewebsites.net/api/";
            var token = GetToken(apiUrl + "token").Result.token;
            var categories = GetCategories(apiUrl + "categories/"+token).Result.data;
            var magazines = GetMagazines(apiUrl + "categories/" + token + "magazines/" + categories[0]).Result;
        }
        private static async Task<Magazine> GetMagazines(string apiUrl)
        {
            var serializer = new DataContractJsonSerializer(typeof(Magazine));

            var streamTask = client.GetStreamAsync(apiUrl).Result;
            var Magazines = serializer.ReadObject(streamTask) as Magazine;
            return Magazines;
        }

        private static async Task<Category> GetCategories(string apiUrl)
        {
            var serializer = new DataContractJsonSerializer(typeof(Category));
          
            var streamTask = client.GetStreamAsync(apiUrl).Result;
            var categories = serializer.ReadObject(streamTask) as Category;
            return categories;
        }
        private static async Task<Token> GetToken(string apiUrl)
        {
            var serializer = new DataContractJsonSerializer(typeof(Token));         
            var streamTask = client.GetStreamAsync(apiUrl).Result;
            var token = serializer.ReadObject(streamTask) as Token;
            return token;
        }
    }
}
