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
        public string Model { get; set; }
        public int WarrantyMonths { get; set; }

        public ApplianceProduct() : base()
        {
            Category = ProductCategory.Appliance;
        }

        protected ApplianceProduct(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Brand = info.GetString("Brand");
            Model = info.GetString("Model");
            WarrantyMonths = info.GetInt32("WarrantyMonths");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Brand", Brand);
            info.AddValue("Model", Model);
            info.AddValue("WarrantyMonths", WarrantyMonths);
        }
    }
}
