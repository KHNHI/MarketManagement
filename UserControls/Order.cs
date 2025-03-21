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
                LoadInvoicesToGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi làm mới dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadInvoicesToGrid()
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, "Data", "orders.json");

                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Không tìm thấy file orders.json", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    db_ordersDataGridView.DataSource = null;
                    return;
                }

                // Đọc nội dung file
                string jsonData = File.ReadAllText(filePath);

                // Sử dụng JsonDocument để phân tích JSON
                using (JsonDocument doc = JsonDocument.Parse(jsonData))
                {
                    JsonElement root = doc.RootElement;

                    // Kiểm tra nếu root có thuộc tính "Orders"
                    if (root.TryGetProperty("Orders", out JsonElement ordersElement) &&
                        ordersElement.ValueKind == JsonValueKind.Array)
                    {
                        // Tạo DataTable để hiển thị thông tin order (không bao gồm OrderDetails)
                        DataTable dt = new DataTable();

                        // Thêm các cột vào DataTable
                        dt.Columns.Add("InvoiceDate");
                        dt.Columns.Add("InvoiceNo");
                        dt.Columns.Add("CustomerId");
                        dt.Columns.Add("CustomerName");
                        dt.Columns.Add("Contact");
                        dt.Columns.Add("Address");
                        dt.Columns.Add("GrandTotal");

                        // Thêm dữ liệu từ mỗi đơn hàng
                        foreach (JsonElement order in ordersElement.EnumerateArray())
                        {
                            DataRow row = dt.NewRow();

                            if (order.TryGetProperty("InvoiceDate", out JsonElement invoiceDateElement))
                                row["InvoiceDate"] = invoiceDateElement.GetString();

                            if (order.TryGetProperty("InvoiceNo", out JsonElement invoiceNoElement))
                                row["InvoiceNo"] = invoiceNoElement.GetString();

                            if (order.TryGetProperty("CustomerId", out JsonElement customerIdElement))
                                row["CustomerId"] = customerIdElement.GetString();

                            if (order.TryGetProperty("CustomerName", out JsonElement customerNameElement))
                                row["CustomerName"] = customerNameElement.GetString();

                            if (order.TryGetProperty("Contact", out JsonElement contactElement))
                                row["Contact"] = contactElement.GetString();

                            if (order.TryGetProperty("Address", out JsonElement addressElement))
                                row["Address"] = addressElement.GetString();

                            if (order.TryGetProperty("GrandTotal", out JsonElement grandTotalElement))
                                row["GrandTotal"] = grandTotalElement.GetDouble().ToString("N2");

                            dt.Rows.Add(row);
                        }

                        // Gán DataTable làm nguồn dữ liệu cho DataGridView
                        db_ordersDataGridView.DataSource = dt;
                        db_ordersDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
                    }
                    else
                    {
                        MessageBox.Show("File JSON không có cấu trúc Orders mong đợi", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void btn_search_by_invoiceno_Click(object sender, EventArgs e)
        {
            try
            {
                // Đường dẫn tới file JSON
                string filePath = Path.Combine(Application.StartupPath, "Data", "orders.json");
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Không tìm thấy file orders.json", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    db_ordersDataGridView.DataSource = null;
                    return;
                }

                // Đọc nội dung file
                string jsonData = File.ReadAllText(filePath);

                // Sử dụng JsonDocument để phân tích JSON
                using (JsonDocument doc = JsonDocument.Parse(jsonData))
                {
                    JsonElement root = doc.RootElement;

                    // Kiểm tra nếu root có thuộc tính "Orders"
                    if (root.TryGetProperty("Orders", out JsonElement ordersElement) &&
                        ordersElement.ValueKind == JsonValueKind.Array)
                    {
                        // Tạo DataTable để hiển thị thông tin order
                        DataTable dt = new DataTable();

                        // Thêm các cột vào DataTable
                        dt.Columns.Add("InvoiceDate");
                        dt.Columns.Add("InvoiceNo");
                        dt.Columns.Add("CustomerId");
                        dt.Columns.Add("CustomerName");
                        dt.Columns.Add("Contact");
                        dt.Columns.Add("Address");
                        dt.Columns.Add("GrandTotal");

                        // Biến để kiểm tra nếu tìm thấy hóa đơn
                        bool found = false;

                        // Lọc và thêm dữ liệu từ mỗi đơn hàng phù hợp với InvoiceNo
                        foreach (JsonElement order in ordersElement.EnumerateArray())
                        {
                            // Kiểm tra nếu InvoiceNo khớp với giá trị tìm kiếm
                            if (order.TryGetProperty("InvoiceNo", out JsonElement invoiceNoElement) &&
                                invoiceNoElement.GetString() == txt_invoiceno.Text)
                            {
                                found = true;
                                DataRow row = dt.NewRow();

                                if (order.TryGetProperty("InvoiceDate", out JsonElement invoiceDateElement))
                                    row["InvoiceDate"] = invoiceDateElement.GetString();
                                if (order.TryGetProperty("InvoiceNo", out _)) // Đã kiểm tra ở trên
                                    row["InvoiceNo"] = invoiceNoElement.GetString();
                                if (order.TryGetProperty("CustomerId", out JsonElement customerIdElement))
                                    row["CustomerId"] = customerIdElement.GetString();
                                if (order.TryGetProperty("CustomerName", out JsonElement customerNameElement))
                                    row["CustomerName"] = customerNameElement.GetString();
                                if (order.TryGetProperty("Contact", out JsonElement contactElement))
                                    row["Contact"] = contactElement.GetString();
                                if (order.TryGetProperty("Address", out JsonElement addressElement))
                                    row["Address"] = addressElement.GetString();
                                if (order.TryGetProperty("GrandTotal", out JsonElement grandTotalElement))
                                    row["GrandTotal"] = grandTotalElement.GetDouble().ToString("N2");

                                dt.Rows.Add(row);
                            }
                        }

                        // Gán DataTable làm nguồn dữ liệu cho DataGridView
                        db_ordersDataGridView.DataSource = dt;
                        db_ordersDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                        // Hiển thị thông báo nếu không tìm thấy hóa đơn
                        if (!found)
                        {
                            MessageBox.Show("No invoice found with invoice number: " + txt_invoiceno.Text,
                                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("File JSON không có cấu trúc Orders mong đợi", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        private void btn_search_by_customer_Id_Click(object sender, EventArgs e)
        {
            try
            {
                // Đường dẫn tới file JSON
                string filePath = Path.Combine(Application.StartupPath, "Data", "orders.json");
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Không tìm thấy file orders.json", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    db_ordersDataGridView.DataSource = null;
                    return;
                }

                // Đọc nội dung file
                string jsonData = File.ReadAllText(filePath);

                // Sử dụng JsonDocument để phân tích JSON
                using (JsonDocument doc = JsonDocument.Parse(jsonData))
                {
                    JsonElement root = doc.RootElement;

                    // Kiểm tra nếu root có thuộc tính "Orders"
                    if (root.TryGetProperty("Orders", out JsonElement ordersElement) &&
                        ordersElement.ValueKind == JsonValueKind.Array)
                    {
                        // Tạo DataTable để hiển thị thông tin order
                        DataTable dt = new DataTable();

                        // Thêm các cột vào DataTable
                        dt.Columns.Add("InvoiceDate");
                        dt.Columns.Add("InvoiceNo");
                        dt.Columns.Add("CustomerId");
                        dt.Columns.Add("CustomerName");
                        dt.Columns.Add("Contact");
                        dt.Columns.Add("Address");
                        dt.Columns.Add("GrandTotal");

                        // Biến để kiểm tra nếu tìm thấy hóa đơn
                        bool found = false;

                        // Lọc và thêm dữ liệu từ mỗi đơn hàng phù hợp với tên khách hàng
                        foreach (JsonElement order in ordersElement.EnumerateArray())
                        {
                            // Kiểm tra nếu CustomerName chứa giá trị tìm kiếm (không phân biệt hoa thường)
                            if (order.TryGetProperty("CustomerName", out JsonElement customerNameElement) &&
                                customerNameElement.GetString().ToLower().Contains(txt_customerId.Text.ToLower()))
                            {
                                found = true;
                                DataRow row = dt.NewRow();

                                if (order.TryGetProperty("InvoiceDate", out JsonElement invoiceDateElement))
                                    row["InvoiceDate"] = invoiceDateElement.GetString();
                                if (order.TryGetProperty("InvoiceNo", out JsonElement invoiceNoElement))
                                    row["InvoiceNo"] = invoiceNoElement.GetString();
                                if (order.TryGetProperty("CustomerId", out JsonElement customerIdElement))
                                    row["CustomerId"] = customerIdElement.GetString();
                                if (order.TryGetProperty("CustomerName", out _)) // Đã kiểm tra ở trên
                                    row["CustomerName"] = customerNameElement.GetString();
                                if (order.TryGetProperty("Contact", out JsonElement contactElement))
                                    row["Contact"] = contactElement.GetString();
                                if (order.TryGetProperty("Address", out JsonElement addressElement))
                                    row["Address"] = addressElement.GetString();
                                if (order.TryGetProperty("GrandTotal", out JsonElement grandTotalElement))
                                    row["GrandTotal"] = grandTotalElement.GetDouble().ToString("N2");

                                dt.Rows.Add(row);
                            }
                        }

                        // Gán DataTable làm nguồn dữ liệu cho DataGridView
                        db_ordersDataGridView.DataSource = dt;
                        db_ordersDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

                        // Hiển thị thông báo nếu không tìm thấy hóa đơn
                        if (!found)
                        {
                            MessageBox.Show("No invoices found for customer name: " + txt_customerId.Text,
                                "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("File JSON không có cấu trúc Orders mong đợi", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
    }
}
