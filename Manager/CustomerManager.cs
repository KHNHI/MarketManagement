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
    public class CustomerManager : IManager<Customer>
    {
        private List<Customer> customers;
        private readonly string filePath;

        public CustomerManager()
        {
            filePath = "customers.json";
            customers = LoadFromFile();
        }

        private List<Customer> LoadFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<List<Customer>>(jsonData) ?? new List<Customer>();
                }
            }
            catch { }
            return new List<Customer>();
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

        public bool Add(Customer customer)
        {
            if (customers.Any(c => c.Id == customer.Id))
            {
                throw new Exception("Customer ID already exists!");
            }
            customers.Add(customer);
            SaveToFile();
            return true;
        }

        public bool Update(Customer customer)
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

        public Customer GetById(string customerId)
        {
            return customers.FirstOrDefault(c => c.Id == customerId);
        }

        public List<Customer> GetAll()
        {
            return customers;
        }

        // Các phương thức bổ sung nếu cần
        public List<Customer> GetVIPCustomers()
        {
            List<Customer> vipCustomers = new List<Customer>();
            foreach (Customer customer in customers)
            {
                if (customer.IsVIP)
                {
                    vipCustomers.Add(customer);
                }
            }
            return vipCustomers;
        }

        public Customer GetByPhoneNumber(string phoneNumber)
        {
            foreach (Customer customer in customers)
            {
                if (customer.PhoneNumber == phoneNumber)
                {
                    return customer;
                }
            }
            return null;
        }

        public List<Customer> SearchByName(string searchTerm)
        {
            List<Customer> results = new List<Customer>();
            foreach (Customer customer in customers)
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