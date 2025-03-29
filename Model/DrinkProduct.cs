using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement.Model
{
    public class DrinkProduct : BaseProduct
    {
        public double Volume { get; set; }
        public bool IsAlcoholic { get; set; }

        public DrinkProduct() : base()
        {
            Category = ProductCategory.Drink;
        }

        protected DrinkProduct(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Volume = info.GetDouble("Volume");
            IsAlcoholic = info.GetBoolean("IsAlcoholic");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Volume", Volume);
            info.AddValue("IsAlcoholic", IsAlcoholic);
        }

        public override bool Validate()
        {
            return base.Validate() && (Volume > 0);
        }
    }
}
