using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using OBDApp.Models;
using RestSharp;
using Newtonsoft.Json;

using System.Net.Http;

namespace OBDApp.ViewModels
{
    public class WebServicesHandler
    {
        public WebServicesHandler() { }

        public void SaveRecordsUsingRest(ObdParameters obdParameters)
        {
            var client = new RestClient("http://3.120.132.27/api/OBDData");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Cache-Control","no-cache");
            request.AddHeader("Content-Type","application/json");

            string json = JsonConvert.SerializeObject(obdParameters);
            request.AddParameter("undefined", json, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Debug.WriteLine("Item Added: "+ json);
        }

        public void GetRecordsRest()
        {
            var client = new RestClient("http://3.120.132.27/api/OBDData");
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            string output = response.Content;
            Debug.WriteLine(output);
            Debug.WriteLine("The number of characters is: "+ output.Length);
            Debug.WriteLine("The number of records are: " + output.Count(x => x == '{'));

        }

        
        public async Task<bool> SaveItemAsync(ObdParameters obdParameters, bool isNewItem = false)
        {

            HttpClient client = new HttpClient();
            client.MaxResponseContentBufferSize = 256000;

            string RestUrl = "http://3.120.132.27/api/OBDData/";
            var uri = new Uri(string.Format(RestUrl, string.Empty));


            string json = JsonConvert.SerializeObject(obdParameters);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = null;
            if (isNewItem)
            {
                response = await client.PostAsync(uri, content);
            }


            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("**** Item has been Saved ****");
                return true;
            }
            else
            {
                Debug.WriteLine("**** Item has not been Saved ****");
                return false;
            }

        }
    }
}
