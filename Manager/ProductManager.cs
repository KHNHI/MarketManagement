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
        private readonly FileHandler fileHandler;
        private readonly JsonSerializerSettings jsonSettings;

        // Singleton instance
        private static ProductManager instance;
        // Lock object for thread safety
        private static readonly object lockObject = new object();

        // Singleton accessor with thread safety
        public static ProductManager Instance
        {
            get
            {
                // First check without locking
                if (instance == null)
                {
                    // Lock for thread safety
                    lock (lockObject)
                    {
                        // Second check inside lock
                        if (instance == null)
                        {
                            instance = new ProductManager();
                        }
                    }
                }
                return instance;
            }
        }

        // Event đơn giản
        public event EventHandler ProductChanged;

        // Constructor là private để đảm bảo Singleton
        private ProductManager()
        {
            fileHandler = new FileHandler("products");
            jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Formatting = Formatting.Indented
            };
            LoadProducts();
        }

        // Phương thức để kích hoạt sự kiện
        protected virtual void OnProductChanged()
        {
            ProductChanged?.Invoke(this, EventArgs.Empty);
        }

        private void LoadProducts()
        {
            try
            {
                // Sử dụng LoadFromFile với JsonSerializerSettings
                products = fileHandler.LoadFromFile<List<BaseProduct>>(jsonSettings) ?? new List<BaseProduct>();
            }
            catch (Exception)
            {
                products = new List<BaseProduct>();
            }
        }

        private void SaveToFile()
        {
            try
            {
                // Sử dụng SaveToFile với JsonSerializerSettings
                fileHandler.SaveToFile(products, jsonSettings);
            }
            catch (Exception) { }
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

            // Thông báo sự kiện
            OnProductChanged();
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

            // Thông báo sự kiện
            OnProductChanged();
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

            // Thông báo sự kiện
            OnProductChanged();
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

        // Factory method để tạo sản phẩm mới theo danh mục
        public static BaseProduct CreateProduct(ProductCategory category)
        {
            BaseProduct product = null;

            switch (category)
            {
                case ProductCategory.Food:
                    product = new FoodProduct();
                    break;
                case ProductCategory.Drink:
                    product = new DrinkProduct();
                    break;
                case ProductCategory.Appliance:
                    product = new ApplianceProduct();
                    break;
                case ProductCategory.Clothes:
                    product = new ClothesProduct();
                    break;
                case ProductCategory.Other:
                    product = new OtherProduct();
                    break;
            }

            return product;
        }
    }
}

