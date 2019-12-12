using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MobileApplication
{
    class Database
    {
        public WebClient client;
        public Products products;
        private string dbUrl;
        private string errorMessage;
        private Request request;

        public Database()
        {
            InitializeWebClient();
        }

        private void InitializeWebClient()
        {
            client = new WebClient();
            client.Headers[HttpRequestHeader.ContentType] = "Content-Type:application/json";
            client.Headers[HttpRequestHeader.Authorization] = "Basic secret-6323";
            dbUrl = "https://f1m5kuz1va.execute-api.us-east-1.amazonaws.com/Stage/";
            request = new Request(client);
            products = new Products(client);
        }

        public bool UserLogin(string username, string password)
        {
            string url = dbUrl + "login";
            string parameters = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",}";
            if (request.Post(url, parameters) != null)
            {
                return true;
            }
            errorMessage = request.GetLastErrorMessage();
            return false;
        }

        public bool UserRegister(string username, string password)
        {
            string url = dbUrl + "register";
            string parameters = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",}";

            if (request.Post(url, parameters) != null)
            {
                return true;
            }
            errorMessage = request.GetLastErrorMessage();
            return false;
        }

        //entering full name is optional
        public bool UserRegister(string username, string password, string fullname)
        {
            string url = dbUrl + "register";
            string parameters = "{\"username\":\"" + username + "\",\"password\":\"" + password + "\",\"name\":\"" + fullname + "\"}";

            if (request.Post(url, parameters) != null)
            {
                return true;
            }
            errorMessage = request.GetLastErrorMessage();
            return false;
        }

        //check if the user exists
        public bool UserCheck(string username)
        {
            string url = dbUrl + "check";
            string parameters = "{\"username\":\"" + username + "\"}";

            if (request.Post(url, parameters) != null)
            {
                return true;
            }
            errorMessage = request.GetLastErrorMessage();
            return false;
        }

        public bool AddToUserInventory(string username, string upcCode)
        {
            string url = dbUrl + "adduserinv";
            string productName = products.GetName(upcCode);
            string productDesc = "asdf";
            string parameters = "{\"username\":\"" + username + "\",\"scanid\":\"" + upcCode + "\",\"productname\":\"" + productName + "\",\"description\":\"" + productDesc + "\"}";

            if (request.Post(url, parameters) != null)
            {
                return true;
            }
            errorMessage = request.GetLastErrorMessage();
            return false;
        }

        //returns products in JSON format. Returns empty array upon error
        public string GetUserInventory(string username)
        {
            string url = dbUrl + "getuserinv";
            string parameters = "{\"username\":\"" + username + "\",}";

            string inventory = request.Post(url, parameters);
            if (inventory != null)
            {
                return inventory;
            }
            errorMessage = request.GetLastErrorMessage();
            return null;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }
    }

    //Scrapes product info from source code
    class Products
    {
        WebClient client;
        string lastError = "";

        public Products(WebClient theClient)
        {
            client = theClient;
        }

        public string GetName(string upcCode)
        {
            string data;
            try
            {
                data = ScrapeProductData(upcCode);
            }
            catch (Exception e)
            {
                lastError = e.Message;
                return null;
            }

            string name;
            string splitString = upcCode + " associated  with ";
            int offset = data.LastIndexOf(splitString) + splitString.Length;

            name = data.Substring(offset);
            name = name.Substring(0, name.IndexOf("\">"));
            return name;
        }

        string GetDescription(string upcCode)
        {
            return "";
        }

        private string ScrapeProductData(string upcCode)
        {
            byte[] raw = client.DownloadData("https://www.barcodespider.com/" + upcCode);
            string data = Encoding.UTF8.GetString(raw);
            if (data.Contains("find upc number"))
            {
                throw new Exception("Could not find a product with this UPC");
            }
            return data;
        }

        public string GetProductData(string upcCode)
        {
            byte[] raw = client.DownloadData("https://api.barcodelookup.com/v2/products?barcode=" + upcCode + "&formatted=y&key=it5z09owihva4agg6jwnms0w06qihl");
            string data = Encoding.UTF8.GetString(raw);
            SimpleJSON.JSONNode node = SimpleJSON.JSON.Parse(data);
            string product = node["products"][0]["product_name"];

            return product;
        }

        public string GetLastErrorMessage()
        {
            return lastError;
        }
    }

    class Request
    {
        private WebClient client;
        private string lastError = "";

        public Request(WebClient theClient)
        {
            client = theClient;
        }

        public string Post(string url, string parameters)
        {
            string response;
            try
            {
                response = client.UploadString(url, parameters);
            }
            catch (Exception e)
            {
                lastError = e.Message;
                return null;
            }
            return response;
        }

        public string GetLastErrorMessage()
        {
            return lastError;
        }
    }
}
