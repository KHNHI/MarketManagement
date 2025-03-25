using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using MarketManagement.UseControl;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using MarketManagement.Model;
using System.Text.Json;
using Microsoft.VisualBasic;
using MarketManagement.Manager;

namespace MarketManagement.UserControls
{
    public partial class Billing : System.Windows.Forms.UserControl
    {
        public static string invoiceid;
        private readonly BillManager _billManager;
        private readonly CustomerManager _customerManager;
        private Bill _currentBill;

        public Billing()
        {
            InitializeComponent();
            _billManager = new BillManager();
            _customerManager =CustomerManager.Instance;

            _currentBill = _billManager.CreateNewBill();

            // Đăng ký các sự kiện để cập nhật tổng giá
            db_dataGridView1.CellValueChanged += db_dataGridView1_CellValueChanged;
            db_dataGridView1.UserDeletedRow += db_dataGridView1_UserDeletedRow;
        }

        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = "123456789".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        private void auto()
        {
            txt_invoiceno.Text = "" + GetUniqueKey(5);
        }

        private void UC_Billing_Load(object sender, EventArgs e)
        {
            auto();
            _currentBill = _billManager.CreateNewBill();

            // Đăng ký các sự kiện TextChanged
            if (!EventExists(txt_productId, "TextChanged", "txt_productId_TextChanged"))
                txt_productId.TextChanged += txt_productId_TextChanged;

            if (!EventExists(txt_productname, "TextChanged", "txt_productname_TextChanged"))
                txt_productname.TextChanged += txt_productname_TextChanged;

            if (!EventExists(txt_productquantity, "TextChanged", "txt_productquantity_TextChanged"))
                txt_productquantity.TextChanged += txt_productquantity_TextChanged;

            if (!EventExists(txt_customerid, "TextChanged", "txt_customerid_TextChanged"))
                txt_customerid.TextChanged += txt_customerid_TextChanged;

         
        }

      

        private void txt_customerid_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txt_customerid.Text))
            {
                var customer = _customerManager.GetById(txt_customerid.Text);
                if (customer != null)
                {
                    txt_customername.Text = customer.CustomerName;
                    txt_contact.Text = customer.PhoneNumber;
                    txt_address.Text = customer.Address;
                }
                else
                {
                    ClearCustomerFields();
                }
            }
            else
            {
                ClearCustomerFields();
            }
        }

        private void ClearCustomerFields()
        {
            txt_customername.Clear();
            txt_contact.Clear();
            txt_address.Text = "";
        }

        private void LoadCustomerById(string customerId)
        {
            try
            {
                // Nếu không có ID khách hàng, xóa thông tin liên quan
                if (string.IsNullOrWhiteSpace(customerId))
                {
                    ClearCustomerFields();
                    return;
                }

                // Đường dẫn tuyệt đối đến file JSON
                string jsonFilePath = Path.Combine(Application.StartupPath, "customers.json");

                // Kiểm tra file tồn tại
                if (!File.Exists(jsonFilePath))
                {
                    MessageBox.Show($"Customer data file not found at: {jsonFilePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Đọc nội dung file JSON
                string jsonData = File.ReadAllText(jsonFilePath);

                // Deserialize JSON thành danh sách BaseCustomer
                List<BaseCustomer> customers = JsonConvert.DeserializeObject<List<BaseCustomer>>(jsonData, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Auto
                });

                if (customers != null)
                {
                    BaseCustomer customer = null;
                    for (int i = 0; i < customers.Count; i++)
                    {
                        if (customers[i].Id.Equals(customerId, StringComparison.OrdinalIgnoreCase))
                        {
                            customer = customers[i];
                            break;
                        }
                    }

                    if (customer != null)
                    {
                        // Cập nhật thông tin vào form
                        txt_customername.Text = customer.CustomerName;
                        txt_contact.Text = customer.PhoneNumber;
                        txt_address.Text = customer.Address;
                        return;
                    }
                }

                // Không tìm thấy khách hàng với ID này
                MessageBox.Show($"Customer with ID '{customerId}' not found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearCustomerFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearCustomerFields();
            }
        }

        private void CalculateCartTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in db_dataGridView1.Rows)
            {
                if (!row.IsNewRow && row.Cells["TotalPrice"].Value != null)
                {
                    if (decimal.TryParse(row.Cells["TotalPrice"].Value.ToString(), out decimal rowTotal))
                    {
                        total += rowTotal;
                    }
                }
            }
            txt_totalprice.Text = total.ToString("N0");
        }

        private void ClearProductFields()
        {
            txt_productId.Clear();
            txt_productname.Clear();
            txt_productprice.Clear();
            txt_productquantity.Clear();
            txt_totalprice.Clear();
        }

        private void txt_productquantity_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txt_productquantity.Text))
                {
                    txt_totalprice.Clear();
                }
                else if (string.IsNullOrEmpty(txt_productname.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txt_productname.Focus();
                }
                else
                {
                    if (!int.TryParse(txt_productquantity.Text, out int quantity))
                    {
                        MessageBox.Show("Vui lòng nhập số lượng hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txt_productquantity.Clear();
                        return;
                    }

                    var product = _billManager.GetProductByName(txt_productname.Text);
                        if (product != null)
                        {
                        if (!_billManager.ValidateQuantity(txt_productname.Text, quantity))
                        {
                            MessageBox.Show($"Số lượng sản phẩm trong kho chỉ còn {product.Quantity}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txt_productquantity.Text = product.Quantity.ToString();
                            quantity = product.Quantity;
                            }

                        decimal totalPrice = _billManager.CalculateTotalPrice(txt_productname.Text, quantity);
                        txt_totalprice.Text = totalPrice.ToString("N0");
                        }
                        else
                        {
                        MessageBox.Show("Không tìm thấy sản phẩm trong cơ sở dữ liệu", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            txt_totalprice.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tính toán giá: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_addtocard_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txt_productname.Text) ||
                    string.IsNullOrEmpty(txt_productquantity.Text) ||
                    string.IsNullOrEmpty(txt_totalprice.Text))
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin sản phẩm", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var product = _billManager.GetProductByName(txt_productname.Text);
                if (product != null)
                {
                    if (int.TryParse(txt_productquantity.Text, out int quantity))
                    {
                        _billManager.AddItemToBill(_currentBill, product.Id, product.ProductName, quantity, product.Price);
                        UpdateDataGridView();
                        ClearInputFields();
                        }
                    }
                }
                catch (Exception ex)
                {
                MessageBox.Show("Lỗi thêm sản phẩm vào giỏ hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateDataGridView()
        {
            try
            {
                // Tạo DataTable mới với đầy đủ các cột
                DataTable dt = new DataTable();
                dt.Columns.Add("ProductName", typeof(string));
                dt.Columns.Add("Quantity", typeof(int));
                dt.Columns.Add("Price", typeof(decimal));
                dt.Columns.Add("Description", typeof(string));
                dt.Columns.Add("Category", typeof(string));
                dt.Columns.Add("Id", typeof(string));

                // Thêm dữ liệu từ _currentBill vào DataTable
                foreach (var item in _currentBill.Items)
                {
                    var product = _billManager.GetProductById(item.ProductId);
                    if (product != null)
                    {
                        dt.Rows.Add(
                            product.ProductName,
                            item.Quantity,
                            product.Price,
                            product.Description,
                            product.Category,
                            product.Id
                        );
                    }
                }

                // Gán DataTable làm DataSource cho DataGridView
                db_dataGridView1.DataSource = null;
                db_dataGridView1.DataSource = dt;

                // Đặt tên hiển thị cho các cột
                if (db_dataGridView1.Columns["ProductName"] != null)
                    db_dataGridView1.Columns["ProductName"].HeaderText = "Tên sản phẩm";
                if (db_dataGridView1.Columns["Quantity"] != null)
                    db_dataGridView1.Columns["Quantity"].HeaderText = "Số lượng";
                if (db_dataGridView1.Columns["Price"] != null)
                    db_dataGridView1.Columns["Price"].HeaderText = "Đơn giá";
                if (db_dataGridView1.Columns["Description"] != null)
                    db_dataGridView1.Columns["Description"].HeaderText = "Mô tả";
                if (db_dataGridView1.Columns["Category"] != null)
                    db_dataGridView1.Columns["Category"].HeaderText = "Danh mục";
                if (db_dataGridView1.Columns["Id"] != null)
                    db_dataGridView1.Columns["Id"].HeaderText = "Mã SP";

                // Định dạng số tiền
                if (db_dataGridView1.Columns["Price"] != null)
                    db_dataGridView1.Columns["Price"].DefaultCellStyle.Format = "N0";

                // Tính tổng tiền
                decimal total = _currentBill.Items.Sum(item => item.TotalPrice);
                txt_totalprice.Text = total.ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating grid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearInputFields()
        {
            txt_productId.Clear();
            txt_productname.Clear();
            txt_productquantity.Clear();
            txt_totalprice.Clear();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                if (_currentBill.Items.Count == 0)
                {
                    MessageBox.Show("Vui lòng thêm sản phẩm vào hóa đơn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Cập nhật thông tin khách hàng vào hóa đơn
                _currentBill.CustomerName = txt_customername.Text;
                _currentBill.CustomerId = txt_customerid.Text;
                _currentBill.Contact = txt_contact.Text;
                _currentBill.Address = txt_address.Text;

                foreach (var item in _currentBill.Items)
                {
                    _billManager.UpdateProductQuantity(item.ProductId, item.Quantity);
                }

                _billManager.SaveBill(_currentBill);
                MessageBox.Show("Lưu hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tạo hóa đơn mới
                _currentBill = _billManager.CreateNewBill();
                txt_invoiceno.Text = _currentBill.BillId;
                UpdateDataGridView();
                ClearInputFields();
                ClearCustomerFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void db_dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            if (e.Row.DataBoundItem is BillItem item)
            {
                _currentBill.RemoveItem(item);
                UpdateDataGridView();
            }
        }

        private bool EventExists(Control control, string eventName, string handlerName)
        {
            var eventField = typeof(Control).GetField(eventName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (eventField != null)
            {
                var eventValue = eventField.GetValue(control);
                if (eventValue != null && eventValue.ToString().Contains(handlerName))
                    return true;
            }

            return false;
        }

        private void txt_productId_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(txt_productId.Text))
                {
                    var product = _billManager.GetProductById(txt_productId.Text);
                        if (product != null)
                        {
                            txt_productname.Text = product.ProductName;
                            txt_productprice.Text = product.Price.ToString();

                            if (product.Quantity > 0)
                                txt_productquantity.Text = "1";
                            else
                                txt_productquantity.Text = "0";

                            decimal totalValue = product.Price;
                            txt_totalprice.Text = totalValue.ToString("0.00");
                        }
                        else if (txt_productId.Text.Length >= 3)
                        {
                        ClearProductFields();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in txt_productId_TextChanged: " + ex.Message);
            }
        }

        private void txt_productname_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txt_productId.Text) && txt_productname.Text.Length >= 3)
            {
                var products = _billManager.SearchProductsByName(txt_productname.Text);
                if (products.Count > 0)
                {
                    var product = products[0]; // Lấy sản phẩm đầu tiên tìm thấy
                        txt_productId.Text = product.Id;
                        txt_productname.Text = product.ProductName;
                        txt_productprice.Text = product.Price.ToString();

                        if (product.Quantity > 0)
                            txt_productquantity.Text = "1";
                        else
                            txt_productquantity.Text = "0";

                        decimal totalValue = product.Price;
                        txt_totalprice.Text = totalValue.ToString("0.00");
                    }
            }
        }

        private void btn_remove_selected_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem có hàng nào được chọn trong DataGridView không
                if (db_dataGridView1.SelectedRows.Count == 0 && db_dataGridView1.SelectedCells.Count == 0)
                {
                    MessageBox.Show("Please, select a product to remove.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lấy hàng được chọn (từ selected row hoặc selected cell)
                DataGridViewRow selectedRow;
                if (db_dataGridView1.SelectedRows.Count > 0)
                {
                    selectedRow = db_dataGridView1.SelectedRows[0];
                }
                else
                {
                    int rowIndex = db_dataGridView1.SelectedCells[0].RowIndex;
                    selectedRow = db_dataGridView1.Rows[rowIndex];
                }

                // Kiểm tra xem hàng được chọn có phải là hàng mới không
                if (selectedRow.IsNewRow)
                {
                    MessageBox.Show("Cannot remove the new row.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Xóa hàng được chọn
                db_dataGridView1.Rows.Remove(selectedRow);

                // Cập nhật tổng giá
                CalculateCartTotal();

                // Xóa thông tin sản phẩm hiện tại
                ClearProductFields();

                MessageBox.Show("Product removed from cart successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                MessageBox.Show("Error removing product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_print_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrEmpty(txt_invoiceno.Text))
                {
                    MessageBox.Show("Please, Enter Invoice No.", "Try Again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_invoiceno.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txt_customername.Text))
                {
                    MessageBox.Show("Please, Enter Customer Name", "Try Again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_customername.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txt_contact.Text))
                {
                    MessageBox.Show("Please, Enter Customer Contact", "Try Again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_contact.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(txt_address.Text))
                {
                    MessageBox.Show("Please, Enter Customer Address", "Try Again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_address.Focus();
                return;
                }

                // Kiểm tra DataGridView có dữ liệu không
                if (db_dataGridView1.Rows.Count <= 1 || db_dataGridView1.DataSource == null) // 1 row mới không có dữ liệu
                {
                    MessageBox.Show("Cart is empty. Please add products first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Hiện hộp thoại xác nhận
                DialogResult result = MessageBox.Show("Do you want to print the invoice?", "Confirmation",
                                                     MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Tải và hiển thị hóa đơn
                    loadinvoice();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadinvoice()
        {
            try
            {
                // Kiểm tra xem có sản phẩm nào trong giỏ hàng không
                if (_currentBill.Items.Count == 0)
                {
                    MessageBox.Show("No products in the cart to print invoice.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Tạo đối tượng chứa thông tin hóa đơn
                InvoiceData invoiceInfo = new InvoiceData()
                {
                    InvoiceNo = txt_invoiceno.Text,
                    InvoiceDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    CustomerName = txt_customername.Text,
                    CustomerContact = txt_contact.Text,
                    CustomerAddress = txt_address.Text,
                    GrandTotal = _currentBill.Items.Sum(item => item.TotalPrice),
                    Products = ConvertBillItemsToDataTable(_currentBill.Items)
                };

                // Lưu thông tin hóa đơn vào cache
                SaveInvoiceDataToCache(invoiceInfo);

                // Tạo và hiển thị form hóa đơn
                frm_invoice invoiceForm = new frm_invoice();
                invoiceForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error preparing invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable ConvertBillItemsToDataTable(List<BillItem> items)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("ProductID", typeof(string));
            dt.Columns.Add("ProductName", typeof(string));
            dt.Columns.Add("ProductQuantity", typeof(int));
            dt.Columns.Add("ProductPrice", typeof(decimal));
            dt.Columns.Add("TotalAmount", typeof(decimal));

            int stt = 1;
            foreach (var item in items)
            {
                dt.Rows.Add(
                    stt++,
                    item.ProductId,
                    item.ProductName,
                    item.Quantity,
                    item.UnitPrice,
                    item.TotalPrice
                );
            }

            return dt;
        }

        private void SaveInvoiceDataToCache(InvoiceData invoiceInfo)
        {
            try
            {
                string tempPath = Path.Combine(Application.StartupPath, "Data", "temp_invoice.json");
                string jsonData = JsonConvert.SerializeObject(invoiceInfo, Formatting.Indented);
                File.WriteAllText(tempPath, jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error saving invoice data to cache: " + ex.Message);
                // Không ném lại ngoại lệ để không gián đoạn quy trình in
            }
        }

        private void db_procardsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;

            if (index >= 0 && index < db_dataGridView1.Rows.Count)
            {
                DataGridViewRow dgvr = db_dataGridView1.Rows[index];

                // Cập nhật ProductID nếu có
                if (dgvr.Cells["ProductID"].Value != null)
                    txt_productId.Text = dgvr.Cells["ProductID"].Value.ToString();

                // Cập nhật tên sản phẩm
                if (dgvr.Cells["ProductName"].Value != null)
                    txt_productname.Text = dgvr.Cells["ProductName"].Value.ToString();

                // Cập nhật giá sản phẩm
                if (dgvr.Cells["Price"].Value != null)
                    txt_productprice.Text = dgvr.Cells["Price"].Value.ToString();

                // Cập nhật số lượng sản phẩm
                if (dgvr.Cells["Quantity"].Value != null)
                    txt_productquantity.Text = dgvr.Cells["Quantity"].Value.ToString();

                // Cập nhật tổng giá (nếu cần)
                if (dgvr.Cells["TotalPrice"].Value != null)
                    txt_totalprice.Text = dgvr.Cells["TotalPrice"].Value.ToString();
            }
        }

        private void db_dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu thay đổi trong cột "Quantity" hoặc "Price"
            if (e.RowIndex >= 0 && (db_dataGridView1.Columns[e.ColumnIndex].Name == "Quantity" ||
                                   db_dataGridView1.Columns[e.ColumnIndex].Name == "Price"))
            {
                try
                {
                    // Lấy dữ liệu từ hàng đang được thay đổi
                    DataGridViewRow row = db_dataGridView1.Rows[e.RowIndex];

                    if (row.Cells["Price"].Value != null && row.Cells["Quantity"].Value != null)
                    {
                        decimal price;
                        int quantity;

                        if (decimal.TryParse(row.Cells["Price"].Value.ToString(), out price) &&
                            int.TryParse(row.Cells["Quantity"].Value.ToString(), out quantity))
                        {
                            // Cập nhật giá trị TotalPrice cho hàng này
                            decimal newTotalPrice = price * quantity;
                            row.Cells["TotalPrice"].Value = newTotalPrice;

                            // Cập nhật tổng giá của toàn bộ giỏ hàng
                            CalculateCartTotal();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating total price: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ClearCart()
        {
            // Xóa DataSource trước
            db_dataGridView1.DataSource = null;

            // Xóa tất cả các hàng dữ liệu
            if (db_dataGridView1.Rows.Count > 0)
            {
                try
                {
                    db_dataGridView1.Rows.Clear();
                }
                catch
                {
                    // Bỏ qua lỗi nếu không thể xóa
                }
            }

            // Tạo DataTable mới
            DataTable emptyTable = new DataTable();

            // Tạo các cột cần thiết cho giỏ hàng
            emptyTable.Columns.Add("ProductName", typeof(string));
            emptyTable.Columns.Add("Quantity", typeof(int));
            emptyTable.Columns.Add("Price", typeof(decimal));
            emptyTable.Columns.Add("Description", typeof(string));
            emptyTable.Columns.Add("Category", typeof(string));
            emptyTable.Columns.Add("Id", typeof(string));

            // Gán DataTable mới làm DataSource cho DataGridView
            db_dataGridView1.DataSource = emptyTable;

            // Cập nhật tổng giá
            txt_totalprice.Text = "0";
        }

        private void btn_removecart_Click(object sender, EventArgs e)
        {
            try
            {
                ClearCart();
                MessageBox.Show("Cart cleared successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error clearing cart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTotalAmount()
        {
            decimal total = 0;
            for (int i = 0; i < _currentBill.Items.Count; i++)
            {
                total += _currentBill.Items[i].TotalPrice;
            }
            txt_totalprice.Text = total.ToString("N0");
        }
    }

    // Thêm lớp này vào namespace hoặc tạo một file riêng
    public class InvoiceData
    {
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContact { get; set; }
        public string CustomerAddress { get; set; }
        public decimal GrandTotal { get; set; }
        public DataTable Products { get; set; }
    }
}

