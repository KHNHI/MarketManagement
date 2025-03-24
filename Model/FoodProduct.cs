using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement.Model
{
    public class FoodProduct : BaseProduct
    {
        public DateTime ExpiryDate { get; set; }
        public string StorageCondition { get; set; }

        public FoodProduct() : base()
        {
            Category = ProductCategory.Food;
        }

        protected FoodProduct(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ExpiryDate = info.GetDateTime("ExpiryDate");
            StorageCondition = info.GetString("StorageCondition");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ExpiryDate", ExpiryDate);
            info.AddValue("StorageCondition", StorageCondition);
        }

        public override bool Validate()
        {
            return base.Validate() && ExpiryDate > DateTime.Now;
        }
    }
}
