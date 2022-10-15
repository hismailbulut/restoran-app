using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YemeksepetiModel
{
    // FIRST FETCH LIST OF ORDERS
    public record AccessTokenContent(
        [property: JsonPropertyName("aud")] string Aud,
        [property: JsonPropertyName("country")] string Country,
        [property: JsonPropertyName("exp")] int Exp,
        [property: JsonPropertyName("iat")] int Iat,
        [property: JsonPropertyName("impersonator")] bool Impersonator,
        [property: JsonPropertyName("iss")] string Iss,
        [property: JsonPropertyName("sub")] string Sub,
        [property: JsonPropertyName("user")] User User,
        [property: JsonPropertyName("vendors")] Vendors Vendors
    );

    public record KeymakerResponse(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("refresh_token")] string RefreshToken,
        [property: JsonPropertyName("meta")] object Meta,
        [property: JsonPropertyName("device_token")] string DeviceToken
    );

    public record AuthResponse(
        [property: JsonPropertyName("accessToken")] string AccessToken,
        [property: JsonPropertyName("refreshToken")] string RefreshToken,
        [property: JsonPropertyName("tokenType")] string TokenType,
        [property: JsonPropertyName("role")] string Role,
        [property: JsonPropertyName("user")] User User,
        [property: JsonPropertyName("accessTokenContent")] AccessTokenContent AccessTokenContent,
        [property: JsonPropertyName("accessTokenV2")] string AccessTokenV2,
        [property: JsonPropertyName("keymaker_response")] KeymakerResponse KeymakerResponse,
        [property: JsonPropertyName("force_reset_password")] bool ForceResetPassword
    );

    public record User(
        [property: JsonPropertyName("createdAt")] DateTime CreatedAt,
        [property: JsonPropertyName("email")] string Email,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("locale")] string Locale,
        [property: JsonPropertyName("role")] string Role
    );

    public record Vendors(
        [property: JsonPropertyName("YS_TR")] YSTR YSTR
    );

    public record YSTR(
        [property: JsonPropertyName("codes")] IReadOnlyList<string> Codes
    );

    public record Order(
        [property: JsonPropertyName("identifier")] string Identifier,
        [property: JsonPropertyName("order_id")] string OrderId,
        [property: JsonPropertyName("global_vendor_code")] string GlobalVendorCode,
        [property: JsonPropertyName("vendor_name")] string VendorName,
        [property: JsonPropertyName("order_status")] string OrderStatus,
        [property: JsonPropertyName("order_timestamp")] DateTime OrderTimestamp,
        [property: JsonPropertyName("billable_status")] string BillableStatus
    );

    public record OrdersResponse(
        [property: JsonPropertyName("orders")] IReadOnlyList<Order> Orders
    );
    
    // SECOND FETCH DETAILED ORDER
    public record AcceptedDetails(
        [property: JsonPropertyName("estimated_delivery_time")] DateTime EstimatedDeliveryTime,
        [property: JsonPropertyName("source")] string Source,
        [property: JsonPropertyName("comment")] string Comment
    );

    public record Delivery(
        [property: JsonPropertyName("provider")] string Provider,
        [property: JsonPropertyName("fee")] string Fee,
        [property: JsonPropertyName("postCode")] string PostCode,
        [property: JsonPropertyName("city")] string City,
        [property: JsonPropertyName("address_text")] string AddressText,
        [property: JsonPropertyName("packaging_charges")] string PackagingCharges
    );

    public record DisplayedAtVendorDetails(
        [property: JsonPropertyName("provider")] string Provider,
        [property: JsonPropertyName("comment")] string Comment
    );

    public record Item(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("quantity")] int Quantity,
        [property: JsonPropertyName("unit_price")] string UnitPrice,
        [property: JsonPropertyName("parent_name")] string ParentName,
        [property: JsonPropertyName("options")] IReadOnlyList<Option> Options
    );

    public record Metadata(
        [property: JsonPropertyName("source")] string Source
    );

    public record Option(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("quantity")] int Quantity,
        [property: JsonPropertyName("unit_price")] string UnitPrice
    );

    public record OrderDetails(
        [property: JsonPropertyName("identifier")] string Identifier,
        [property: JsonPropertyName("order_id")] string OrderId,
        [property: JsonPropertyName("global_vendor_code")] string GlobalVendorCode,
        [property: JsonPropertyName("vendor_name")] string VendorName,
        [property: JsonPropertyName("order_timestamp")] DateTime OrderTimestamp,
        [property: JsonPropertyName("total")] string Total,
        [property: JsonPropertyName("is_billable")] string IsBillable,
        [property: JsonPropertyName("payment")] Payment Payment,
        [property: JsonPropertyName("delivery")] Delivery Delivery,
        [property: JsonPropertyName("items")] IReadOnlyList<Item> Items,
        [property: JsonPropertyName("version")] int Version,
        [property: JsonPropertyName("changed_at")] object ChangedAt
    );

    public record OrderStatus(
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("timestamp")] DateTime Timestamp,
        [property: JsonPropertyName("billable")] string Billable,
        [property: JsonPropertyName("sending_to_vendor_details")] SendingToVendorDetails SendingToVendorDetails,
        [property: JsonPropertyName("metadata")] Metadata Metadata,
        [property: JsonPropertyName("sent_to_transmission_details")] SentToTransmissionDetails SentToTransmissionDetails,
        [property: JsonPropertyName("displayed_at_vendor_details")] DisplayedAtVendorDetails DisplayedAtVendorDetails,
        [property: JsonPropertyName("accepted_details")] AcceptedDetails AcceptedDetails,
        [property: JsonPropertyName("picked_up_details")] PickedUpDetails PickedUpDetails
    );

    public record Payment(
        [property: JsonPropertyName("total")] string Total,
        [property: JsonPropertyName("discount")] string Discount,
        [property: JsonPropertyName("voucher")] string Voucher,
        [property: JsonPropertyName("minimum_order_value_fee")] string MinimumOrderValueFee
    );

    public record PickedUpDetails(
        [property: JsonPropertyName("timestamp")] DateTime Timestamp
    );

    public record OrderDetailsResponse(
        [property: JsonPropertyName("order")] OrderDetails Order,
        [property: JsonPropertyName("order_statuses")] IReadOnlyList<OrderStatus> OrderStatuses,
        [property: JsonPropertyName("previous_versions")] IReadOnlyList<object> PreviousVersions
    );

    public record SendingToVendorDetails(
        [property: JsonPropertyName("estimated_delivery_time")] DateTime EstimatedDeliveryTime,
        [property: JsonPropertyName("committed_pickup_time")] DateTime CommittedPickupTime,
        [property: JsonPropertyName("comment")] string Comment
    );

    public record SentToTransmissionDetails(
        [property: JsonPropertyName("provider")] string Provider,
        [property: JsonPropertyName("comment")] string Comment
    );
}
