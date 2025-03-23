using MarketManagement.Model;
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
                    JsonSerializerSettings settings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All
                    };
                    return JsonConvert.DeserializeObject<List<BaseProduct>>(jsonData, settings) ?? new List<BaseProduct>();
                }
            }
            catch { }
            return new List<BaseProduct>();
        }

        private void SaveToFile()
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented
                };
                string jsonData = JsonConvert.SerializeObject(products, settings);
                File.WriteAllText(filePath, jsonData);
            }
            catch { }
        }

        public bool Add(BaseProduct product)
        {
            if (product == null || !product.Validate())
                return false;

            // Kiểm tra trùng ID
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Id == product.Id)
                    return false;
            }

            products.Add(product);
            SaveToFile();
            return true;
        }

        public bool Update(BaseProduct product)
        {
            if (product == null || !product.Validate())
                return false;

            // Tìm index sản phẩm cần update
            int index = -1;
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Id == product.Id)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                return false;

            products[index] = product;
            SaveToFile();
            return true;
        }

        public bool Remove(string id)
        {
            // Tìm index sản phẩm cần xóa
            int index = -1;
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Id == id)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                return false;

            products.RemoveAt(index);
            SaveToFile();
            return true;
        }

        public BaseProduct GetById(string id)
        {
            // Tìm sản phẩm theo ID
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Id == id)
                    return products[i];
            }
            return null;
        }

        public List<BaseProduct> GetAll()
        {
            return products;
        }
        public List<BaseProduct> GetByCategory(ProductCategory category)
        {
            List<BaseProduct> result = new List<BaseProduct>();

            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Category == category)
                {
                    result.Add(products[i]);
                }
            }

            return result;
        }

        //    // Factory method để tạo sản phẩm mới theo danh mục
        //    public static BaseProduct CreateProduct(ProductCategory category)
        //    {
        //        BaseProduct product = null;

        //        switch (category)
        //        {
        //            case ProductCategory.Food:
        //                product = new FoodProduct();
        //                break;
        //            case ProductCategory.Drink:
        //                product = new DrinkProduct();
        //                break;
        //            case ProductCategory.Appliance:
        //                product = new ApplianceProduct();
        //                break;
        //            case ProductCategory.Clothes:
        //                product = new ClothesProduct();
        //                break;
        //            case ProductCategory.Other:
        //                product = new OtherProduct();
        //                break;
        //        }

        //        return product;
        //    }
    }
}
