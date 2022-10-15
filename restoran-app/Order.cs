using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace restoran_app
{
    // Just a plan
    public interface IOrder
    {
        string GetCustomerDetails();
        string GetStatusString();
        string GetNextStatusString();
        bool AdvanceStatus();
    }

    public enum API
    {
        Trendyol,
        Yemeksepeti,
        Getir
    }
    public enum PackageStatus
    {
        Unknown, // eğer bu görünürse bir hata var demektir
        Created, // Oluşturuldu
        Picking, // Kabul edildi, hazırlanıyor (tüm siparişler gelir gelmez bu duruma güncellenecek)
        Invoiced, // Hazırlandı, kuryeyi bekliyor
        Shipped, // Yola çıktı
        Unsupplied, // ?? (sanırım ürün bitti)
        Cancelled, // İptal edildi
        Delivered, // Teslim edildi
    }
    public class ItemDescription
    {
        public readonly long id;
        public readonly string name;
        public readonly double price;
        public ItemDescription(long id, string name, double price)
        {
            this.id = id;
            this.name = name;
            this.price = price;
        }
    }
    public class Item
    {
        public readonly ItemDescription description;
        public readonly List<Item> modifiers;
        public readonly List<ItemDescription> extras;
        public readonly List<ItemDescription> removed;
        public Item(ItemDescription description, List<Item> modifiers, List<ItemDescription> extras, List<ItemDescription> removed)
        {
            this.description = description;
            this.modifiers = modifiers;
            this.extras = extras;
            this.removed = removed;
        }
    }
    public class CustomerDetails
    {
        public readonly string name;
        public readonly string address;
        public readonly string neighborhood; // mahalle
        public readonly string district; // ilçe
        public readonly string city;
        public readonly string apartmentNumber;
        public readonly string doorNumber;
        public readonly string note; // hints about address
        public readonly string postalCode;
        public readonly string phone;
        public CustomerDetails(string name, string address, string neighborhood, string district, string city, string apartmentNumber, string doorNumber, string note, string postalCode, string phone)
        {
            this.name = name;
            this.address = address;
            this.neighborhood = neighborhood;
            this.district = district;
            this.city = city;
            this.apartmentNumber = apartmentNumber;
            this.doorNumber = doorNumber;
            this.note = note; 
            this.postalCode = postalCode;
            this.phone = phone;
        }
    }
    public class Order
    {
        public readonly API api;
        public readonly string origID;
        public readonly BigInteger id;
        public readonly DateTime creationTime;
        public DateTime? pickedTime;
        public readonly CustomerDetails customerDetails;
        public readonly double totalPrice;
        public readonly List<Item> items;
        public readonly string customerNote; // note to restaurant about menu
        public PackageStatus packageStatus;
        public Order(API api, string origID, BigInteger id, DateTime creationTime, DateTime? pickedTime, CustomerDetails customerDetails, double totalPrice, List<Item> items, string customerNote, PackageStatus packageStatus)
        {
            this.api = api;
            this.origID = origID;
            this.id = id;
            this.creationTime = creationTime;
            this.pickedTime = pickedTime;
            this.customerDetails = customerDetails;
            this.totalPrice = totalPrice;
            this.items = items;
            this.customerNote = customerNote;
            this.packageStatus = packageStatus;
            // For debugging
            if (!validatePrice())
            {
                Logger.LogError("order: price is not correct. Must: {0}, Calculated: {1}, API: {2}, Customer: {3}",
                    totalPrice.ToString(),
                    sumItemsPrice(items).ToString(),
                    api.ToString(),
                    customerDetails.name
                );
            }
        }
        public void SetPackageStatus(PackageStatus status)
        {
            this.packageStatus = status;
        }
        public string PackageStatusString()
        {
            switch (packageStatus)
            {
                case PackageStatus.Created:
                    return "Yeni";
                case PackageStatus.Picking:
                    return "Kabul edildi, hazırlanıyor";
                case PackageStatus.Invoiced:
                    return "Hazırlandı, kuryeyi bekliyor";
                case PackageStatus.Cancelled:
                    return "İptal edildi";
                case PackageStatus.Unsupplied:
                    return "TODO";
                case PackageStatus.Shipped:
                    return "Yola çıktı";
                case PackageStatus.Delivered:
                    return "Teslim edildi";
                default:
                    return "Bilinmiyor";
            }
        }
        private bool validatePrice()
        {
            return (Math.Abs(sumItemsPrice(items) - totalPrice) <= 0.001);
        }
        private double sumItemsPrice(List<Item> items)
        {
            double price = 0;
            foreach (Item item in items)
            {
                price += item.description.price;
                price += sumItemsPrice(item.modifiers);
                foreach (var extra in item.extras)
                {
                    price += extra.price;
                }
            }
            return price;
        }
    }
}
