using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace restoran_app
{
    public class Getir
    {
        string restaurantID;
        string token; // TODO: find a way to get this dynamically
        RestClient client;
        public Getir(string restaurantID, string token)
        {
            this.restaurantID = restaurantID;
            this.token = token;
            client = new RestClient("https://food-panel-backend.getirapi.com/");
        }
        /*
        public async Task Authenticate()
        {

        }
        */
        public async Task<List<Order>> FetchOrders()
        {
            // Fetch orders
            var request = new RestRequest("restaurants/" + restaurantID + "/food-orders");
            request.AddHeader("restaurantid", restaurantID);
            request.AddHeader("token", token);
            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddDays(-5);
            Dictionary<string, dynamic> body = new();
            body.Add("startDate", startDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            body.Add("endDate", endDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            body.Add("sortType", "DESC");
            body.Add("status", new[] { 500, 900, 1000, 1500, 1600 });
            body.Add("sortField", "createdAt");
            request.AddJsonBody(body);

            var response = await client.ExecuteAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("GetirYemek: failed to fetch orders. Status: {0}", response.StatusCode));
            }
            if (response.Content == null)
            {
                throw new Exception("GetirYemek: null response");
            }

            Logger.LogInfo("GetirYemek response: {0}", response.Content);

            return new List<Order>();
        }
    }
}
