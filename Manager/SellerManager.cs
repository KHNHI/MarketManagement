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

        // Singleton instance
        private static SellerManager instance;
        // Lock object for thread safety
        private static readonly object lockObject = new object();
        
        // Singleton accessor with thread safety
        public static SellerManager Instance
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
                            instance = new SellerManager();
                        }
                    }
                }
                return instance;
            }
        }

        // Event đơn giản
        public event EventHandler SellerChanged;

        // Đổi constructor thành private
        private SellerManager()
        {
            filePath = "sellers.json";
            sellers = LoadFromFile();
        }

        // Phương thức để kích hoạt sự kiện
        protected virtual void OnSellerChanged()
        {
            SellerChanged?.Invoke(this, EventArgs.Empty);
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
            for (int i = 0; i < sellers.Count; i++)
            {
                if (sellers[i].Id == seller.Id)
                {
                    throw new Exception("Seller ID already exists!");
                }
            }
            sellers.Add(seller);
            SaveToFile();
            
            // Thông báo sự kiện
            OnSellerChanged();
            return true;
        }

        public bool Update(Seller seller)
        {
            int index = sellers.FindIndex(s => s.Id == seller.Id);
            if (index != -1)
            {
                sellers[index] = seller;
                SaveToFile();
                
                // Thông báo sự kiện
                OnSellerChanged();
                return true;
            }
            return false;
        }

        public bool Remove(string sellerId)
        {
            sellers.RemoveAll(s => s.Id == sellerId);
            SaveToFile();
            
            // Thông báo sự kiện
            OnSellerChanged();
            return true;
        }

        public Seller GetById(string sellerId)
        {
            for (int i = 0; i < sellers.Count; i++)
            {
                if (sellers[i].Id == sellerId)
                {
                    return sellers[i];
                }
            }
            return null;
        }

        public List<Seller> GetAll()
        {
            return sellers;
        }
    }
}