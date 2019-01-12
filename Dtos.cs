using System.Runtime.Serialization;
using System;
using System.Globalization;
using System.Collections.Generic;

namespace WebAPIClient
{
    public class Token
    {
        public string message;
        public bool success;
        public string token;
    }
    public class Category
    {
        public List<string> data;
        public bool success;
        public string token;
    }
    public class MagazineInfo
    {
        public int id;
        public string name;
        public string category;
    }
    public class Magazine
    {
        public List<MagazineInfo> data;
        public bool success;
        public string token;
    }
    public class ResponseData
    {
        public string totalTime { get; set; }
        public bool answerCorrect { get; set; }
        public List<string> shouldBe { get; set; }
    }

    public class Response
    {
        public ResponseData data { get; set; }
        public bool success { get; set; }
        public string token { get; set; }
        public string message { get; set; }
    }
    public class Datum
    {
        public string id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public List<int> magazineIds { get; set; }
    }

    public class Subscriber
    {
        public List<Datum> data { get; set; }
        public bool success { get; set; }
        public string token { get; set; }
        public string message { get; set; }
    }
    public class Answer
    {
        public List<string> subscribers { get; set; }
    }
}
