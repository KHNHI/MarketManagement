using MarketManagement.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using MarketManagement.UseControl;

namespace MarketManagement.Manager
{
    public class CategoryManager: IManager<Category>
    {
        private List<Category> categories;
        private readonly string filePath;

        public CategoryManager()
        {
            filePath = "categories.json";
            categories = LoadFromFile();
        }

        private List<Category> LoadFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<List<Category>>(jsonData) ?? new List<Category>();
                }
            }
            catch { }
            return new List<Category>();
        }
        private void SaveToFile()
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(categories, Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
            }
            catch { }
        }

        public bool Add(Category category)
        {
            if (categories.Any(c => c.Id == category.Id))
            {
                throw new Exception("Category ID already exists!");
            }
            categories.Add(category);
            SaveToFile();
            return true;
        }

        public bool Update(Category category)
        {
            int index = categories.FindIndex(c => c.Id == category.Id);
            if (index != -1)
            {
                categories[index] = category;
                SaveToFile();
                return true;
            }
            return false;
        }

        public bool Remove(string categoryId)
        {
            categories.RemoveAll(c => c.Id == categoryId);
            SaveToFile();
            return true;
        }

        public Category GetById(string categoryId)
        {
            return categories.FirstOrDefault(c => c.Id == categoryId);
        }

        public List<Category> GetAll()
        {
            return categories;
        }

        
    }
}
