using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement
{
    [Serializable]
    public class ElectronicProduct : BaseProduct
    {
        public string Brand { get; set; }
        public int WarrantyMonths { get; set; }

        public ElectronicProduct() : base() { }

        public ElectronicProduct(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Brand = info.GetString("Brand");
            WarrantyMonths = info.GetInt32("WarrantyMonths");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Brand", Brand);
            info.AddValue("WarrantyMonths", WarrantyMonths);
        }

        public override bool ValidateProduct()
        {
            return !string.IsNullOrEmpty(ProductName) &&
                   !string.IsNullOrEmpty(Brand) &&
                   ProductPrice > 0 &&
                   ProductQuantity >= 0 &&
                   WarrantyMonths > 0;
        }
    }
    [Serializable]
    public class FoodProduct : BaseProduct
    {
        public DateTime ExpiryDate { get; set; }
        public string StorageType { get; set; }  // Frozen, Cool, Normal
        public bool IsOrganic { get; set; }

        public FoodProduct() : base() { }

        public FoodProduct(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ExpiryDate = info.GetDateTime("ExpiryDate");
            StorageType = info.GetString("StorageType");
            IsOrganic = info.GetBoolean("IsOrganic");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ExpiryDate", ExpiryDate);
            info.AddValue("StorageType", StorageType);
            info.AddValue("IsOrganic", IsOrganic);
        }

        public override bool ValidateProduct()
        {
            return !string.IsNullOrEmpty(ProductName) &&
                   !string.IsNullOrEmpty(StorageType) &&
                   ProductPrice > 0 &&
                   ProductQuantity >= 0 &&
                   ExpiryDate > DateTime.Now;
        }

        // Phương thức đặc biệt cho FoodProduct
        public bool IsExpired()
        {
            return DateTime.Now > ExpiryDate;
        }

        public int DaysUntilExpiry()
        {
            return (ExpiryDate - DateTime.Now).Days;
        }
    }

    [Serializable]
    public class ClothesProduct : BaseProduct
    {
        public string Size { get; set; }  // S, M, L, XL
        public string Material { get; set; }
        public string Color { get; set; }
        public string Season { get; set; }  // Summer, Winter, All Season

        public ClothesProduct() : base() { }

        public ClothesProduct(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Size = info.GetString("Size");
            Material = info.GetString("Material");
            Color = info.GetString("Color");
            Season = info.GetString("Season");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Size", Size);
            info.AddValue("Material", Material);
            info.AddValue("Color", Color);
            info.AddValue("Season", Season);
        }

        public override bool ValidateProduct()
        {
            return !string.IsNullOrEmpty(ProductName) &&
                   !string.IsNullOrEmpty(Size) &&
                   !string.IsNullOrEmpty(Material) &&
                   !string.IsNullOrEmpty(Color) &&
                   ProductPrice > 0 &&
                   ProductQuantity >= 0;
        }

        // Phương thức đặc biệt cho ClothesProduct
        public bool IsSeasonalProduct()
        {
            return Season != "All Season";
        }
    }

}
