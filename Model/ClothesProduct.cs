using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement.Model
{
    public class ClothesProduct : BaseProduct
    {
        public List<string> AvailableSizes { get; set; }
        public string Material { get; set; }

        public ClothesProduct() : base()
        {
            Category = ProductCategory.Clothes;
            AvailableSizes = new List<string>();
        }

        protected ClothesProduct(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            AvailableSizes = (List<string>)info.GetValue("AvailableSizes", typeof(List<string>));
            Material = info.GetString("Material");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("AvailableSizes", AvailableSizes);
            info.AddValue("Material", Material);
        }
        
        public override bool Validate()
        {
            return base.Validate() && 
                   AvailableSizes != null && 
                   AvailableSizes.Count > 0;
        }
        
        public string GetSizesAsString()
        {
            return string.Join(", ", AvailableSizes);
        }
    }
}
