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
    public class SellerManager : IManager<Seller>
    {
        private List<Seller> sellers;
        private readonly string filePath;

        public SellerManager()
        {
            filePath = "sellers.json";
            sellers = LoadFromFile();
        }

        private List<Seller> LoadFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<List<Seller>>(jsonData) ?? new List<Seller>();
                }
            }
            catch { }
            return new List<Seller>();
        }

        private void SaveToFile()
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(sellers, Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
            }
            catch { }
        }

        public bool Add(Seller seller)
        {
            if (sellers.Any(s => s.Id == seller.Id))
            {
                throw new Exception("Seller ID already exists!");
            }
            sellers.Add(seller);
            SaveToFile();
            return true;
        }

        public bool Update(Seller seller)
        {
            int index = sellers.FindIndex(s => s.Id == seller.Id);
            if (index != -1)
            {
                sellers[index] = seller;
                SaveToFile();
                return true;
            }
            return false;
        }

        public bool Remove(string sellerId)
        {
            sellers.RemoveAll(s => s.Id == sellerId);
            SaveToFile();
            return true;
        }

        public Seller GetById(string sellerId)
        {
            return sellers.FirstOrDefault(s => s.Id == sellerId);
        }

        public List<Seller> GetAll()
        {
            return sellers;
        }
    }
}