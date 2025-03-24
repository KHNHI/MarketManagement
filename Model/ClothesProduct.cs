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
        public string Color { get; set; }

        public ClothesProduct() : base()
        {
            Category = ProductCategory.Clothes;
            AvailableSizes = new List<string>();
        }

        protected ClothesProduct(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            AvailableSizes = (List<string>)info.GetValue("AvailableSizes", typeof(List<string>));
            Material = info.GetString("Material");
            Color = info.GetString("Color");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("AvailableSizes", AvailableSizes);
            info.AddValue("Material", Material);
            info.AddValue("Color", Color);
        }
        
        // Phương thức tiện ích để lấy chuỗi các kích thước
        public string GetSizesAsString()
        {
            return string.Join(", ", AvailableSizes);
        }
    }
}
