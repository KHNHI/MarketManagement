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
    public class CustomerManager : IManager<BaseCustomer>
    {
        private List<BaseCustomer> customers;
        private readonly string filePath;

        // Singleton instance
        private static CustomerManager instance;
        // Lock object for thread safety
        private static readonly object lockObject = new object();
        
        // Singleton accessor with thread safety
        public static CustomerManager Instance
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
                            instance = new CustomerManager();
                        }
                    }
                }
                return instance;
            }
        }

        // Event đơn giản
        public event EventHandler CustomerChanged;

        // Đổi constructor thành private
        private CustomerManager()
        {
            filePath = "customers.json";
            customers = LoadFromFile();
        }

        // Phương thức để kích hoạt sự kiện
        protected virtual void OnCustomerChanged()
        {
            CustomerChanged?.Invoke(this, EventArgs.Empty);
        }

        private List<BaseCustomer> LoadFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<List<BaseCustomer>>(jsonData) ?? new List<BaseCustomer>();
                }
            }
            catch { }
            return new List<BaseCustomer>();
        }

        private void SaveToFile()
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(customers, Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
            }
            catch { }
        }

        public bool Add(BaseCustomer customer)
        {
            if (customer == null || !customer.Validate())
                return false;

            customers.Add(customer);
            SaveToFile();
            
            // Thông báo sự kiện
            OnCustomerChanged();
            return true;
        }

        public bool Update(BaseCustomer customer)
        {
            if (customer == null || !customer.Validate())
                return false;

            // Tìm index khách hàng cần update
            int index = -1;
            for (int i = 0; i < customers.Count; i++)
            {
                if (customers[i].Id == customer.Id)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                return false;

            customers[index] = customer;
            SaveToFile();
            
            // Thông báo sự kiện
            OnCustomerChanged();
            return true;
        }

        public bool Remove(string id)
        {
            // Tìm index khách hàng cần xóa
            int index = -1;
            for (int i = 0; i < customers.Count; i++)
            {
                if (customers[i].Id == id)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                return false;

            customers.RemoveAt(index);
            SaveToFile();
            
            // Thông báo sự kiện
            OnCustomerChanged();
            return true;
        }

        public BaseCustomer GetById(string customerId)
        {
            return customers.FirstOrDefault(c => c.Id == customerId);
        }

        public List<BaseCustomer> GetAll()
        {
            return customers;
        }

        // Các phương thức bổ sung nếu cần
        public List<BaseCustomer> GetVIPCustomers()
        {
            List<BaseCustomer> vipCustomers = new List<BaseCustomer>();
            foreach (BaseCustomer customer in customers)
            {
                if (customer.IsVIP)
                {
                    vipCustomers.Add(customer);
                }
            }
            return vipCustomers;
        }

        public BaseCustomer GetByPhoneNumber(string phoneNumber)
        {
            foreach (BaseCustomer customer in customers)
            {
                if (customer.PhoneNumber == phoneNumber)
                {
                    return customer;
                }
            }
            return null;
        }

        public List<BaseCustomer> SearchByName(string searchTerm)
        {
            List<BaseCustomer> results = new List<BaseCustomer>();
            foreach (BaseCustomer customer in customers)
            {
                if (customer.CustomerName != null &&
                    customer.CustomerName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    results.Add(customer);
                }
            }
            return results;
        }
    }
}