using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using MarketManagement.Model;

namespace MarketManagement.Manager
{
    public class BillManager
    {
        private readonly string _productsFilePath;
        private readonly string _billsFilePath;
        private readonly string _ordersFilePath;
        private List<BaseProduct> _products;
        private List<Bill> _bills;

        public BillManager()
        {
            _productsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "products.json");
            _billsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bills.json");
            _ordersFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "orders.json");
            LoadData();
        }

        private void LoadData()
        {
            LoadProducts();
            LoadBills();
        }

        private void LoadProducts()
        {
            if (File.Exists(_productsFilePath))
            {
                string jsonData = File.ReadAllText(_productsFilePath);
                _products = JsonConvert.DeserializeObject<List<BaseProduct>>(jsonData, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                }) ?? new List<BaseProduct>();
            }
            else
            {
                _products = new List<BaseProduct>();
            }
        }

        private void LoadBills()
        {
            if (File.Exists(_billsFilePath))
            {
                string jsonData = File.ReadAllText(_billsFilePath);
                _bills = JsonConvert.DeserializeObject<List<Bill>>(jsonData) ?? new List<Bill>();
            }
            else
            {
                _bills = new List<Bill>();
            }
        }

        public string GenerateBillId()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public BaseProduct GetProductById(string productId)
        {
            for (int i = 0; i < _products.Count; i++)
            {
                if (_products[i].Id.Equals(productId, StringComparison.OrdinalIgnoreCase))
                {
                    return _products[i];
                }
            }
            return null;
        }

        public BaseProduct GetProductByName(string productName)
        {
            for (int i = 0; i < _products.Count; i++)
            {
                if (_products[i].ProductName != null && 
                    _products[i].ProductName.Equals(productName, StringComparison.OrdinalIgnoreCase))
                {
                    return _products[i];
                }
            }
            return null;
        }

        public List<BaseProduct> SearchProductsByName(string searchTerm)
        {
            List<BaseProduct> results = new List<BaseProduct>();
            for (int i = 0; i < _products.Count; i++)
            {
                if (_products[i].ProductName != null && 
                    _products[i].ProductName.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    results.Add(_products[i]);
                }
            }
            return results;
        }

        public decimal CalculateTotalPrice(string productName, int quantity)
        {
            var product = GetProductByName(productName);
            if (product == null) return 0;
            return product.Price * quantity;
        }

        public bool ValidateQuantity(string productName, int quantity)
        {
            var product = GetProductByName(productName);
            return product != null && quantity <= product.Quantity;
        }

        public Bill CreateNewBill()
        {
            return new Bill
            {
                BillId = DateTime.Now.ToString("yyyyMMddHHmmss"),
                Date = DateTime.Now,
                Items = new List<BillItem>()
            };
        }

        public void AddItemToBill(Bill bill, string productId, string productName, int quantity, decimal unitPrice)
        {
            var item = new BillItem(productId, productName, quantity, unitPrice);
            bill.AddItem(item);
        }

        public void SaveBill(Bill bill)
        {
            try
            {
                // Cập nhật số lượng sản phẩm
                foreach (var item in bill.Items)
                {
                    var product = GetProductById(item.ProductId);
                    if (product != null)
                    {
                        if (product.Quantity >= item.Quantity)
                        {
                            product.Quantity -= item.Quantity;
                        }
                        else
                        {
                            throw new Exception($"Không đủ số lượng sản phẩm {product.ProductName} trong kho");
                        }
                    }
                }

                // Lưu cập nhật số lượng sản phẩm
                SaveProducts();

                // Lưu hóa đơn
                _bills.Add(bill);
                SaveBills();

                // Lưu vào orders.json
                SaveOrder(bill);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lưu hóa đơn: {ex.Message}");
            }
        }

        private void SaveOrder(Bill bill)
        {
            try
            {
                OrdersData ordersData;
                if (File.Exists(_ordersFilePath))
                {
                    string jsonData = File.ReadAllText(_ordersFilePath);
                    ordersData = JsonConvert.DeserializeObject<OrdersData>(jsonData) ?? new OrdersData { Orders = new List<Order>() };
                }
                else
                {
                    ordersData = new OrdersData { Orders = new List<Order>() };
                }

                Order order = new Order
                {
                    InvoiceNo = bill.BillId,
                    InvoiceDate = bill.Date.ToString("dd/MM/yyyy HH:mm:ss"),
                    CustomerId = bill.CustomerId,
                    CustomerName = bill.CustomerName,
                    Contact = bill.Contact,
                    Address = bill.Address,
                    GrandTotal = bill.TotalAmount,
                    OrderDetails = new List<OrderDetail>()
                };

                // Chuyển đổi từ BillItem sang OrderDetail
                for (int i = 0; i < bill.Items.Count; i++)
                {
                    BillItem item = bill.Items[i];
                    OrderDetail detail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        Price = item.UnitPrice,
                        Total = item.TotalPrice
                    };
                    order.OrderDetails.Add(detail);
                }

                ordersData.Orders.Add(order);

                string updatedJsonData = JsonConvert.SerializeObject(ordersData, Formatting.Indented);
                File.WriteAllText(_ordersFilePath, updatedJsonData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving order: {ex.Message}");
            }
        }

        private void SaveBills()
        {
            string jsonData = JsonConvert.SerializeObject(_bills, Formatting.Indented);
            File.WriteAllText(_billsFilePath, jsonData);
        }

        public List<Bill> GetAllBills()
        {
            return _bills;
        }

        public Bill GetBillById(string billId)
        {
            for (int i = 0; i < _bills.Count; i++)
            {
                if (_bills[i].BillId == billId)
                {
                    return _bills[i];
                }
            }
            return null;
        }

        public void UpdateProductQuantity(string productId, int soldQuantity)
        {
            BaseProduct product = null;
            for (int i = 0; i < _products.Count; i++)
            {
                if (_products[i].Id == productId)
                {
                    product = _products[i];
                    break;
                }
            }

            if (product != null)
            {
                if (product.Quantity >= soldQuantity)
                {
                    product.Quantity -= soldQuantity;
                    SaveProducts();
                }
                else
                {
                    throw new Exception($"Not enough quantity for product {product.ProductName}");
                }
            }
        }

        private void SaveProducts()
        {
            string jsonData = JsonConvert.SerializeObject(_products, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            });
            File.WriteAllText(_productsFilePath, jsonData);
        }
    }
} 