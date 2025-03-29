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
        private BillItem _item;
        //private List<BillItem> _items;

        public Billing()
        {
            InitializeComponent();
            _billManager = new BillManager();
            _customerManager =CustomerManager.Instance;
           // _items = new List<BillItem>();

            _currentBill = _billManager.CreateNewBill();

            // Đăng ký các sự kiện để cập nhật tổng giá
           // db_dataGridView1.CellValueChanged += db_dataGridView1_CellValueChanged;
            db_dataGridView1.UserDeletedRow += db_dataGridView1_UserDeletedRow;
            db_dataGridView1.CellDoubleClick += Db_dataGridView1_CellDoubleClick;
        }
        private void auto()
        {
            txt_invoiceno.Text = _currentBill.GenerateId();
        }

        private void UC_Billing_Load(object sender, EventArgs e)
        {
            auto();
            _currentBill = _billManager.CreateNewBill();

            // Đăng ký các sự kiện TextChanged
            if (!EventExists(txt_productId, "TextChanged", "txt_productId_TextChanged"))
                txt_productId.TextChanged += txt_productId_TextChanged;
          
            if (!EventExists(txt_productquantity, "TextChanged", "txt_productquantity_TextChanged"))
                txt_productquantity.TextChanged += txt_productquantity_TextChanged;

            if (!EventExists(txt_customerid, "TextChanged", "txt_customerid_TextChanged"))
                txt_customerid.TextChanged += txt_customerid_TextChanged;

        }

        private void txt_customerid_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txt_customerid.Text))
            {
                BaseCustomer customer = _customerManager.GetById(txt_customerid.Text);
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


        private void ClearProductFields()
        {
            txt_productId.Clear();
            txt_productname.Clear();
            txt_productprice.Clear();
            txt_productquantity.Clear();
            txt_totalprice.Clear();
            txt_productprice.Clear();
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

                    BaseProduct product = _billManager.GetProductById(txt_productId.Text);
                        if (product != null)
                        {
                        if (!_billManager.ValidateQuantity(txt_productId.Text, quantity))
                        {
                            MessageBox.Show($"Số lượng sản phẩm trong kho chỉ còn {product.Quantity}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                txt_productquantity.Text = product.Quantity.ToString();
                            quantity = product.Quantity;
                            }
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
                if (string.IsNullOrEmpty(txt_productId.Text) ||
                    string.IsNullOrEmpty(txt_productquantity.Text)||
                    string.IsNullOrEmpty(txt_customerid.Text))

                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin hóa đơn", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                BaseProduct product = _billManager.GetProductById(txt_productId.Text);

                if (product != null)
                {
                    if (int.TryParse(txt_productquantity.Text, out int quantity))
                    {
                        BillItem _item = new BillItem(txt_productId.Text, txt_productname.Text, quantity, product.Price);
                      
                        _billManager.AddItem(_item);
                        txt_totalprice.Text = _billManager.CalculateTotalCart2().ToString("N0");
                        _currentBill.Items.Add(_item);
                        UpdateDataGridView();
                        ClearProductFields();
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
                dt.Columns.Add("UnitPrice", typeof(decimal));
               
                dt.Columns.Add("ProductId", typeof(string));
                dt.Columns.Add("TotalPrice", typeof(decimal));

                foreach (BillItem item in _currentBill.Items)
                {
                    BillItem _item = _billManager.GetItemById(item.ProductId);
                    if (_item == null)
                        continue;

                    dt.Rows.Add(
                        _item.ProductName,
                        _item.Quantity,
                        _item.UnitPrice,
                        _item.ProductId,
                        _item.TotalPrice
                    );
                }

                db_dataGridView1.DataSource = dt;

                // Định dạng số tiền
                if (db_dataGridView1.Columns["Price"] != null)
                    db_dataGridView1.Columns["Price"].DefaultCellStyle.Format = "N0";
                if (db_dataGridView1.Columns["TotalProductPrice"] != null)
                    db_dataGridView1.Columns["TotalProductPrice"].DefaultCellStyle.Format = "N0";

                // Tính tổng tiền
                decimal total = 0;
                for (int i = 0; i < _currentBill.Items.Count; i++)
                {
                    total += _currentBill.Items[i].TotalPrice;
                }
                txt_totalprice.Text = total.ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating grid: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

         
            // Cập nhật tổng giá
            txt_totalprice.Text = "0";
        }
        private void ClearInputFields()
        {
            txt_productId.Clear();
            txt_productname.Clear();
            txt_productquantity.Clear();
            txt_totalprice.Clear();
            txt_productprice.Clear();
            txt_customername.Clear();
            txt_address.Clear();
            txt_customerid.Clear();
            txt_contact.Clear();

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
                _currentBill.TotalCart= _billManager.CalculateTotalCart2();
                for (int i = 0; i < _currentBill.Items.Count; i++)
                {
                    BillItem item = _currentBill.Items[i];
                    _billManager.UpdateProductQuantity(item.ProductId, item.Quantity);
                }
               
                _billManager.SaveBill(_currentBill);
                MessageBox.Show("Lưu hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Tạo hóa đơn mới
                _currentBill = _billManager.CreateNewBill();
                txt_invoiceno.Text = _currentBill.BillId;
                UpdateDataGridView();
                ClearInputFields();
                
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
                _billManager.RemoveItem(item);
                txt_totalprice.Text = _billManager.CalculateTotalCart2().ToString("N0");
                UpdateDataGridView();
            }
        }

        private bool EventExists(Control control, string eventName, string handlerName)
        {
            System.Reflection.FieldInfo eventField = typeof(Control).GetField(eventName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (eventField != null)
            {
                object eventValue = eventField.GetValue(control);
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
                    BaseProduct product = _billManager.GetProductById(txt_productId.Text);
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
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in txt_productId_TextChanged: " + ex.Message);
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
                txt_totalprice.Text = _billManager.CalculateTotalCart2().ToString("N0");

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

                // Tính tổng giá trị hóa đơn
                decimal grandTotal = 0;
                for (int i = 0; i < _currentBill.Items.Count; i++)
                {
                    grandTotal += _currentBill.Items[i].TotalPrice;
                }

                // Tạo đối tượng chứa thông tin hóa đơn
                InvoiceData invoiceInfo = new InvoiceData()
                {
                    InvoiceNo = txt_invoiceno.Text,
                    InvoiceDate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                    CustomerName = txt_customername.Text,
                    CustomerContact = txt_contact.Text,
                    CustomerAddress = txt_address.Text,
                    GrandTotal = grandTotal,
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
            for (int i = 0; i < items.Count; i++)
            {
                BillItem item = items[i];
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
                // Chuyển DataTable thành một danh sách các đối tượng
                List<ProductRow> productRows = new List<ProductRow>();
                for (int i = 0; i < invoiceInfo.Products.Rows.Count; i++)
                {
                    DataRow row = invoiceInfo.Products.Rows[i];
                    ProductRow productRow = new ProductRow
                    {
                        STT = Convert.ToInt32(row["STT"]),
                        ProductID = row["ProductID"].ToString(),
                        ProductName = row["ProductName"].ToString(),
                        ProductQuantity = Convert.ToInt32(row["ProductQuantity"]),
                        ProductPrice = Convert.ToDecimal(row["ProductPrice"]),
                        TotalAmount = Convert.ToDecimal(row["TotalAmount"])
                    };
                    productRows.Add(productRow);
                }

                // Tạo đối tượng dữ liệu mới với danh sách sản phẩm
                object invoiceData = new
                {
                    InvoiceNo = invoiceInfo.InvoiceNo,
                    InvoiceDate = invoiceInfo.InvoiceDate,
                    CustomerName = invoiceInfo.CustomerName,
                    CustomerContact = invoiceInfo.CustomerContact,
                    CustomerAddress = invoiceInfo.CustomerAddress,
                    GrandTotal = invoiceInfo.GrandTotal,
                    Products = productRows
                };

                string jsonData = JsonConvert.SerializeObject(invoiceData, Formatting.Indented);
                
                // Tạo thư mục Data nếu chưa tồn tại
                string appPath = Application.StartupPath;
                string dataDir = Path.Combine(appPath, "Data");
                if (!Directory.Exists(dataDir))
                {
                    Directory.CreateDirectory(dataDir);
                }
                
                // Lưu file vào thư mục Data
                string tempPath = Path.Combine(dataDir, "temp_invoice.json");
                File.WriteAllText(tempPath, jsonData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving invoice data to cache: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

      
        private void Db_dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Check if the click was on a valid row (not header, not -1)
            if (e.RowIndex >= 0 && e.RowIndex < db_dataGridView1.Rows.Count)
            {
                // Process the double-click on a valid row here
                DataGridViewRow row = db_dataGridView1.Rows[e.RowIndex];
                // Do something with the row data, like selecting it or showing details
            }
            // If index is -1 or otherwise invalid, we do nothing, preventing the exception
        }
    }

   

    // Class để lưu thông tin từng dòng sản phẩm khi serialization
    public class ProductRow
    {
        public int STT { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public int ProductQuantity { get; set; }
        public decimal ProductPrice { get; set; }
        public decimal TotalAmount { get; set; }
       
    }


}

