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
            DateTime startDate = endDate.AddDays(-10);
            Dictionary<string, dynamic> body = new();
            body.Add("startDate", startDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            body.Add("endDate", endDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            body.Add("sortType", "DESC");
            body.Add("status", new[] { 500, 900, 1000, 1500, 1600 });
            body.Add("sortField", "createdAt");
            request.AddJsonBody(body);

            var response = await client.ExecuteAsync<GetirModel.OrdersResponse>(request, Method.Post);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("GetirYemek: failed to fetch orders. Status: {0}", response.StatusCode));
            }
            if (response.Data == null)
            {
                throw new Exception("GetirYemek: null response");
            }

            List<Order> orders = new();
            // iterate through each order and get their details
            foreach (var order in response.Data.FoodOrders)
            {
                // parse customer details
                CustomerDetails customerDetails = new CustomerDetails(
                    order.Client.Name,
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    "",
                    order.Client.ClientPhoneNumber
                );
                orders.Add(new Order(
                    API.Getir,
                    order.Id,
                    Common.StringToBigInteger(order.Id),
                    order.CheckoutDate,
                    null,
                    customerDetails,
                    order.TotalPrice,
                    ParseItems(order),
                    order.ClientNote,
                    ParseStatus(order.Status)
                ));
            }
            return orders;
        }
        private List<Item> ParseItems(GetirModel.FoodOrder order)
        {
            List<Item> items = new();
            foreach (var product in order.Products)
            {
                for (int i = 0; i < product.Count; i++)
                {
                    List<ItemDescription> extras = new();
                    List<ItemDescription> removed = new();
                    foreach (var optCategory in product.OptionCategories)
                    {
                        if (optCategory.Name.Tr.ToLowerInvariant().Contains("çıkar"))
                        {
                            // append to removeds
                            foreach (var opt in optCategory.Options)
                            {
                                removed.Add(new ItemDescription(
                                    -1,
                                    opt.Name.Tr,
                                    opt.Price
                                ));
                            }
                        }
                        else
                        {
                            // append to extras
                            foreach (var opt in optCategory.Options)
                            {
                                extras.Add(new ItemDescription(
                                    -1,
                                    opt.Name.Tr,
                                    opt.Price
                                ));
                            }
                        }
                    }
                    items.Add(new Item(
                        new ItemDescription(
                            0, //long.Parse(product.Id, System.Globalization.NumberStyles.HexNumber),
                            product.Name.Tr,
                            product.PriceWithOption
                        ),
                        new(),
                        extras,
                        removed
                    ));
                }
            }
            return items;
        }
        private PackageStatus ParseStatus(int status)
        {
            switch (status)
            {
                case 500:
                    return PackageStatus.Created;
                case 900:
                    return PackageStatus.Delivered;
                default:
                    return PackageStatus.Unknown;
            }
        }
    }
}