using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MarketManagement.UserControls
{
    public partial class Order : UserControl
    {
        private string _ordersJsonPath;

        public Order()
        {
            InitializeComponent();

            // Add event handler for cell double-click
            db_ordersDataGridView.CellDoubleClick += Db_ordersDataGridView_CellDoubleClick;

            // Khởi tạo đường dẫn đến file orders.json
            _ordersJsonPath = Path.Combine(Application.StartupPath, "orders.json");
        }

        private void Db_ordersDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the click was on a valid row (not header, not -1)
            if (e.RowIndex >= 0 && e.RowIndex < db_ordersDataGridView.Rows.Count)
            {
                // Process the double-click on a valid row here
                DataGridViewRow row = db_ordersDataGridView.Rows[e.RowIndex];
                // Do something with the row data
            }
        }

        private void UC_orders_Load(object sender, EventArgs e)
        {
            try
            {
                LoadInvoicesToGrid();
                FormatGrandTotalColumn();
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading orders", ex);
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
                ShowErrorMessage("Error refreshing orders data", ex);
            }
        }

        private void LoadInvoicesToGrid()
        {
            try
            {
                // Kiểm tra file có tồn tại không
                if (!File.Exists(_ordersJsonPath))
                {
                    MessageBox.Show("Orders file not found. Checked path: " + _ordersJsonPath,
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    db_ordersDataGridView.DataSource = null;
                    return;
                }

                // Đọc nội dung file
                string jsonData = File.ReadAllText(_ordersJsonPath);

                // Kiểm tra file có dữ liệu không
                if (string.IsNullOrWhiteSpace(jsonData))
                {
                    MessageBox.Show("Orders file is empty", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    db_ordersDataGridView.DataSource = null;
                    return;
                }

                // Tạo DataTable để hiển thị dữ liệu
                DataTable dt = CreateOrdersDataTable();
                bool dataLoaded = LoadOrdersDataToTable(jsonData, dt);

                // Cập nhật DataGridView
                db_ordersDataGridView.DataSource = dt;

                // Định dạng cột GrandTotal để hiển thị tiền tệ
                FormatGrandTotalColumn();

                // Hiển thị thông báo nếu không có dữ liệu
                if (!dataLoaded || dt.Rows.Count == 0)
                {
                    MessageBox.Show("No orders found or data format is not recognized.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error loading invoices", ex);
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
                    MessageBox.Show("Please enter an invoice number to search.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txt_invoiceno.Focus();
                    return;
                }

                string searchText = txt_invoiceno.Text.Trim();
                SearchOrders("InvoiceNo", searchText);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error searching for invoice", ex);
            }
        }

        private void btn_search_by_customer_Id_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nếu không có ID khách hàng nhập vào
                if (string.IsNullOrWhiteSpace(txt_customerId.Text))
                {
                    MessageBox.Show("Please enter a customer ID to search.",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txt_customerId.Focus();
                    return;
                }

                string searchText = txt_customerId.Text.Trim();
                SearchOrders("CustomerId", searchText);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error searching for customer orders", ex);
            }
        }

        private void SearchOrders(string fieldName, string searchValue)
        {
            // Validate file existence
            if (!File.Exists(_ordersJsonPath))
            {
                MessageBox.Show($"Orders file not found. Checked path: {_ordersJsonPath}",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Read and validate JSON content
            string jsonData = File.ReadAllText(_ordersJsonPath);
            if (string.IsNullOrWhiteSpace(jsonData))
            {
                MessageBox.Show("Orders file is empty",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Prepare result table
            DataTable dt = CreateOrdersDataTable();
            bool orderFound = false;

            try
            {
                // Process orders that match the search criteria
                var jsonObject = JObject.Parse(jsonData);
                var ordersArray = jsonObject["Orders"] as JArray;

                if (ordersArray != null)
                {
                    foreach (var orderToken in ordersArray)
                    {
                        string fieldValue = orderToken[fieldName]?.ToString() ?? "";

                        if (!string.IsNullOrEmpty(fieldValue) &&
                            string.Compare(fieldValue, searchValue, true) == 0)
                        {
                            AddOrderToDataTable(orderToken, dt);
                            orderFound = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing JSON: {ex.Message}");
                MessageBox.Show($"Error parsing orders data: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Update UI
            db_ordersDataGridView.DataSource = dt;
            FormatGrandTotalColumn();

            if (!orderFound)
            {
                MessageBox.Show($"No records found with {fieldName}: {searchValue}",
                    "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private DataTable CreateOrdersDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("InvoiceDate", typeof(string));
            dt.Columns.Add("InvoiceNo", typeof(string));
            dt.Columns.Add("CustomerId", typeof(string));
            dt.Columns.Add("CustomerName", typeof(string));
            dt.Columns.Add("Contact", typeof(string));
            dt.Columns.Add("Address", typeof(string));
            dt.Columns.Add("GrandTotal", typeof(decimal));
            return dt;
        }

        private void AddOrderToDataTable(JToken orderToken, DataTable dt)
        {
            DataRow row = dt.NewRow();

            if (orderToken["InvoiceDate"] != null)
                row["InvoiceDate"] = orderToken["InvoiceDate"].ToString();
            else
                row["InvoiceDate"] = "";

            if (orderToken["InvoiceNo"] != null)
                row["InvoiceNo"] = orderToken["InvoiceNo"].ToString();
            else
                row["InvoiceNo"] = "";

            if (orderToken["CustomerId"] != null)
                row["CustomerId"] = orderToken["CustomerId"].ToString();
            else
                row["CustomerId"] = "";

            if (orderToken["CustomerName"] != null)
                row["CustomerName"] = orderToken["CustomerName"].ToString();
            else
                row["CustomerName"] = "";

            if (orderToken["Contact"] != null)
                row["Contact"] = orderToken["Contact"].ToString();
            else
                row["Contact"] = "";

            if (orderToken["Address"] != null)
                row["Address"] = orderToken["Address"].ToString();
            else
                row["Address"] = "";

            decimal grandTotal = 0;
            if (orderToken["GrandTotal"] != null)
            {
                decimal.TryParse(orderToken["GrandTotal"].ToString(), out grandTotal);
            }
            row["GrandTotal"] = grandTotal;

            dt.Rows.Add(row);
        }

        private void FormatGrandTotalColumn()
        {
            if (db_ordersDataGridView.Columns["GrandTotal"] != null)
            {
                db_ordersDataGridView.Columns["GrandTotal"].DefaultCellStyle.Format = "C2";
            }
        }

        private void ShowErrorMessage(string message, Exception ex)
        {
            MessageBox.Show(message + ": " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool LoadOrdersDataToTable(string jsonData, DataTable dt)
        {
            bool dataLoaded = false;

            try
            {
                dynamic fullData = JsonConvert.DeserializeObject(jsonData);
                foreach (JProperty prop in fullData)
                {
                    if (prop.Value is JArray)
                    {
                        JArray array = (JArray)prop.Value;
                        for (int i = 0; i < array.Count; i++)
                        {
                            if (array[i]["InvoiceNo"] != null || array[i]["CustomerName"] != null)
                            {
                                AddOrderToDataTable(array[i], dt);
                                dataLoaded = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading JSON data: " + ex.Message);
            }

            return dataLoaded;
        }
    }
}