using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Net.Http;
using System.Globalization;

using RestSharp;
using RestSharp.Authenticators;

namespace restoran_app
{
    public class Yemeksepeti
    {
        string email;
        string password;
        private struct AuthInfo
        {
            public string accessToken;
            public string tokenType;
            public string vendor;
        }
        Task<AuthInfo> authInfo;
        RestClient authClient;
        RestClient ordersClient;
        public Yemeksepeti(string email, string password)
        {
            this.email = email;
            this.password = password;
            // Create clients
            authClient = new RestClient("https://vp-bff.api.eu.prd.portal.restaurant/auth/v4/token");
            ordersClient = new RestClient("https://os-backend.api.eu.prd.portal.restaurant/v1/vendors");
            // Authenticate
            // TODO: if accessToken expires we need to re-auth
            authInfo = Authenticate();
        }
        private async Task<AuthInfo> Authenticate()
        {
            var credentials = new Dictionary<string, string>();
            credentials.Add("username", email);
            credentials.Add("password", password);

            var response =
                await authClient.PostJsonAsync<Dictionary<string, string>, YemeksepetiModel.AuthResponse>("", credentials)
                ?? throw new Exception("yemeksepeti: null response");

            AuthInfo info = new AuthInfo();
            info.accessToken = response.AccessToken;
            info.tokenType = response.TokenType;
            info.vendor = response.AccessTokenContent.Vendors.YSTR.Codes[0];

            // Logger.LogInfo("YemekSepeti auth: {0}", info.accessToken);

            return info;
        }
        public async Task<List<Order>> FetchOrders()
        {
            // Wait for auth info
            var authInfo = await this.authInfo;
            // Generate request
            var request = new RestRequest("orders");
            request.AddHeader("Authorization", authInfo.tokenType + " " + authInfo.accessToken);
            var body = new Dictionary<string, dynamic>();
            body.Add("global_vendor_codes", new[] { "YS_TR;" + authInfo.vendor } );
            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddDays(-5);
            string timeFrom = startDate.ToString("yyyy-MM-ddTHH:mm:sszzz");
            string timeTo = endDate.ToString("yyyy-MM-ddTHH:mm:sszzz");
            body.Add("time_from", timeFrom.ToString());
            body.Add("time_to", timeTo.ToString());
            request.AddJsonBody(body);
            // Do request
            var response = await ordersClient.ExecutePostAsync<YemeksepetiModel.OrdersResponse>(request);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(string.Format("yemeksepeti: failed to fetch, status: {0}", response.StatusCode));
            }
            if (response.Data == null)
            {
                throw new Exception("yemeksepeti: null response");
            }
            // Logger.LogInfo("Yemeksepeti orders fetched. Content: {0}", response.Content);
            // Fetch details and parse orders
            // ConcurrentBag<Order> orders = new();
            List<Order> orders = new();
            // Parallel.ForEach(response.Data.Orders, async order =>
            foreach (var order in response.Data.Orders)
            {
                // Fetch order details
                var _request = new RestRequest(body["global_vendor_codes"][0] + "/orders/" + order.OrderId);
                _request.AddHeader("Authorization", authInfo.tokenType + " " + authInfo.accessToken);
                var _response = await ordersClient.ExecuteAsync<YemeksepetiModel.OrderDetailsResponse>(_request);
                if (!_response.IsSuccessStatusCode)
                {
                    // Continue, we will retry it again in next fetch
                    Logger.LogError("yemeksepeti: failed to fetch order details! Status: {0} ID: {1}", _response.StatusCode, order.OrderId);
                    continue;
                }
                if (_response.Data == null)
                {
                    Logger.LogError("yemeksepeti: null response in order details");
                    continue;
                }
                // Logger.LogInfo("Order Details: {0}", _response.Content);
                orders.Add(ParseOrder(order, _response.Data.Order));
            }
            return orders;
        }
        private Order ParseOrder(YemeksepetiModel.Order order, YemeksepetiModel.OrderDetails orderDetails)
        {
            // Parse status
            PackageStatus status = ParseOrderStatus(order.OrderStatus);
            if (status == PackageStatus.Unknown)
            {
                Logger.LogError("yemeksepeti: unknown package status: {0}", order.OrderStatus);
            }
            CustomerDetails customerDetails = new CustomerDetails(
                "",
                orderDetails.Delivery.AddressText,
                "",
                "",
                orderDetails.Delivery.City,
                "",
                "",
                "",
                orderDetails.Delivery.PostCode,
                ""
            );
            return new Order(
                API.YemekSepeti,
                order.OrderId,
                Common.StringToBigInteger(order.OrderId),
                order.OrderTimestamp, // DateTime.ParseExact(obj.OrderTimestamp, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
                null,
                customerDetails,
                double.Parse(orderDetails.Total, CultureInfo.InvariantCulture),
                ParseItems(orderDetails),
                "",
                status
            );
        }
        private List<Item> ParseItems(YemeksepetiModel.OrderDetails orderDetails)
        {
            List<Item> items = new();
            foreach (var item in orderDetails.Items)
            {
                for (int i = 0; i < item.Quantity; i++)
                {
                    List<ItemDescription> extras = new();
                    // Parse options
                    foreach (var option in item.Options)
                    {
                        for (int j = 0; j < option.Quantity; j++)
                        {
                            extras.Add(new ItemDescription(
                                long.Parse(option.Id),
                                option.Name,
                                double.Parse(option.UnitPrice, CultureInfo.InvariantCulture)
                            ));
                        }
                    }
                    // Logger.LogInfo("UnitPrice: {0}, Parsed: {1}", item.UnitPrice, double.Parse(item.UnitPrice));
                    // No modifier products (?)
                    // No removeds (?)
                    // We add options to extras but IDK is it true
                    items.Add(new Item(
                        new ItemDescription(
                            long.Parse(item.Id),
                            item.Name,
                            double.Parse(item.UnitPrice, CultureInfo.InvariantCulture)
                        ),
                        new(),
                        extras,
                        new()
                    ));
                }
            }
            return items;
        }
        private PackageStatus ParseOrderStatus(string status)
        {
            switch (status)
            {
                // TODO: find correct correspondings
                case "ACCEPTED":
                    return PackageStatus.Picking;
                case "PICKED_UP":
                    return PackageStatus.Invoiced;
                case "CANCELLED":
                    return PackageStatus.Cancelled;
                default:
                    return PackageStatus.Unknown;
            }
        }
    }
}
