using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement
{
    [Serializable]
    public class ProductList : ISerializable
    {
        public List<BaseProduct> Products { get; private set; }

        public ProductList()
        {
            Products = new List<BaseProduct>();
        }

        protected ProductList(SerializationInfo info, StreamingContext context)
        {
            Products = (List<BaseProduct>)info.GetValue("Products", typeof(List<BaseProduct>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Products", Products);
        }

        public void Add(BaseProduct product)
        {
            if (product.ValidateProduct())
            {
                Products.Add(product);
            }
            else
            {
                throw new Exception("Invalid product data");
            }
        }

        public void Remove(string productId)
        {
            Products.RemoveAll(p => p.ProductId == productId);
        }

        public void Update(BaseProduct product)
        {
            int index = Products.FindIndex(p => p.ProductId == product.ProductId);
            if (index != -1 && product.ValidateProduct())
            {
                Products[index] = product;
            }
        }
    }
}
