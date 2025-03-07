using MarketManagement.UseControl;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement
{
    //public class ProductManager
    //{
    //    private List<BaseProduct> products;
    //    private readonly FileHandler fileHandler;

    //    public ProductManager()
    //    {
    //        fileHandler = new FileHandler("products");
    //        LoadProducts();
    //    }

    //    private void LoadProducts()
    //    {
    //        try
    //        {
    //            products = fileHandler.LoadFromFile<List<BaseProduct>>() ?? new List<BaseProduct>();
    //        }
    //        catch
    //        {
    //            products = new List<BaseProduct>();
    //        }
    //    }

    //    public void AddProduct(BaseProduct product)
    //    {
    //        if (products.Any(p => p.ProductId == product.ProductId))
    //        {
    //            throw new Exception("Product ID already exists!");
    //        }
    //        products.Add(product);
    //        SaveChanges();
    //    }

    //    public void UpdateProduct(BaseProduct product)
    //    {
    //        int index = products.FindIndex(p => p.ProductId == product.ProductId);
    //        if (index != -1)
    //        {
    //            products[index] = product;
    //            SaveChanges();
    //        }
    //    }

    //    public void RemoveProduct(string productId)
    //    {
    //        products.RemoveAll(p => p.ProductId == productId);
    //        SaveChanges();
    //    }

    //    public List<BaseProduct> GetAllProducts()
    //    {
    //        return products;
    //    }

    //    private void SaveChanges()
    //    {
    //        fileHandler.SaveToFile(products);
    //    }
    //}
    public class ProductManager : IManager<BaseProduct>
    {
        private List<BaseProduct> products;
        private readonly string filePath;

        public ProductManager()
        {
            filePath = "products.json";
            products = LoadFromFile();
        }

        private List<BaseProduct> LoadFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<List<BaseProduct>>(jsonData) ?? new List<BaseProduct>();
                }
            }
            catch { }
            return new List<BaseProduct>();
        }

        private void SaveToFile()
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(products, Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
            }
            catch { }
        }

        public bool Add(BaseProduct product)
        {
            if (product == null || !product.Validate())
                return false;

            if (products.Any(p => p.Id == product.Id))
                return false;

            products.Add(product);
            SaveToFile();
            return true;
        }

        public bool Update(BaseProduct product)
        {
            if (product == null || !product.Validate())
                return false;

            int index = products.FindIndex(p => p.Id == product.Id);
            if (index == -1)
                return false;

            products[index] = product;
            SaveToFile();
            return true;
        }

        public bool Remove(string id)
        {
            int index = products.FindIndex(p => p.Id == id);
            if (index == -1)
                return false;

            products.RemoveAt(index);
            SaveToFile();
            return true;
        }

        public BaseProduct GetById(string id)
        {
            return products.FirstOrDefault(p => p.Id == id);
        }

        public List<BaseProduct> GetAll()
        {
            return products;
        }
    }
}
