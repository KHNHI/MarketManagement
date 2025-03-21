using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement
{

    [Serializable]
    public class BaseProduct : BaseEntity, ISerializable
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public List<BaseProduct> Products { get; set; }
        public BaseProduct()
        {
            Id = GenerateId();
        }
        public string GetId() => Id;

        protected BaseProduct(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetString("Id");
            ProductName = info.GetString("ProductName");
            Quantity = info.GetInt32("Quantity");
            Price = info.GetDecimal("Price");
            Description = info.GetString("Description");
            Category = info.GetString("Category");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("ProductName", ProductName);
            info.AddValue("Quantity", Quantity);
            info.AddValue("Price", Price);
            info.AddValue("Description", Description);
            info.AddValue("Category", Category);
        }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(ProductName) &&
                   Price >= 0 &&
                   Quantity >= 0;
        }

        public override string GenerateId()
        {
            if (Category == "Electronic")
            {
                return "EL" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            else if (Category == "Food")
            {
                return "FO" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            else if (Category == "Clothes")
            {
                return "CL" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            else
            {
                return "O" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }
    }
}
