using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement
{
    [Serializable]
    public abstract class BaseProduct : ISerializable
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int ProductQuantity { get; set; }
        public decimal ProductPrice { get; set; }
        public string Description { get; set; }
        public string CategoryName { get; set; }

        protected BaseProduct()
        {
            ProductId = GenerateId();
        }

        protected BaseProduct(SerializationInfo info, StreamingContext context)
        {
            ProductId = info.GetString("ProductId");
            ProductName = info.GetString("ProductName");
            ProductQuantity = info.GetInt32("ProductQuantity");
            ProductPrice = info.GetDecimal("ProductPrice");
            Description = info.GetString("Description");
            CategoryName = info.GetString("CategoryName");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ProductId", ProductId);
            info.AddValue("ProductName", ProductName);
            info.AddValue("ProductQuantity", ProductQuantity);
            info.AddValue("ProductPrice", ProductPrice);
            info.AddValue("Description", Description);
            info.AddValue("CategoryName", CategoryName);
        }

        protected string GenerateId()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public abstract bool ValidateProduct();
    }
}
