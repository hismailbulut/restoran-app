using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace restoran_app
{
    public class GetirModel
    {
        // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
        public record Client(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("contactPhoneNumber")] string ContactPhoneNumber,
            [property: JsonPropertyName("clientPhoneNumber")] string ClientPhoneNumber
        );

        public record Courier(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("status")] int Status,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("location")] Location Location
        );

        public record DisplayInfo(
            [property: JsonPropertyName("title")] Title Title
            // [property: JsonPropertyName("options")] Options Options
        );

        public record FoodOrder(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("status")] int Status,
            [property: JsonPropertyName("confirmationId")] string ConfirmationId,
            [property: JsonPropertyName("client")] Client Client,
            [property: JsonPropertyName("courier")] Courier Courier,
            [property: JsonPropertyName("products")] IReadOnlyList<Product> Products,
            [property: JsonPropertyName("clientNote")] string ClientNote,
            [property: JsonPropertyName("doNotKnock")] bool DoNotKnock,
            [property: JsonPropertyName("dropOffAtDoor")] bool DropOffAtDoor,
            [property: JsonPropertyName("totalPrice")] double TotalPrice,
            [property: JsonPropertyName("checkoutDate")] DateTime CheckoutDate,
            [property: JsonPropertyName("verifyDate")] DateTime VerifyDate,
            [property: JsonPropertyName("prepareDate")] DateTime PrepareDate,
            [property: JsonPropertyName("handoverDate")] DateTime HandoverDate,
            [property: JsonPropertyName("courierVerifyDate")] DateTime CourierVerifyDate,
            [property: JsonPropertyName("reachDate")] DateTime ReachDate,
            [property: JsonPropertyName("deliverDate")] DateTime DeliverDate,
            [property: JsonPropertyName("deliveryType")] int DeliveryType,
            [property: JsonPropertyName("isEcoFriendly")] bool IsEcoFriendly,
            [property: JsonPropertyName("restaurant")] string Restaurant,
            [property: JsonPropertyName("isScheduled")] bool IsScheduled,
            [property: JsonPropertyName("isCancelAvailable")] bool IsCancelAvailable,
            [property: JsonPropertyName("isTransferAvailable")] bool IsTransferAvailable,
            [property: JsonPropertyName("paymentMethodText")] PaymentMethodText PaymentMethodText,
            [property: JsonPropertyName("totalDiscountedPrice")] double? TotalDiscountedPrice,
            [property: JsonPropertyName("paymentMethod")] int? PaymentMethod
        );

        public record Location(
            [property: JsonPropertyName("lat")] double Lat,
            [property: JsonPropertyName("lon")] double Lon
        );

        public record Name(
            [property: JsonPropertyName("tr")] string Tr,
            [property: JsonPropertyName("en")] string En
        );

        public record Option(
            // [property: JsonPropertyName("option")] string Option, // something like id
            [property: JsonPropertyName("name")] Name Name,
            [property: JsonPropertyName("price")] int Price,
            [property: JsonPropertyName("tr")] IReadOnlyList<string> Tr,
            [property: JsonPropertyName("en")] IReadOnlyList<string> En
        );

        public record OptionCategory(
            // [property: JsonPropertyName("optionCategory")] string OptionCategory, // something like id
            [property: JsonPropertyName("name")] Name Name,
            [property: JsonPropertyName("options")] IReadOnlyList<Option> Options
        );

        public record PaymentMethodText(
            [property: JsonPropertyName("en")] string En,
            [property: JsonPropertyName("tr")] string Tr
        );

        public record Product(
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("imageURL")] string ImageURL,
            [property: JsonPropertyName("wideImageURL")] string WideImageURL,
            [property: JsonPropertyName("count")] int Count,
            // [property: JsonPropertyName("product")] string Product, // something like id
            [property: JsonPropertyName("name")] Name Name,
            [property: JsonPropertyName("price")] double Price,
            [property: JsonPropertyName("optionPrice")] int OptionPrice,
            [property: JsonPropertyName("priceWithOption")] double PriceWithOption,
            [property: JsonPropertyName("totalPrice")] double TotalPrice,
            [property: JsonPropertyName("totalOptionPrice")] int TotalOptionPrice,
            [property: JsonPropertyName("totalPriceWithOption")] double TotalPriceWithOption,
            [property: JsonPropertyName("optionCategories")] IReadOnlyList<OptionCategory> OptionCategories,
            [property: JsonPropertyName("displayInfo")] DisplayInfo DisplayInfo,
            [property: JsonPropertyName("note")] string Note,
            [property: JsonPropertyName("discountedPriceWithOption")] double? DiscountedPriceWithOption,
            [property: JsonPropertyName("totalDiscountedPriceWithOption")] double? TotalDiscountedPriceWithOption
        );

        public record OrdersResponse(
            [property: JsonPropertyName("foodOrders")] IReadOnlyList<FoodOrder> FoodOrders,
            [property: JsonPropertyName("foodOrdersCount")] int FoodOrdersCount,
            [property: JsonPropertyName("totalPrice")] double TotalPrice
        );

        public record Title(
            [property: JsonPropertyName("tr")] string Tr,
            [property: JsonPropertyName("en")] string En
        );
    }
}
