using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement
{
    public class ProductManager
    {
        private ProductList productList;
        private readonly FileHandler fileHandler;

        public ProductManager()
        {
            fileHandler = new FileHandler("products");
            LoadProducts();
        }

        private void LoadProducts()
        {
            productList = fileHandler.LoadFromFile();
        }

        public void AddProduct(BaseProduct product)
        {
            productList.Add(product);
            SaveChanges();
        }

        public void UpdateProduct(BaseProduct product)
        {
            productList.Update(product);
            SaveChanges();
        }

        public void RemoveProduct(string productId)
        {
            productList.Remove(productId);
            SaveChanges();
        }

        public List<BaseProduct> GetAllProducts()
        {
            return productList.Products;
        }

        public BaseProduct GetProductById(string productId)
        {
            return productList.Products.Find(p => p.ProductId == productId);
        }

        private void SaveChanges()
        {
            fileHandler.SaveToFile(productList);
        }
    }
}