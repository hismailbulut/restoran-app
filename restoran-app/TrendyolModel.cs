using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TrendyolModel
{
    public record Address(
        [property: JsonPropertyName("firstName")] string FirstName,
        [property: JsonPropertyName("lastName")] string LastName,
        [property: JsonPropertyName("company")] string Company,
        [property: JsonPropertyName("address1")] string Address1,
        [property: JsonPropertyName("address2")] string Address2,
        [property: JsonPropertyName("city")] string City,
        [property: JsonPropertyName("cityCode")] int CityCode,
        [property: JsonPropertyName("cityId")] int CityId,
        [property: JsonPropertyName("district")] string District,
        [property: JsonPropertyName("districtId")] int DistrictId,
        [property: JsonPropertyName("neighborhoodId")] int NeighborhoodId,
        [property: JsonPropertyName("neighborhood")] string Neighborhood,
        [property: JsonPropertyName("apartmentNumber")] string ApartmentNumber,
        [property: JsonPropertyName("floor")] string Floor,
        [property: JsonPropertyName("doorNumber")] string DoorNumber,
        [property: JsonPropertyName("addressDescription")] string AddressDescription,
        [property: JsonPropertyName("postalCode")] string PostalCode,
        [property: JsonPropertyName("countryCode")] string CountryCode,
        [property: JsonPropertyName("latitude")] string Latitude,
        [property: JsonPropertyName("longitude")] string Longitude,
        [property: JsonPropertyName("phone")] string Phone
    );

    public record Amount(
        [property: JsonPropertyName("seller")] double Seller
    );

    public record CancelInfo(
        [property: JsonPropertyName("reasonCode")] int ReasonCode
    );

    public record Content(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("supplierId")] int SupplierId,
        [property: JsonPropertyName("storeId")] int StoreId,
        [property: JsonPropertyName("orderCode")] string OrderCode,
        [property: JsonPropertyName("orderId")] string OrderId,
        [property: JsonPropertyName("orderNumber")] string OrderNumber,
        [property: JsonPropertyName("packageCreationDate")] long PackageCreationDate,
        [property: JsonPropertyName("packageModificationDate")] long PackageModificationDate,
        [property: JsonPropertyName("preparationTime")] int PreparationTime,
        [property: JsonPropertyName("totalPrice")] double TotalPrice,
        [property: JsonPropertyName("callCenterPhone")] string CallCenterPhone,
        [property: JsonPropertyName("deliveryType")] string DeliveryType,
        [property: JsonPropertyName("customer")] Customer Customer,
        [property: JsonPropertyName("address")] Address Address,
        [property: JsonPropertyName("packageStatus")] string PackageStatus,
        [property: JsonPropertyName("lines")] IReadOnlyList<Line> Lines,
        [property: JsonPropertyName("customerNote")] string CustomerNote,
        [property: JsonPropertyName("lastModifiedDate")] long LastModifiedDate,
        [property: JsonPropertyName("isCourierNearby")] bool IsCourierNearby,
        [property: JsonPropertyName("cancelInfo")] CancelInfo CancelInfo,
        [property: JsonPropertyName("eta")] string Eta
    );

    public record Coupon(
        [property: JsonPropertyName("couponId")] string CouponId,
        [property: JsonPropertyName("sellerCoverageRatio")] double SellerCoverageRatio,
        [property: JsonPropertyName("amount")] Amount Amount
    );

    public record Customer(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("firstName")] string FirstName,
        [property: JsonPropertyName("lastName")] string LastName
    );

    public record ExtraIngredient(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("price")] double Price
    );

    public record Item(
        [property: JsonPropertyName("packageItemId")] string PackageItemId,
        [property: JsonPropertyName("lineItemId")] long LineItemId,
        [property: JsonPropertyName("isCancelled")] bool IsCancelled,
        [property: JsonPropertyName("promotions")] IReadOnlyList<Promotion> Promotions,
        [property: JsonPropertyName("coupons")] IReadOnlyList<Coupon> Coupons
    );

    public record Line(
        [property: JsonPropertyName("price")] double Price,
        [property: JsonPropertyName("unitSellingPrice")] double UnitSellingPrice,
        [property: JsonPropertyName("productId")] int ProductId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("items")] IReadOnlyList<Item> Items,
        [property: JsonPropertyName("modifierProducts")] IReadOnlyList<ModifierProduct> ModifierProducts,
        [property: JsonPropertyName("extraIngredients")] IReadOnlyList<ExtraIngredient> ExtraIngredients,
        [property: JsonPropertyName("removedIngredients")] IReadOnlyList<RemovedIngredient> RemovedIngredients
    );

    public record ModifierProduct(
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("price")] double Price,
        [property: JsonPropertyName("productId")] int ProductId,
        [property: JsonPropertyName("modifierGroupId")] int ModifierGroupId,
        [property: JsonPropertyName("modifierProducts")] IReadOnlyList<ModifierProduct> ModifierProducts,
        [property: JsonPropertyName("extraIngredients")] IReadOnlyList<ExtraIngredient> ExtraIngredients,
        [property: JsonPropertyName("removedIngredients")] IReadOnlyList<RemovedIngredient> RemovedIngredients
    );

    public record Promotion(
        [property: JsonPropertyName("promotionId")] int PromotionId,
        [property: JsonPropertyName("discountType")] string DiscountType,
        [property: JsonPropertyName("sellerCoverageRatio")] double SellerCoverageRatio,
        [property: JsonPropertyName("amount")] Amount Amount
    );

    public record RemovedIngredient(
        [property: JsonPropertyName("id")] int Id,
        [property: JsonPropertyName("name")] string Name
    );

    public record ResponseHeader(
        [property: JsonPropertyName("page")] int Page,
        [property: JsonPropertyName("size")] int Size,
        [property: JsonPropertyName("totalPages")] int TotalPages,
        [property: JsonPropertyName("totalCount")] int TotalCount,
        [property: JsonPropertyName("content")] IReadOnlyList<Content> Content
    );
}
