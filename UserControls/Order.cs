using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using MarketManagement.Model;
using System.Text.Json;
using Newtonsoft.Json.Linq;

namespace MarketManagement.UserControls
{
    public partial class Order : UserControl
    {
        public Order()
        {
            InitializeComponent();
        }


        // Phương thức tải danh sách hóa đơn
        private void UC_orders_Load(object sender, EventArgs e)
        {
            try
            {
                // Đường dẫn đến file JSON chứa dữ liệu đơn hàng
                string jsonFilePath = Path.Combine(Application.StartupPath, "orders.json");

                // Tạo DataTable để hiển thị dữ liệu
                DataTable dt = new DataTable();
                dt.Columns.Add("InvoiceDate");
                dt.Columns.Add("InvoiceNo");
                dt.Columns.Add("CustomerId");
                dt.Columns.Add("CustomerName");
                dt.Columns.Add("Contact");
                dt.Columns.Add("Address");
                dt.Columns.Add("GrandTotal", typeof(decimal));

                // Kiểm tra xem file có tồn tại không
                if (File.Exists(jsonFilePath))
                {
                    // Đọc nội dung file JSON
                    string jsonData = File.ReadAllText(jsonFilePath);

                    // Chuyển đổi JSON thành đối tượng OrdersData
                    OrdersData ordersData = JsonConvert.DeserializeObject<OrdersData>(jsonData);

                    if (ordersData != null && ordersData.Orders != null)
                    {
                        // Thêm dữ liệu vào DataTable
                        System.Collections.IList list = ordersData.Orders;
                        for (int i = 0; i < list.Count; i++)
                        {
                            MarketManagement.Model.Order order = (MarketManagement.Model.Order)list[i];
                            DataRow row = dt.NewRow();
                            row["InvoiceDate"] = order.InvoiceDate;
                            row["InvoiceNo"] = order.InvoiceNo;
                            row["CustomerId"] = order.CustomerId;
                            row["CustomerName"] = order.CustomerName;
                            row["Contact"] = order.Contact;
                            row["Address"] = order.Address;
                            row["GrandTotal"] = order.GrandTotal;
                            dt.Rows.Add(row);
                        }
                    }
                }

                LoadInvoicesToGrid();


                // Định dạng cột GrandTotal để hiển thị tiền tệ
                if (db_ordersDataGridView.Columns["GrandTotal"] != null)
                {
                    db_ordersDataGridView.Columns["GrandTotal"].DefaultCellStyle.Format = "C2";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading orders: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            try
            {
                // Gọi phương thức LoadInvoicesToGrid để tải lại dữ liệu
                LoadInvoicesToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing orders data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //private void LoadInvoicesToGrid()
        //{
        //    try
        //    {
        //        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "orders.json");


        //        if (!File.Exists(filePath))
        //        {
        //            db_ordersDataGridView.DataSource = null;
        //            return;
        //        }

        //        string jsonData = File.ReadAllText(filePath);
        //        var orders = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonData) ?? new List<Dictionary<string, string>>();


        //        db_ordersDataGridView.DataSource = orders;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error loading invoices: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}



        private void LoadInvoicesToGrid()
        {
            try
            {
                // Thử cả hai đường dẫn có thể
                //string dataFolderPath = Path.Combine(Application.StartupPath, "Data");
                //string filePath1 = Path.Combine(dataFolderPath, "orders.json");
                string filePath2 = Path.Combine(Application.StartupPath, "orders.json");

                string jsonFilePath = "";

                // Kiểm tra file tồn tại ở đường dẫn nào
                //if (File.Exists(filePath1))
                //{
                //    jsonFilePath = filePath1;
                //}
                 if (File.Exists(filePath2))
                {
                    jsonFilePath = filePath2;
                }
                else
                {
                    MessageBox.Show("Orders file not found. Checked paths:\n" +/* filePath1 */ "\n" + filePath2,
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    db_ordersDataGridView.DataSource = null;
                    return;
                }

                // Đọc nội dung file
                string jsonData = File.ReadAllText(jsonFilePath);

                // Kiểm tra file có dữ liệu không
                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    MessageBox.Show("Orders file is empty", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    db_ordersDataGridView.DataSource = null;
                    return;
                }

                // Tạo DataTable để hiển thị dữ liệu
                DataTable dt = new DataTable();
                dt.Columns.Add("InvoiceDate", typeof(string));
                dt.Columns.Add("InvoiceNo", typeof(string));
                dt.Columns.Add("CustomerId", typeof(string));
                dt.Columns.Add("CustomerName", typeof(string));
                dt.Columns.Add("Contact", typeof(string));
                dt.Columns.Add("Address", typeof(string));
                dt.Columns.Add("GrandTotal", typeof(decimal));

                bool dataLoaded = false;

                try
                {
                    // In ra console để debug
                    Console.WriteLine("Loading invoice data from: " + jsonFilePath);
                    Console.WriteLine("JSON Data (first 100 chars): " + jsonData.Substring(0, Math.Min(100, jsonData.Length)));

                    // Sử dụng JObject để đọc dữ liệu - cách này linh hoạt nhất
                    JObject jsonObject = JObject.Parse(jsonData);

                    // Kiểm tra nếu có thuộc tính Orders
                    if (jsonObject["Orders"] != null && jsonObject["Orders"].Type == JTokenType.Array)
                    {
                        JArray ordersArray = (JArray)jsonObject["Orders"];
                        foreach (JToken orderToken in ordersArray)
                        {
                            string invoiceNo = orderToken["InvoiceNo"]?.ToString() ?? "";
                            string customerName = orderToken["CustomerName"]?.ToString() ?? "";

                            // In thông tin debug
                            Console.WriteLine($"Found order: {invoiceNo} - {customerName}");

                            DataRow row = dt.NewRow();
                            row["InvoiceDate"] = orderToken["InvoiceDate"]?.ToString() ?? "";
                            row["InvoiceNo"] = invoiceNo;
                            row["CustomerId"] = orderToken["CustomerId"]?.ToString() ?? "";
                            row["CustomerName"] = customerName;
                            row["Contact"] = orderToken["Contact"]?.ToString() ?? "";
                            row["Address"] = orderToken["Address"]?.ToString() ?? "";

                            decimal grandTotal = 0;
                            if (orderToken["GrandTotal"] != null)
                            {
                                decimal.TryParse(orderToken["GrandTotal"].ToString(), out grandTotal);
                            }
                            row["GrandTotal"] = grandTotal;

                            dt.Rows.Add(row);
                            dataLoaded = true;
                        }
                    }
                    else
                    {
                        // Thử với cách khác nếu không có thuộc tính Orders
                        dynamic dynamicData = JsonConvert.DeserializeObject(jsonData);
                        if (dynamicData != null)
                        {
                            if (dynamicData.Orders != null)
                            {
                                IEnumerable<dynamic> orders = dynamicData.Orders;
                                foreach (dynamic order in orders)
                                {
                                    DataRow row = dt.NewRow();

                                    // Xử lý từng trường dữ liệu
                                    try { row["InvoiceDate"] = order.InvoiceDate?.ToString() ?? ""; } catch { row["InvoiceDate"] = ""; }
                                    try { row["InvoiceNo"] = order.InvoiceNo?.ToString() ?? ""; } catch { row["InvoiceNo"] = ""; }
                                    try { row["CustomerId"] = order.CustomerId?.ToString() ?? ""; } catch { row["CustomerId"] = ""; }
                                    try { row["CustomerName"] = order.CustomerName?.ToString() ?? ""; } catch { row["CustomerName"] = ""; }
                                    try { row["Contact"] = order.Contact?.ToString() ?? ""; } catch { row["Contact"] = ""; }
                                    try { row["Address"] = order.Address?.ToString() ?? ""; } catch { row["Address"] = ""; }

                                    decimal grandTotal = 0;
                                    try
                                    {
                                        string grandTotalStr = order.GrandTotal?.ToString() ?? "0";
                                        decimal.TryParse(grandTotalStr, out grandTotal);
                                    }
                                    catch { grandTotal = 0; }

                                    row["GrandTotal"] = grandTotal;
                                    dt.Rows.Add(row);
                                    dataLoaded = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception parseEx)
                {
                    // Log lỗi
                    Console.WriteLine("Error parsing JSON: " + parseEx.Message);

                    // Thử phương pháp cuối cùng - đọc toàn bộ JSON
                    try
                    {
                        dynamic fullData = JsonConvert.DeserializeObject(jsonData);

                        // Duyệt qua tất cả thuộc tính để tìm một mảng
                        if (fullData != null)
                        {
                            IEnumerable<dynamic> properties = fullData;
                            foreach (dynamic prop in properties)
                            {
                                if (prop is JArray array)
                                {
                                    // Thử đọc từng mảng
                                    foreach (JToken item in array)
                                    {
                                        if (item["InvoiceNo"] != null || item["CustomerName"] != null)
                                        {
                                            DataRow row = dt.NewRow();

                                            try { row["InvoiceDate"] = item["InvoiceDate"]?.ToString() ?? ""; } catch { row["InvoiceDate"] = ""; }
                                            try { row["InvoiceNo"] = item["InvoiceNo"]?.ToString() ?? ""; } catch { row["InvoiceNo"] = ""; }
                                            try { row["CustomerId"] = item["CustomerId"]?.ToString() ?? ""; } catch { row["CustomerId"] = ""; }
                                            try { row["CustomerName"] = item["CustomerName"]?.ToString() ?? ""; } catch { row["CustomerName"] = ""; }
                                            try { row["Contact"] = item["Contact"]?.ToString() ?? ""; } catch { row["Contact"] = ""; }
                                            try { row["Address"] = item["Address"]?.ToString() ?? ""; } catch { row["Address"] = ""; }

                                            decimal grandTotal = 0;
                                            try { decimal.TryParse(item["GrandTotal"]?.ToString() ?? "0", out grandTotal); } catch { }

                                            row["GrandTotal"] = grandTotal;
                                            dt.Rows.Add(row);
                                            dataLoaded = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception lastEx)
                    {
                        // Ghi lại lỗi cuối cùng
                        Console.WriteLine("Final error: " + lastEx.Message);
                    }
                }

                // Cập nhật DataGridView
                db_ordersDataGridView.DataSource = dt;

                // Định dạng cột GrandTotal để hiển thị tiền tệ
                if (db_ordersDataGridView.Columns["GrandTotal"] != null)
                {
                    db_ordersDataGridView.Columns["GrandTotal"].DefaultCellStyle.Format = "C2";
                }

                // Hiển thị thông báo nếu không có dữ liệu
                if (!dataLoaded || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No orders found or data format is not recognized.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading invoices: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                db_ordersDataGridView.DataSource = null;
            }
        }








        private void btn_search_by_invoiceno_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu không có số hóa đơn nhập vào
                if (string.IsNullOrWhiteSpace(txt_invoiceno.Text))
                {
                    MessageBox.Show("Please enter an invoice number to search.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txt_invoiceno.Focus();
                    return;
                }

                // Thử cả hai đường dẫn có thể
                string filePath2 = Path.Combine(Application.StartupPath, "orders.json");

                string jsonFilePath = "";

                // Kiểm tra file tồn tại ở đường dẫn nào
                
                 if (File.Exists(filePath2))
                {
                    jsonFilePath = filePath2;
                }
                else
                {
                    MessageBox.Show("Orders file not found. Checked paths:\n" +  "\n" + filePath2,
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Đọc nội dung file JSON
                string jsonData = File.ReadAllText(jsonFilePath);

                // Kiểm tra nếu file trống
                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    MessageBox.Show("Orders file is empty", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Tạo DataTable để hiển thị kết quả
                DataTable dt = new DataTable();
                dt.Columns.Add("InvoiceDate", typeof(string));
                dt.Columns.Add("InvoiceNo", typeof(string));
                dt.Columns.Add("CustomerId", typeof(string));
                dt.Columns.Add("CustomerName", typeof(string));
                dt.Columns.Add("Contact", typeof(string));
                dt.Columns.Add("Address", typeof(string));
                dt.Columns.Add("GrandTotal", typeof(decimal));

                bool orderFound = false;
                string searchInvoiceNo = txt_invoiceno.Text.Trim();

                // Debug info
                Console.WriteLine("Searching for invoice: " + searchInvoiceNo);
                Console.WriteLine("JSON file path: " + jsonFilePath);

                try
                {
                    // Sử dụng JObject - cách linh hoạt nhất
                    JObject jsonObject = JObject.Parse(jsonData);

                    // Kiểm tra nhiều cấu trúc có thể
                    if (jsonObject["Orders"] != null && jsonObject["Orders"].Type == JTokenType.Array)
                    {
                        JArray ordersArray = (JArray)jsonObject["Orders"];
                        Console.WriteLine("Found Orders array with " + ordersArray.Count + " items");

                        foreach (JToken orderToken in ordersArray)
                        {
                            string invoiceNo = orderToken["InvoiceNo"]?.ToString();
                            Console.WriteLine($"Checking invoice: {invoiceNo} against {searchInvoiceNo}");

                            if (!string.IsNullOrEmpty(invoiceNo) &&
                                invoiceNo.Equals(searchInvoiceNo, StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("Invoice match found!");
                                DataRow row = dt.NewRow();
                                row["InvoiceDate"] = orderToken["InvoiceDate"]?.ToString() ?? "";
                                row["InvoiceNo"] = invoiceNo;
                                row["CustomerId"] = orderToken["CustomerId"]?.ToString() ?? "";
                                row["CustomerName"] = orderToken["CustomerName"]?.ToString() ?? "";
                                row["Contact"] = orderToken["Contact"]?.ToString() ?? "";
                                row["Address"] = orderToken["Address"]?.ToString() ?? "";

                                decimal grandTotal = 0;
                                if (orderToken["GrandTotal"] != null)
                                {
                                    decimal.TryParse(orderToken["GrandTotal"].ToString(), out grandTotal);
                                }
                                row["GrandTotal"] = grandTotal;

                                dt.Rows.Add(row);
                                orderFound = true;
                            }
                        }
                    }
                    else
                    {
                        // Thử xem có mảng nào khác không
                        IEnumerable<JProperty> properties = jsonObject.Properties();
                        foreach (JProperty prop in properties)
                        {
                            Console.WriteLine("Checking property: " + prop.Name);
                            if (prop.Value.Type == JTokenType.Array)
                            {
                                JArray array = (JArray)prop.Value;
                                Console.WriteLine("Found array in property " + prop.Name + " with " + array.Count + " items");

                                foreach (JToken item in array)
                                {
                                    string invoiceNo = item["InvoiceNo"]?.ToString();
                                    if (!string.IsNullOrEmpty(invoiceNo) &&
                                        invoiceNo.Equals(searchInvoiceNo, StringComparison.OrdinalIgnoreCase))
                                    {
                                        DataRow row = dt.NewRow();
                                        row["InvoiceDate"] = item["InvoiceDate"]?.ToString() ?? "";
                                        row["InvoiceNo"] = invoiceNo;
                                        row["CustomerId"] = item["CustomerId"]?.ToString() ?? "";
                                        row["CustomerName"] = item["CustomerName"]?.ToString() ?? "";
                                        row["Contact"] = item["Contact"]?.ToString() ?? "";
                                        row["Address"] = item["Address"]?.ToString() ?? "";

                                        decimal grandTotal = 0;
                                        if (item["GrandTotal"] != null)
                                        {
                                            decimal.TryParse(item["GrandTotal"].ToString(), out grandTotal);
                                        }
                                        row["GrandTotal"] = grandTotal;

                                        dt.Rows.Add(row);
                                        orderFound = true;
                                    }
                                }
                            }
                        }
                    }

                    // Nếu không tìm thấy, thử cách khác
                    if (!orderFound)
                    {
                        dynamic dynamicData = JsonConvert.DeserializeObject(jsonData);
                        if (dynamicData != null && dynamicData.Orders != null)
                        {
                            IEnumerable<dynamic> orders = dynamicData.Orders;
                            foreach (dynamic order in orders)
                            {
                                string invoiceNo = order.InvoiceNo?.ToString();
                                if (!string.IsNullOrEmpty(invoiceNo) &&
                                    invoiceNo.Equals(searchInvoiceNo, StringComparison.OrdinalIgnoreCase))
                                {
                                    DataRow row = dt.NewRow();
                                    row["InvoiceDate"] = order.InvoiceDate?.ToString() ?? "";
                                    row["InvoiceNo"] = invoiceNo;
                                    row["CustomerId"] = order.CustomerId?.ToString() ?? "";
                                    row["CustomerName"] = order.CustomerName?.ToString() ?? "";
                                    row["Contact"] = order.Contact?.ToString() ?? "";
                                    row["Address"] = order.Address?.ToString() ?? "";

                                    decimal grandTotal = 0;
                                    try
                                    {
                                        string grandTotalStr = order.GrandTotal?.ToString() ?? "0";
                                        decimal.TryParse(grandTotalStr, out grandTotal);
                                    }
                                    catch { grandTotal = 0; }

                                    row["GrandTotal"] = grandTotal;
                                    dt.Rows.Add(row);
                                    orderFound = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception parseEx)
                {
                    // Ghi lại lỗi phân tích
                    Console.WriteLine("Error parsing JSON: " + parseEx.Message);

                    // Hiển thị nội dung JSON để debug
                    MessageBox.Show($"Error parsing orders data: {parseEx.Message}\n\nFile content sample: {jsonData.Substring(0, Math.Min(100, jsonData.Length))}...",
                        "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Cập nhật DataGridView với kết quả tìm kiếm
                db_ordersDataGridView.DataSource = dt;

                // Định dạng cột GrandTotal để hiển thị tiền tệ
                if (db_ordersDataGridView.Columns["GrandTotal"] != null)
                {
                    db_ordersDataGridView.Columns["GrandTotal"].DefaultCellStyle.Format = "C2";
                }

                // Hiển thị thông báo nếu không tìm thấy hóa đơn
                if (!orderFound)
                {
                    MessageBox.Show("No invoice found with invoice number: " + searchInvoiceNo,
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching for invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btn_search_by_customer_Id_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu không có ID khách hàng nhập vào
                if (string.IsNullOrWhiteSpace(txt_customerId.Text))
                {
                    MessageBox.Show("Please enter a customer ID to search.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txt_customerId.Focus();
                    return;
                }

                // Thử cả hai đường dẫn có thể
                string filePath2 = Path.Combine(Application.StartupPath, "orders.json");
                string jsonFilePath = "";

                // Kiểm tra file tồn tại ở đường dẫn nào
                if (File.Exists(filePath2))
                {
                    jsonFilePath = filePath2;
                }
                else
                {
                    MessageBox.Show("Orders file not found. Checked paths:\n" + "\n" + filePath2,
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Đọc nội dung file JSON
                string jsonData = File.ReadAllText(jsonFilePath);

                // Kiểm tra nếu file trống
                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    MessageBox.Show("Orders file is empty", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Tạo DataTable để hiển thị kết quả
                DataTable dt = new DataTable();
                dt.Columns.Add("InvoiceDate", typeof(string));
                dt.Columns.Add("InvoiceNo", typeof(string));
                dt.Columns.Add("CustomerId", typeof(string));
                dt.Columns.Add("CustomerName", typeof(string));
                dt.Columns.Add("Contact", typeof(string));
                dt.Columns.Add("Address", typeof(string));
                dt.Columns.Add("GrandTotal", typeof(decimal));

                bool orderFound = false;
                string searchCustomerId = txt_customerId.Text.Trim();

                // Debug info
                Console.WriteLine("Searching for customer ID: " + searchCustomerId);
                Console.WriteLine("JSON file path: " + jsonFilePath);

                try
                {
                    // Sử dụng JObject - cách linh hoạt nhất
                    JObject jsonObject = JObject.Parse(jsonData);

                    // Kiểm tra nhiều cấu trúc có thể
                    if (jsonObject["Orders"] != null && jsonObject["Orders"].Type == JTokenType.Array)
                    {
                        JArray ordersArray = (JArray)jsonObject["Orders"];
                        Console.WriteLine("Found Orders array with " + ordersArray.Count + " items");

                        foreach (JToken orderToken in ordersArray)
                        {
                            string customerId = orderToken["CustomerId"]?.ToString() ?? "";
                            Console.WriteLine($"Checking customer ID: {customerId} against {searchCustomerId}");

                            if (!string.IsNullOrEmpty(customerId) &&
                                customerId.Equals(searchCustomerId, StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("Customer ID match found!");
                                DataRow row = dt.NewRow();
                                row["InvoiceDate"] = orderToken["InvoiceDate"]?.ToString() ?? "";
                                row["InvoiceNo"] = orderToken["InvoiceNo"]?.ToString() ?? "";
                                row["CustomerId"] = customerId;
                                row["CustomerName"] = orderToken["CustomerName"]?.ToString() ?? "";
                                row["Contact"] = orderToken["Contact"]?.ToString() ?? "";
                                row["Address"] = orderToken["Address"]?.ToString() ?? "";

                                decimal grandTotal = 0;
                                if (orderToken["GrandTotal"] != null)
                                {
                                    decimal.TryParse(orderToken["GrandTotal"].ToString(), out grandTotal);
                                }
                                row["GrandTotal"] = grandTotal;

                                dt.Rows.Add(row);
                                orderFound = true;
                            }
                        }
                    }
                    else
                    {
                        // Thử xem có mảng nào khác không
                        IEnumerable<JProperty> properties = jsonObject.Properties();
                        foreach (JProperty prop in properties)
                        {
                            Console.WriteLine("Checking property: " + prop.Name);
                            if (prop.Value.Type == JTokenType.Array)
                            {
                                JArray array = (JArray)prop.Value;
                                Console.WriteLine("Found array in property " + prop.Name + " with " + array.Count + " items");

                                foreach (JToken item in array)
                                {
                                    string customerId = item["CustomerId"]?.ToString() ?? "";
                                    if (!string.IsNullOrEmpty(customerId) &&
                                        customerId.Equals(searchCustomerId, StringComparison.OrdinalIgnoreCase))
                                    {
                                        DataRow row = dt.NewRow();
                                        row["InvoiceDate"] = item["InvoiceDate"]?.ToString() ?? "";
                                        row["InvoiceNo"] = item["InvoiceNo"]?.ToString() ?? "";
                                        row["CustomerId"] = customerId;
                                        row["CustomerName"] = item["CustomerName"]?.ToString() ?? "";
                                        row["Contact"] = item["Contact"]?.ToString() ?? "";
                                        row["Address"] = item["Address"]?.ToString() ?? "";

                                        decimal grandTotal = 0;
                                        if (item["GrandTotal"] != null)
                                        {
                                            decimal.TryParse(item["GrandTotal"].ToString(), out grandTotal);
                                        }
                                        row["GrandTotal"] = grandTotal;

                                        dt.Rows.Add(row);
                                        orderFound = true;
                                    }
                                }
                            }
                        }
                    }

                    // Nếu không tìm thấy, thử cách khác
                    if (!orderFound)
                    {
                        dynamic dynamicData = JsonConvert.DeserializeObject(jsonData);
                        if (dynamicData != null && dynamicData.Orders != null)
                        {
                            IEnumerable<dynamic> orders = dynamicData.Orders;
                            foreach (dynamic order in orders)
                            {
                                string customerId = order.CustomerId?.ToString() ?? "";
                                if (!string.IsNullOrEmpty(customerId) &&
                                    customerId.Equals(searchCustomerId, StringComparison.OrdinalIgnoreCase))
                                {
                                    DataRow row = dt.NewRow();
                                    row["InvoiceDate"] = order.InvoiceDate?.ToString() ?? "";
                                    row["InvoiceNo"] = order.InvoiceNo?.ToString() ?? "";
                                    row["CustomerId"] = customerId;
                                    row["CustomerName"] = order.CustomerName?.ToString() ?? "";
                                    row["Contact"] = order.Contact?.ToString() ?? "";
                                    row["Address"] = order.Address?.ToString() ?? "";

                                    decimal grandTotal = 0;
                                    try
                                    {
                                        string grandTotalStr = order.GrandTotal?.ToString() ?? "0";
                                        decimal.TryParse(grandTotalStr, out grandTotal);
                                    }
                                    catch { grandTotal = 0; }

                                    row["GrandTotal"] = grandTotal;
                                    dt.Rows.Add(row);
                                    orderFound = true;
                                }
                            }
                        }
                    }
                }
                catch (Exception parseEx)
                {
                    // Ghi lại lỗi phân tích
                    Console.WriteLine("Error parsing JSON: " + parseEx.Message);

                    // Hiển thị nội dung JSON để debug
                    MessageBox.Show($"Error parsing orders data: {parseEx.Message}\n\nFile content sample: {jsonData.Substring(0, Math.Min(100, jsonData.Length))}...",
                        "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Cập nhật DataGridView với kết quả tìm kiếm
                db_ordersDataGridView.DataSource = dt;

                // Định dạng cột GrandTotal để hiển thị tiền tệ
                if (db_ordersDataGridView.Columns["GrandTotal"] != null)
                {
                    db_ordersDataGridView.Columns["GrandTotal"].DefaultCellStyle.Format = "C2";
                }

                // Hiển thị thông báo nếu không tìm thấy hóa đơn
                if (!orderFound)
                {
                    MessageBox.Show("No invoices found for customer ID: " + searchCustomerId,
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching for customer orders: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
