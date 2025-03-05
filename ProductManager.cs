using MarketManagement.UseControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement
{
    public class ProductManager
    {
        private List<BaseProduct> products;
        private readonly FileHandler fileHandler;

        public ProductManager()
        {
            fileHandler = new FileHandler("products");
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                products = fileHandler.LoadFromFile<List<BaseProduct>>() ?? new List<BaseProduct>();
            }
            catch
            {
                products = new List<BaseProduct>();
            }
        }

        public void AddProduct(BaseProduct product)
        {
            if (products.Any(p => p.ProductId == product.ProductId))
            {
                throw new Exception("Product ID already exists!");
            }
            products.Add(product);
            SaveChanges();
        }

        public void UpdateProduct(BaseProduct product)
        {
            int index = products.FindIndex(p => p.ProductId == product.ProductId);
            if (index != -1)
            {
                products[index] = product;
                SaveChanges();
            }
        }

        public void RemoveProduct(string productId)
        {
            products.RemoveAll(p => p.ProductId == productId);
            SaveChanges();
        }

        public List<BaseProduct> GetAllProducts()
        {
            return products;
        }

        private void SaveChanges()
        {
            fileHandler.SaveToFile(products);
        }
    }
}
