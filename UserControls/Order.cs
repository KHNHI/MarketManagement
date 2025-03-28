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
            
            // Add event handler for cell double-click
            db_ordersDataGridView.CellDoubleClick += Db_ordersDataGridView_CellDoubleClick;
        }

        private void Db_ordersDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the click was on a valid row (not header, not -1)
            if (e.RowIndex >= 0 && e.RowIndex < db_ordersDataGridView.Rows.Count)
            {
                // Process the double-click on a valid row here
                // For example, you could show order details
                DataGridViewRow row = db_ordersDataGridView.Rows[e.RowIndex];
                // Do something with the row data
            }
            // If index is -1 or otherwise invalid, we do nothing, preventing the exception
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








        

        private void db_ordersDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
