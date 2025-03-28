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
        public readonly string filePath;

        public FileHandler(string fileName)
        {
            filePath = fileName + ".json";
        }

        public void SaveToFile<T>(T data)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving file: " + ex.Message);
            }
        }

        public void SaveToFile<T>(T data, JsonSerializerSettings settings)
        {
            try
            {
                string jsonString = JsonConvert.SerializeObject(data, settings);
                File.WriteAllText(filePath, jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving file: " + ex.Message);
            }
        }

        public T LoadFromFile<T>() where T : new()
        {
            try
            {
                if (!File.Exists(filePath))
                    return new T();

                string jsonString = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading file: " + ex.Message);
            }
        }

        public T LoadFromFile<T>(JsonSerializerSettings settings) where T : new()
        {
            try
            {
                if (!File.Exists(filePath))
                    return new T();

                string jsonString = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(jsonString, settings);
            }
            catch (Exception ex)
            {
                throw new Exception("Error loading file: " + ex.Message);
            }
        }
    }
}
