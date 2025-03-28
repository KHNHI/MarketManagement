using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using MarketManagement.Model;
using System.Security.Cryptography;
using System.Text;
using MarketManagement.UseControl;

namespace MarketManagement.Manager
{
    public class BillManager 
    {
        private readonly FileHandler _productsFileHandler;
        private readonly FileHandler _billsFileHandler;
        private readonly FileHandler _ordersFileHandler;
        private readonly JsonSerializerSettings _jsonSettings;
        private List<BaseProduct> _products;
        private List<Bill> _bills;
        private Bill _bill;
        public List<BillItem> Items;

        public BillItem Item;

        
        public BillManager()
        {
            Items = new List<BillItem>();
            _bill = new Bill();
            _productsFileHandler = new FileHandler("products");
            _billsFileHandler = new FileHandler("bills");
            _ordersFileHandler = new FileHandler("orders");

            _jsonSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Formatting = Formatting.Indented
            };

            LoadData();
        }

        private void LoadData()
        {
            LoadProducts();
            LoadBills();
        }

        private void LoadProducts()
        {
            try
            {
                // Sử dụng FileHandler để tải dữ liệu sản phẩm
                _products = _productsFileHandler.LoadFromFile<List<BaseProduct>>(_jsonSettings) ?? new List<BaseProduct>();
            }
            catch (Exception)
            {
                _products = new List<BaseProduct>();
            }
        }

        private void LoadBills()
        {
            try
            {
                // Sử dụng FileHandler để tải dữ liệu hóa đơn
                _bills = _billsFileHandler.LoadFromFile<List<Bill>>(_jsonSettings) ?? new List<Bill>();
            }
            catch (Exception)
            {
                _bills = new List<Bill>();
            }
        }


        public BillItem GetItemById(string ProductId)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].ProductId.Equals(ProductId, StringComparison.OrdinalIgnoreCase))
                {
                    return Items[i];
                }
            }
            return null;
        }

        public BaseProduct GetProductById(string Id)
        {
            for (int i = 0; i < _products.Count; i++)
            {
                if (_products[i].Id.Equals(Id, StringComparison.OrdinalIgnoreCase))
                {
                    return _products[i];
                }
            }
            return null;
        }

        public void AddItem(BillItem Item)
        {
            Items.Add(Item);
            CalculateTotalCart2();
        }

        public void RemoveItem(BillItem Item)
        {
            Items.Remove(Item);
            CalculateTotalCart2();
        }

       



              public decimal CalculateTotalCart2()
        {
            decimal total = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                total += Items[i].TotalPrice;
            }

            _bill.TotalCart = total;
            return _bill.TotalCart;
        }

        public bool ValidateQuantity(string productId, int quantity)
        {
            var product = GetProductById(productId);
            return product != null && quantity <= product.Quantity;
        }

        
        public Bill CreateNewBill()
        {
            // Tạo đối tượng Bill mới
            Bill newBill = new Bill();
            return newBill;
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
                try
                {
                    // Sử dụng FileHandler để tải dữ liệu đơn hàng
                    ordersData = _ordersFileHandler.LoadFromFile<OrdersData>() ?? new OrdersData { Orders = new List<Order>() };
                    
                    // Ensure Orders is not null
                    if (ordersData.Orders == null)
                    {
                        ordersData.Orders = new List<Order>();
                    }
                }
                catch
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
                    GrandTotal = bill.TotalCart,
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

                // Sử dụng FileHandler để lưu dữ liệu đơn hàng
                _ordersFileHandler.SaveToFile(ordersData, _jsonSettings);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving order: {ex.Message}");
            }
        }

        private void SaveBills()
        {
            try
            {
                // Sử dụng FileHandler để lưu dữ liệu hóa đơn
                _billsFileHandler.SaveToFile(_bills, _jsonSettings);
                _bills.Add(_bill);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving bills: {ex.Message}");
            }
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
            try
            {
                // Sử dụng FileHandler để lưu dữ liệu sản phẩm
                _productsFileHandler.SaveToFile(_products, _jsonSettings);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving products: {ex.Message}");
            }
        }
    }
}