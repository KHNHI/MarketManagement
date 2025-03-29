using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.IO;
using MarketManagement;

namespace MarketManagement.UserControls
{
    public partial class Order : UserControl
    {
        private OrderManager _orderManager;
        private string _ordersJsonPath;

        public Order()
        {
            InitializeComponent();

            // Add event handler for cell double-click
            db_ordersDataGridView.CellDoubleClick += Db_ordersDataGridView_CellDoubleClick;

            // Khởi tạo đường dẫn đến file orders.json
            _ordersJsonPath = Path.Combine(Application.StartupPath, "orders.json");

            // Khởi tạo OrderManager
            _orderManager = new OrderManager(_ordersJsonPath);
            
            // Thêm context menu cho tính năng copy
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("Copy Value");
            copyMenuItem.Click += CopyMenuItem_Click;
            contextMenu.Items.Add(copyMenuItem);
            db_ordersDataGridView.ContextMenuStrip = contextMenu;
            
            // Thêm hỗ trợ phím tắt Ctrl+C
            db_ordersDataGridView.KeyDown += Db_ordersDataGridView_KeyDown;
        }

        private void Db_ordersDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the click was on a valid row (not header, not -1)
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.RowIndex < db_ordersDataGridView.Rows.Count)
            {
                // Lấy giá trị từ cell và copy vào clipboard
                object cellValue = db_ordersDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (cellValue != null)
                {
                    Clipboard.SetText(cellValue.ToString());
                    MessageBox.Show("Copied to clipboard: " + cellValue.ToString(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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
                // Sử dụng OrderManager để lấy dữ liệu
                DataTable dt = _orderManager.GetAllOrders();

                // Cập nhật DataGridView
                db_ordersDataGridView.DataSource = dt;

                // Định dạng cột GrandTotal để hiển thị tiền tệ
                FormatGrandTotalColumn();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                db_ordersDataGridView.DataSource = null;
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show(ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                db_ordersDataGridView.DataSource = null;
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
            try
            {
                // Sử dụng OrderManager để tìm kiếm
                DataTable dt = _orderManager.SearchOrders(fieldName, searchValue);

                // Cập nhật DataGridView
                db_ordersDataGridView.DataSource = dt;
                FormatGrandTotalColumn();

                // Hiển thị thông báo nếu không tìm thấy kết quả
                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show($"No records found with {fieldName}: {searchValue}",
                        "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidDataException ex)
            {
                MessageBox.Show(ex.Message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Error searching for {fieldName}", ex);
            }
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

        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            CopyCellValueToClipboard();
        }

        private void Db_ordersDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            // Kiểm tra phím tắt Ctrl+C để copy
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyCellValueToClipboard();
                e.Handled = true;
            }
        }

        private void CopyCellValueToClipboard()
        {
            try
            {
                // Kiểm tra xem có cell nào được chọn không
                if (db_ordersDataGridView.CurrentCell != null && db_ordersDataGridView.CurrentCell.Value != null)
                {
                    // Copy giá trị vào clipboard
                    Clipboard.SetText(db_ordersDataGridView.CurrentCell.Value.ToString());
                }
                else if (db_ordersDataGridView.SelectedRows.Count > 0)
                {
                    // Nếu chọn cả hàng, lấy giá trị từ cột đầu tiên
                    DataGridViewRow row = db_ordersDataGridView.SelectedRows[0];
                    if (row.Cells[0].Value != null)
                    {
                        Clipboard.SetText(row.Cells[0].Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("Error copying value", ex);
            }
        }
    }
}