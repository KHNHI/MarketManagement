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

        public CustomerManager()
        {
            filePath = "customers.json";
            customers = LoadFromFile();
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
            if (customers.Any(c => c.Id == customer.Id))
            {
                throw new Exception("Customer ID already exists!");
            }
            customers.Add(customer);
            SaveToFile();
            return true;
        }

        public bool Update(BaseCustomer customer)
        {
            int index = customers.FindIndex(c => c.Id == customer.Id);
            if (index != -1)
            {
                customers[index] = customer;
                SaveToFile();
                return true;
            }
            return false;
        }

        public bool Remove(string customerId)
        {
            customers.RemoveAll(c => c.Id == customerId);
            SaveToFile();
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