using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace MarketManagement
{
    public class FileHandler
    {
        private readonly string filePath;

        public FileHandler(string fileName)
        {
            filePath = fileName + ".json";
        }

        public void SaveToFile(ProductList productList)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All,
                    Formatting = Formatting.Indented
                };

                string json = JsonConvert.SerializeObject(productList, settings);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving file: " + ex.Message);
            }
        }

        public ProductList LoadFromFile()
        {
            try
            {
                if (!File.Exists(filePath))
                    return new ProductList();

                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };

                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<ProductList>(json, settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading file: " + ex.Message);
            }
        }
    }
   
}

