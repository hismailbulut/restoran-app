using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Numerics;

using RestSharp;
using RestSharp.Authenticators;

namespace restoran_app
{
    public class Trendyol
    {
        string email;
        string sellerID;
        string apiKey;
        string apiSecret;
        RestClient client;
        public Trendyol(string email, string sellerID, string apiKey, string apiSecret)
        {
            this.email = email;
            this.sellerID = sellerID;
            this.apiKey = apiKey;
            this.apiSecret = apiSecret;
            // Create client
            client = new RestClient("https://api.trendyol.com/mealgw/suppliers/")
            {
                Authenticator = new HttpBasicAuthenticator(apiKey, apiSecret)
            };
            // Initialize default headers
            client.AddDefaultHeader("x-agentname", sellerID + " - SelfIntegration");
            client.AddDefaultHeader("x-executor-user", email);
            // client.AddDefaultHeader("Authorization", "Basic " + Common.Base64Encode(apiKey + ":" + apiSecret));
        }
        public async Task<List<Order>> FetchOrders()
        {
            // Fetch with headers
            // string URL = "https://api.trendyol.com/mealgw/suppliers/" + this.sellerID.ToString() + "/packages?" +
            // "packageModificationStartDate=" + startDateUnix.ToString() + "&" +
            // "packageModificationEndDate=" + endDateUnix.ToString();

            // Only fetch orders in a day
            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddDays(-1);
            long startDateUnix = ((DateTimeOffset)startDate).ToUnixTimeMilliseconds();
            long endDateUnix = ((DateTimeOffset)endDate).ToUnixTimeMilliseconds();

            // var request = new RestRequest(this.sellerID.ToString() + "/packages");

            var response = await client.GetJsonAsync<TrendyolModel.ResponseHeader>(
                this.sellerID.ToString() + "/packages",
                new
                {
                    packageModificationStartDate = startDateUnix.ToString(),
                    packageModificationEndDate = endDateUnix.ToString(),
                }
            ) ?? throw new Exception("trendyol: null response");

            int totalItems = response.TotalCount;
            // Copy all content to our new array
            List<TrendyolModel.Content> orders = new List<TrendyolModel.Content>();
            orders.AddRange(response.Content);
            // Header includes current page and page size, in order to get all content we need to fetch all pages
            // Because we already fetched first page, continue by adding one
            for (int i = response.Page + 1; i < response.TotalPages; i++)
            {
                // Add page param to url and fetch
                var _response = await client.GetJsonAsync<TrendyolModel.ResponseHeader>(
                    this.sellerID.ToString() + "packages",
                    new
                    {
                        packageModificationStartDate = startDateUnix.ToString(),
                        packageModificationEndDate = endDateUnix.ToString(),
                        page = i.ToString()
                    }
                ) ?? throw new Exception("trendyol: null response");
                orders.AddRange(_response.Content);
                totalItems += _response.TotalCount;
            }
            if (totalItems != orders.Count)
            {
                Logger.LogError("trendyol: expected {0}, but only {1} fetched.", totalItems, orders.Count);
            }
            return ParseOrders(orders);
        }
        private List<Order> ParseOrders(List<TrendyolModel.Content> orders)
        {
            List<Order> result = new List<Order>();
            foreach (var tOrder in orders)
            {
                BigInteger id = BigInteger.Parse(tOrder.Id, System.Globalization.NumberStyles.HexNumber);
                DateTime creationTime = Common.DateTimeFromUnixMillis(tOrder.PackageCreationDate);
                CustomerDetails customerDetails = new CustomerDetails(
                    tOrder.Address.FirstName + " " + tOrder.Address.LastName,
                    tOrder.Address.Address1 + " " + tOrder.Address.Address2,
                    tOrder.Address.Neighborhood,
                    tOrder.Address.District,
                    tOrder.Address.City,
                    tOrder.Address.ApartmentNumber,
                    tOrder.Address.DoorNumber,
                    tOrder.Address.AddressDescription,
                    tOrder.Address.PostalCode,
                    tOrder.Address.Phone
                );
                List<Item> items = new List<Item>();
                foreach (var line in tOrder.Lines)
                {
                    var count = line.Items.Count;
                    for (int i = 0; i < count; i++)
                    {
                        items.Add(new Item(
                            new ItemDescription(line.ProductId, line.Name, line.Price),
                            ParseModifiers(line.ModifierProducts),
                            ParseExtraIngredients(line.ExtraIngredients),
                            ParseRemovedIngredients(line.RemovedIngredients)
                        ));
                    }
                }
                PackageStatus packageStatus = PackageStatus.Unknown;
                switch (tOrder.PackageStatus)
                {
                    case "Created":
                        packageStatus = PackageStatus.Created;
                        break;
                    case "Picking":
                        packageStatus = PackageStatus.Picking;
                        break;
                    case "Invoiced":
                        packageStatus = PackageStatus.Invoiced;
                        break;
                    case "Cancelled":
                        packageStatus = PackageStatus.Cancelled;
                        break;
                    case "Unsupplied":
                        packageStatus = PackageStatus.Unsupplied;
                        break;
                    case "Shipped":
                        packageStatus = PackageStatus.Shipped;
                        break;
                    case "Delivered":
                        packageStatus = PackageStatus.Delivered;
                        break;
                }
                // Ceate order and append to result
                result.Add(new Order(
                    API.Trendyol,
                    tOrder.Id,
                    id,
                    creationTime,
                    null,
                    customerDetails,
                    tOrder.TotalPrice,
                    items,
                    tOrder.CustomerNote,
                    packageStatus
                ));
            }
            return result;
        }
        private List<Item> ParseModifiers(IReadOnlyList<TrendyolModel.ModifierProduct> products)
        {
            List<Item> modifiers = new List<Item>();
            foreach (var product in products)
            {
                modifiers.Add(new Item(
                    new ItemDescription(product.ProductId, product.Name, product.Price),
                    ParseModifiers(product.ModifierProducts),
                    ParseExtraIngredients(product.ExtraIngredients),
                    ParseRemovedIngredients(product.RemovedIngredients)
                ));
            }
            return modifiers;
        }
        private List<ItemDescription> ParseExtraIngredients(IReadOnlyList<TrendyolModel.ExtraIngredient> ingredients)
        {
            List<ItemDescription> result = new List<ItemDescription>();
            foreach (var ing in ingredients)
            {
                result.Add(new ItemDescription(ing.Id, ing.Name, ing.Price));
            }
            return result;
        }
        private List<ItemDescription> ParseRemovedIngredients(IReadOnlyList<TrendyolModel.RemovedIngredient> ingredients)
        {
            List<ItemDescription> result = new List<ItemDescription>();
            foreach (var ing in ingredients)
            {
                result.Add(new ItemDescription(ing.Id, ing.Name, 0));
            }
            return result;
        }
    }
}
