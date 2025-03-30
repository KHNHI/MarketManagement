using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using MarketManagement.Model;
using System.Security.Cryptography;
using System.Text;
using MarketManagement.UseControl;
using System.Windows.Forms;

namespace MarketManagement { 
    public class BillManager : IManager<Bill>
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

        // Singleton instance
        private static BillManager instance;
        // Lock object for thread safety
        private static readonly object lockObject = new object();

        // Singleton accessor with thread safety
        public static BillManager Instance
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
                            instance = new BillManager();
                        }
                    }
                }
                return instance;
            }
        }

        // Event đơn giản
        public event EventHandler BillChanged;

        // Phương thức để kích hoạt sự kiện
        protected virtual void OnBillChanged()
        {
            BillChanged?.Invoke(this, EventArgs.Empty);
        }
        
        // Constructor là private để đảm bảo Singleton
        private BillManager()
        {
            _products= new List<BaseProduct>();
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
            BaseProduct product = GetProductById(productId);
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
                foreach (BillItem item in bill.Items)
                {
                    BaseProduct product = GetProductById(item.ProductId);
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
                
                // Kích hoạt sự kiện BillChanged
                OnBillChanged();
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


        public void UpdateProductQuantity(string productId, int soldQuantity)
        {
            // Sử dụng ProductManager để lấy và cập nhật sản phẩm
            ProductManager productManager = ProductManager.Instance;
            BaseProduct product = productManager.GetById(productId);

            if (product != null)
            {
                if (product.Quantity >= soldQuantity)
                {
                    // Giảm số lượng sản phẩm
                    product.Quantity -= soldQuantity;
                    
                    // Cập nhật sản phẩm thông qua ProductManager
                    productManager.Update(product);
                }
                else
                {
                    throw new Exception($"Not enough quantity for product {product.ProductName}");
                }
            }
            else
            {
                throw new Exception($"Product with ID {productId} not found");
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

      
        public bool RemoveSelectedItem(string productId)
        {
            
            BillItem itemToRemove = GetItemById(productId);
            if (itemToRemove != null)
            {
                // Xóa khỏi danh sách Items
                Items.Remove(itemToRemove);
                
                for (int i = 0; i < _bill.Items.Count; i++)
                {
                    if (_bill.Items[i].ProductId == productId)
                    {
                        _bill.Items.RemoveAt(i);
                        break;
                    }
                }
                CalculateTotalCart2();
                return true;
            }
            return false;
        }
        
        
        public void ClearAllItems()
        {
            Items.Clear();
            _bill.Items.Clear();
            _bill.TotalCart = 0;
        }

        // Triển khai các phương thức từ interface IManager<Bill>
        public bool Add(Bill bill)
        {
            try
            {
                if (bill == null || !bill.Validate())
                    return false;

                // Kiểm tra trùng ID
                for (int i = 0; i < _bills.Count; i++)
                {
                    if (_bills[i].Id == bill.Id)
                        return false;
                }
                
                // Thực hiện lưu hóa đơn với logic hiện tại
                SaveBill(bill);
                
                // Kích hoạt sự kiện
                OnBillChanged();
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Update(Bill bill)
        {
            try
            {
                if (bill == null || !bill.Validate())
                    return false;

                // Tìm index hóa đơn cần update
                int index = -1;
                for (int i = 0; i < _bills.Count; i++)
                {
                    if (_bills[i].Id == bill.Id)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                    return false;

                _bills[index] = bill;
                _billsFileHandler.SaveToFile(_bills, _jsonSettings);
                
                // Kích hoạt sự kiện
                OnBillChanged();
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Remove(string id)
        {
            try
            {
                // Tìm index hóa đơn cần xóa
                int index = -1;
                for (int i = 0; i < _bills.Count; i++)
                {
                    if (_bills[i].Id == id)
                    {
                        index = i;
                        break;
                    }
                }

                if (index == -1)
                    return false;

                _bills.RemoveAt(index);
                _billsFileHandler.SaveToFile(_bills, _jsonSettings);
                
                // Kích hoạt sự kiện
                OnBillChanged();
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Bill GetById(string id)
        {
            for (int i = 0; i < _bills.Count; i++)
            {
                if (_bills[i].Id == id)
                    return _bills[i];
            }
            return null;
        }

        public List<Bill> GetAll()
        {
            return _bills;
        }
    }
}