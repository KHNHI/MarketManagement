using MarketManagement.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using MarketManagement.UseControl;
using System.Windows.Forms;

namespace MarketManagement.Manager
{
    public class CustomerManager : IManager<BaseCustomer>
    {
        private List<BaseCustomer> customers;
        private readonly FileHandler fileHandler;
        private readonly JsonSerializerSettings jsonSettings;

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
            fileHandler = new FileHandler("customers");
            jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };
            LoadCustomers();
        }

        // Phương thức để kích hoạt sự kiện
        protected virtual void OnCustomerChanged()
        {
            CustomerChanged?.Invoke(this, EventArgs.Empty);
        }

        private void LoadCustomers()
        {
            try
            {
                // Sử dụng FileHandler để load dữ liệu
                customers = fileHandler.LoadFromFile<List<BaseCustomer>>(jsonSettings) ?? new List<BaseCustomer>();
            }
            catch (Exception)
            {
                customers = new List<BaseCustomer>();
            }
        }

        private void SaveToFile()
        {
            try
            {
                // Sử dụng FileHandler để lưu dữ liệu
                fileHandler.SaveToFile(customers, jsonSettings);
            }
            catch (Exception) { }
        }

        private bool IsPhoneNumberExists(string phoneNumber, string excludeCustomerId = null)
        {
            for (int i = 0; i < customers.Count; i++)
            {
                if (customers[i].PhoneNumber == phoneNumber && customers[i].Id != excludeCustomerId)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Add(BaseCustomer customer)
        {
            if (customer == null)
                return false;

            var validationResult = customer.ValidateWithDetails() as CustomerValidationResult;
            if (validationResult == null)
                return false;

            // Kiểm tra số điện thoại trùng
            if (IsPhoneNumberExists(customer.PhoneNumber))
            {
                validationResult.AddPhoneNumberDuplicationError(customer.PhoneNumber);
            }

            if (!validationResult.IsValid)
            {
                MessageBox.Show(validationResult.GetErrorMessage(), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            customers.Add(customer);
            SaveToFile();

            // Thông báo sự kiện
            OnCustomerChanged();
            return true;
        }

        public bool Update(BaseCustomer customer)
        {
            if (customer == null)
                return false;

            var validationResult = customer.ValidateWithDetails() as CustomerValidationResult;
            if (validationResult == null)
                return false;

            // Kiểm tra số điện thoại trùng, loại trừ chính customer hiện tại
            if (IsPhoneNumberExists(customer.PhoneNumber, customer.Id))
            {
                validationResult.AddPhoneNumberDuplicationError(customer.PhoneNumber);
            }

            if (!validationResult.IsValid)
            {
                MessageBox.Show(validationResult.GetErrorMessage(), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

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
            if (string.IsNullOrWhiteSpace(customerId))
                return null;

            for (int i = 0; i < customers.Count; i++)
            {
                if (customers[i].Id.Equals(customerId, StringComparison.OrdinalIgnoreCase))
                {
                    return customers[i];
                }
            }
            return null;
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


    }
}