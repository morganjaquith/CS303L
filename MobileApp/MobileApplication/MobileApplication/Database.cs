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
            string parameters = "{\"username\":\"" + username.ToUpper() + "\",\"password\":\"" + password + "\",}";
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
            string parameters = "{\"username\":\"" + username.ToUpper() + "\",\"password\":\"" + password + "\",}";
            if (!UserLogin(username, password))
            {
                if (request.Post(url, parameters) != null)
                {
                    return true;
                }
                errorMessage = request.GetLastErrorMessage();
            }
            return false;
        }

        //entering full name is optional
        public bool UserRegister(string username, string password, string fullname)
        {
            string url = dbUrl + "register";
            string parameters = "{\"username\":\"" + username.ToUpper() + "\",\"password\":\"" + password + "\",\"name\":\"" + fullname + "\"}";

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
            string[] data = products.GetProductData(upcCode);
            string productName = data[0];
            string productDesc = data[1];
            string productImage = data[2];
            string parameters = "{\"username\":\"" + username + "\",\"scanid\":\"" + upcCode + "\",\"productname\":\"" + productName + "\",\"description\":\"" + productDesc + "\"}";

            if (request.Post(url, parameters) != null)
            {
                return true;
            }
            errorMessage = request.GetLastErrorMessage();
            return false;
        }

        //returns products in JSON format. Returns empty array upon error
        public string[,] GetUserInventory(string username)
        {
            string url = dbUrl + "getuserinv";
            string parameters = "{\"username\":\"" + username + "\",}";

            string jsonInventory = request.Post(url, parameters);
            if (jsonInventory != null)
            {
                SimpleJSON.JSONNode node = SimpleJSON.JSON.Parse(jsonInventory);
                string[,] inventory = new string[node["inventory"].Count, 4];
                for (int i = 0; i < node["inventory"].Count; i++)
                {
                    SimpleJSON.JSONNode item = SimpleJSON.JSON.Parse(node["inventory"][i]);
                    inventory[i, 0] = item["scanid"];
                    inventory[i, 1] = item["productname"];
                    inventory[i, 2] = item["description"];
                    inventory[i, 3] = products.GetProductData(inventory[i, 0])[3];
                }
                return inventory;
            }
            errorMessage = request.GetLastErrorMessage();
            return null;
        }

        public bool RemoveFromUserInventory(string username, string upcCode)
        {
            string url = dbUrl + "deleteuserinv";
            string[] data = products.GetProductData(upcCode);
            string parameters = "{\"username\":\"" + username + "\",\"scanid\":\"" + upcCode + "\",\"productname\":\"" + data[0] + "\",\"description\":\"" + data[1] + "\"}";

            if (request.Post(url, parameters) != null)
            {
                return true;
            }
            errorMessage = request.GetLastErrorMessage();
            return false;
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

        string name;
        string desc;
        string imagesource;

        public Products(WebClient theClient)
        {
            client = theClient;
        }

        public string GetProduct(string upcCode)
        {
            string[] data;
            try
            {
                data = GetProductData(upcCode);
            }
            catch (Exception e)
            {
                lastError = e.Message;
                return null;
            }

            name = data[0];
            desc = data[1];
            imagesource = data[2];
            return name;
        }

        public string[] GetProductData(string upcCode)
        {
            string[] productData = new string[4];
            byte[] raw = client.DownloadData("https://api.barcodelookup.com/v2/products?barcode=" + upcCode + "&formatted=y&key=jxk68km5g89sjjl9bh45xhiecoqjwq");
            string data = Encoding.UTF8.GetString(raw);
            SimpleJSON.JSONNode node = SimpleJSON.JSON.Parse(data);
            productData[0] = node["products"][0]["barcode_number"];
            productData[1] = node["products"][0]["product_name"];
            productData[2] = node["products"][0]["description"];
            productData[3] = node["products"][0]["images"][0];

            return productData;
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
