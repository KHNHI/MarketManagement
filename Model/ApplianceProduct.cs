using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement.Model
{
    public class ApplianceProduct : BaseProduct
    {
        public string Brand { get; set; }
        public int WarrantyMonths { get; set; }

        public ApplianceProduct() : base()
        {
            Category = ProductCategory.Appliance;
        }

        protected ApplianceProduct(SerializationInfo info, StreamingContext context) : base(info, context)
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
    }
}
