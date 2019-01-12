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
}
