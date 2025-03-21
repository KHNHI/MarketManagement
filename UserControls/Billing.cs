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

namespace MarketManagement.UserControls
{
    public partial class Billing : UserControl
    {
        public static string invoiceid;
        public Billing()
        {
            InitializeComponent();

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

            // Không cần đăng ký lại sự kiện nếu đã đăng ký trong constructor
            // Chỉ đăng ký các sự kiện TextChanged nếu cần thiết
            if (!EventExists(txt_productId, "TextChanged", "txt_productId_TextChanged"))
                txt_productId.TextChanged += txt_productId_TextChanged;

            if (!EventExists(txt_productname, "TextChanged", "txt_productname_TextChanged"))
                txt_productname.TextChanged += txt_productname_TextChanged;

            if (!EventExists(txt_productquantity, "TextChanged", "txt_productquantity_TextChanged"))
                txt_productquantity.TextChanged += txt_productquantity_TextChanged;

            // Các sự kiện DataGridView đã được đăng ký trong constructor, không cần đăng ký lại
        }

        // Thêm phương thức này để kiểm tra sự kiện đã tồn tại chưa
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

        private void showdata()
        {
            try
            {
                // Đường dẫn tới file JSON
                string jsonFilePath = Path.Combine(Application.StartupPath, "Data", "products.json");

                // Kiểm tra file tồn tại
                if (!File.Exists(jsonFilePath))
                {
                    MessageBox.Show("JSON data file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Đọc toàn bộ nội dung file JSON
                string jsonData = File.ReadAllText(jsonFilePath);

                // Chuyển JSON thành danh sách sản phẩm
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                List<BaseProduct> allProducts = JsonConvert.DeserializeObject<List<BaseProduct>>(jsonObject["products"].ToString());

                // Danh sách chứa các sản phẩm khớp với Id
                List<BaseProduct> filteredProducts = new List<BaseProduct>();

                // Duyệt thủ công để tìm sản phẩm có Id khớp
                foreach (BaseProduct product in allProducts)
                {
                    if (product.Id == txt_invoiceno.Text)
                    {
                        filteredProducts.Add(product);
                    }
                }

                // Hiển thị kết quả trong DataGridView
                db_dataGridView1.DataSource = filteredProducts;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    MessageBox.Show("Please, Enter Product Name", "Try Again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_productname.Focus();
                }
                else
                {
                    // Đường dẫn file JSON
                    string filePath = Path.Combine(Application.StartupPath, "Data", "products.json");

                    // Kiểm tra xem file có tồn tại không
                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show("Không tìm thấy file: " + filePath, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Đọc dữ liệu JSON
                    string jsonData = File.ReadAllText(filePath);

                    // Sử dụng JsonDocument để phân tích dữ liệu
                    using (JsonDocument doc = JsonDocument.Parse(jsonData))
                    {
                        JsonElement root = doc.RootElement;
                        if (root.TryGetProperty("Products", out JsonElement productsElement) &&
                            productsElement.ValueKind == JsonValueKind.Array)
                        {
                            bool foundProduct = false;
                            int requestedQuantity = 0;

                            // Kiểm tra nếu giá trị nhập vào là số hợp lệ
                            if (!int.TryParse(txt_productquantity.Text, out requestedQuantity))
                            {
                                MessageBox.Show("Vui lòng nhập số lượng hợp lệ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txt_productquantity.Clear();
                                return;
                            }

                            // Tìm sản phẩm trong mảng
                            foreach (JsonElement product in productsElement.EnumerateArray())
                            {
                                if (product.TryGetProperty("ProductName", out JsonElement nameElement) &&
                                    nameElement.GetString().Equals(txt_productname.Text, StringComparison.OrdinalIgnoreCase))
                                {
                                    foundProduct = true;

                                    // Kiểm tra số lượng sản phẩm
                                    if (product.TryGetProperty("Quantity", out JsonElement quantityElement))
                                    {
                                        int availableQuantity = quantityElement.GetInt32();

                                        if (requestedQuantity > availableQuantity)
                                        {
                                            MessageBox.Show($"Available quantity is only {availableQuantity}", "Quantity Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                            txt_productquantity.Text = availableQuantity.ToString();
                                            requestedQuantity = availableQuantity;
                                        }

                                        // Tính tổng giá
                                        if (product.TryGetProperty("Price", out JsonElement priceElement))
                                        {
                                            decimal price = 0;

                                            if (priceElement.ValueKind == JsonValueKind.Number)
                                            {
                                                price = priceElement.GetDecimal();
                                            }
                                            else if (priceElement.ValueKind == JsonValueKind.String)
                                            {
                                                decimal.TryParse(priceElement.GetString(), out price);
                                            }

                                            decimal tprice = price * requestedQuantity;
                                            // Hiển thị không có định dạng tiền tệ
                                            txt_totalprice.Text = tprice.ToString("0.00");
                                        }
                                        break;
                                    }
                                }
                            }

                            if (!foundProduct)
                            {
                                MessageBox.Show("Product not found in database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                txt_totalprice.Clear();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid JSON format", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating price: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Phương thức thêm sản phẩm vào giỏ hàng
        private void btn_addtocard_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra các thông tin cần thiết
                if (string.IsNullOrEmpty(txt_productname.Text) ||
                    string.IsNullOrEmpty(txt_productquantity.Text) ||
                    string.IsNullOrEmpty(txt_totalprice.Text))
                {
                    MessageBox.Show("Please fill all product details", "Incomplete Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Đường dẫn file JSON
                string filePath = Path.Combine(Application.StartupPath, "Data", "products.json");

                // Kiểm tra xem file có tồn tại không
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Không tìm thấy file: " + filePath, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string jsonData = File.ReadAllText(filePath);
                string productId = "";
                decimal productPrice = 0;

                // Lấy ID sản phẩm từ file JSON
                using (JsonDocument doc = JsonDocument.Parse(jsonData))
                {
                    JsonElement root = doc.RootElement;
                    if (root.TryGetProperty("Products", out JsonElement productsElement) &&
                        productsElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement product in productsElement.EnumerateArray())
                        {
                            if (product.TryGetProperty("ProductName", out JsonElement nameElement) &&
                                nameElement.GetString().Equals(txt_productname.Text, StringComparison.OrdinalIgnoreCase))
                            {
                                // Lấy ID sản phẩm
                                if (product.TryGetProperty("Id", out JsonElement idElement))
                                {
                                    productId = idElement.GetString();
                                }

                                // Lấy giá sản phẩm
                                if (product.TryGetProperty("Price", out JsonElement priceElement))
                                {
                                    if (priceElement.ValueKind == JsonValueKind.Number)
                                    {
                                        productPrice = priceElement.GetDecimal();
                                    }
                                    else if (priceElement.ValueKind == JsonValueKind.String)
                                    {
                                        decimal.TryParse(priceElement.GetString(), out productPrice);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(productId))
                {
                    MessageBox.Show("Could not find product ID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Thêm sản phẩm vào DataTable thay vì trực tiếp vào DataGridView
                int quantity = int.Parse(txt_productquantity.Text);
                decimal totalPrice = decimal.Parse(txt_totalprice.Text);

                // Lấy DataTable từ DataSource của DataGridView
                DataTable cartTable;
                if (db_dataGridView1.DataSource is DataTable)
                {
                    cartTable = (DataTable)db_dataGridView1.DataSource;
                }
                else if (db_dataGridView1.DataSource is BindingSource && ((BindingSource)db_dataGridView1.DataSource).DataSource is DataTable)
                {
                    cartTable = (DataTable)((BindingSource)db_dataGridView1.DataSource).DataSource;
                }
                else
                {
                    // Nếu chưa có DataSource, tạo mới DataTable và thiết lập làm DataSource
                    cartTable = new DataTable();

                    // Tạo các cột cần thiết nếu DataTable mới
                    if (cartTable.Columns.Count == 0)
                    {
                        cartTable.Columns.Add("ProductID", typeof(string));
                        cartTable.Columns.Add("ProductName", typeof(string));
                        cartTable.Columns.Add("Price", typeof(decimal));
                        cartTable.Columns.Add("Quantity", typeof(int));
                        cartTable.Columns.Add("TotalPrice", typeof(decimal));
                    }

                    // Gán DataTable làm DataSource cho DataGridView
                    db_dataGridView1.DataSource = cartTable;
                }

                // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
                bool productExists = false;
                DataRow existingRow = null;

                // Tìm theo ProductID
                if (cartTable.Columns.Contains("ProductID"))
                {
                    foreach (DataRow row in cartTable.Rows)
                    {
                        if (row["ProductID"].ToString() == productId)
                        {
                            productExists = true;
                            existingRow = row;
                            break;
                        }
                    }
                }
                // Nếu không có cột ProductID, tìm theo ProductName
                else if (cartTable.Columns.Contains("ProductName"))
                {
                    foreach (DataRow row in cartTable.Rows)
                    {
                        if (row["ProductName"].ToString() == txt_productname.Text)
                        {
                            productExists = true;
                            existingRow = row;
                            break;
                        }
                    }
                }

                if (productExists && existingRow != null)
                {
                    // Cập nhật số lượng và tổng giá nếu sản phẩm đã tồn tại
                    int currentQuantity = Convert.ToInt32(existingRow["Quantity"]);
                    decimal currentTotalPrice = Convert.ToDecimal(existingRow["TotalPrice"]);

                    existingRow["Quantity"] = currentQuantity + quantity;
                    existingRow["TotalPrice"] = currentTotalPrice + totalPrice;

                    MessageBox.Show("Product quantity updated in cart", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Thêm sản phẩm mới vào DataTable
                    DataRow newRow = cartTable.NewRow();

                    if (cartTable.Columns.Contains("ProductID"))
                        newRow["ProductID"] = productId;

                    newRow["ProductName"] = txt_productname.Text;
                    newRow["Price"] = productPrice;
                    newRow["Quantity"] = quantity;
                    newRow["TotalPrice"] = totalPrice;

                    cartTable.Rows.Add(newRow);

                    MessageBox.Show("Product added to cart", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Tính lại tổng giá trị giỏ hàng
                CalculateCartTotal();

                // Xóa các ô nhập liệu để người dùng có thể thêm sản phẩm mới
                ClearProductFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product to cart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // This function now calculates the total without updating txt_totalprice
        private void CalculateCartTotal()
        {
            try
            {
                decimal grandTotal = 0;

                // Kiểm tra nếu DataGridView có dữ liệu
                if (db_dataGridView1.DataSource != null)
                {
                    DataTable cartTable;

                    // Lấy DataTable từ DataSource
                    if (db_dataGridView1.DataSource is DataTable)
                    {
                        cartTable = (DataTable)db_dataGridView1.DataSource;
                    }
                    else if (db_dataGridView1.DataSource is BindingSource && ((BindingSource)db_dataGridView1.DataSource).DataSource is DataTable)
                    {
                        cartTable = (DataTable)((BindingSource)db_dataGridView1.DataSource).DataSource;
                    }
                    else
                    {
                        // Nếu không lấy được DataTable, duyệt qua các hàng trực tiếp
                        foreach (DataGridViewRow row in db_dataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                // Kiểm tra cột TotalPrice trước
                                if (db_dataGridView1.Columns.Contains("TotalPrice") && row.Cells["TotalPrice"].Value != null)
                                {
                                    decimal rowTotal;
                                    if (decimal.TryParse(row.Cells["TotalPrice"].Value.ToString(), out rowTotal))
                                    {
                                        grandTotal += rowTotal;
                                    }
                                }
                                // Nếu không có cột TotalPrice, tính từ Price và Quantity
                                else if (db_dataGridView1.Columns.Contains("Price") && db_dataGridView1.Columns.Contains("Quantity") &&
                                        row.Cells["Price"].Value != null && row.Cells["Quantity"].Value != null)
                                {
                                    decimal price;
                                    int quantity;
                                    if (decimal.TryParse(row.Cells["Price"].Value.ToString(), out price) &&
                                        int.TryParse(row.Cells["Quantity"].Value.ToString(), out quantity))
                                    {
                                        grandTotal += price * quantity;
                                    }
                                }
                            }
                        }

                        // Cập nhật tổng giá
                        txt_totalprice.Text = grandTotal.ToString("0.00");
                        return;
                    }

                    // Tính tổng từ DataTable
                    if (cartTable.Columns.Contains("TotalPrice"))
                    {
                        foreach (DataRow row in cartTable.Rows)
                        {
                            if (row["TotalPrice"] != DBNull.Value)
                            {
                                grandTotal += Convert.ToDecimal(row["TotalPrice"]);
                            }
                        }
                    }
                    else if (cartTable.Columns.Contains("Price") && cartTable.Columns.Contains("Quantity"))
                    {
                        foreach (DataRow row in cartTable.Rows)
                        {
                            if (row["Price"] != DBNull.Value && row["Quantity"] != DBNull.Value)
                            {
                                decimal price = Convert.ToDecimal(row["Price"]);
                                int quantity = Convert.ToInt32(row["Quantity"]);
                                grandTotal += price * quantity;
                            }
                        }
                    }
                }
                else
                {
                    // Nếu không có DataSource, duyệt qua các hàng trong DataGridView
                    foreach (DataGridViewRow row in db_dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            if (db_dataGridView1.Columns.Contains("TotalPrice") && row.Cells["TotalPrice"].Value != null)
                            {
                                decimal rowTotal;
                                if (decimal.TryParse(row.Cells["TotalPrice"].Value.ToString(), out rowTotal))
                                {
                                    grandTotal += rowTotal;
                                }
                            }
                            else if (db_dataGridView1.Columns.Contains("Price") && db_dataGridView1.Columns.Contains("Quantity") &&
                                    row.Cells["Price"].Value != null && row.Cells["Quantity"].Value != null)
                            {
                                decimal price;
                                int quantity;
                                if (decimal.TryParse(row.Cells["Price"].Value.ToString(), out price) &&
                                    int.TryParse(row.Cells["Quantity"].Value.ToString(), out quantity))
                                {
                                    grandTotal += price * quantity;
                                }
                            }
                        }
                    }
                }

                // Cập nhật tổng giá
                txt_totalprice.Text = grandTotal.ToString("0.00");
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                MessageBox.Show("Error calculating total: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Phương thức xóa các ô nhập liệu
        private void ClearProductFields()
        {
            txt_productname.Clear();
            txt_productprice.Clear();
            txt_productquantity.Clear();
            txt_totalprice.Clear();
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



        private void txt_customerid_TextChanged(object sender, EventArgs e)
        {
            LookupCustomer(txt_customerid.Text);
        }

        private void LookupCustomer(string customerId)
        {
            try
            {
                // Nếu không có ID khách hàng, xóa thông tin liên quan
                if (string.IsNullOrWhiteSpace(customerId))
                {
                    txt_contact.Clear();
                    txt_address.Clear();
                    txt_customerid.Text = "";
                    // Thêm dòng này để xóa tên khách hàng
                    txt_customername.Clear(); // hoặc tên control hiển thị tên khách hàng của bạn
                    return;
                }

                // Đường dẫn tới file JSON
                string jsonFilePath = Path.Combine(Application.StartupPath, "Data", "customers.json");

                // Kiểm tra file tồn tại
                if (!File.Exists(jsonFilePath))
                {
                    MessageBox.Show($"Customer data file not found at: {jsonFilePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Đọc nội dung file JSON
                string jsonData = File.ReadAllText(jsonFilePath);

                // Sử dụng JsonDocument để xử lý JSON
                using (JsonDocument doc = JsonDocument.Parse(jsonData))
                {
                    JsonElement root = doc.RootElement;

                    // Kiểm tra cấu trúc JSON với thuộc tính "Customers" (chữ C viết hoa)
                    if (root.TryGetProperty("Customers", out JsonElement customersElement) &&
                        customersElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement customer in customersElement.EnumerateArray())
                        {
                            // Sử dụng trường "Id" thay vì "cusid"
                            if (customer.TryGetProperty("Id", out JsonElement idElement) &&
                                idElement.GetString().Equals(customerId, StringComparison.OrdinalIgnoreCase))
                            {
                                // Tìm thấy khách hàng theo ID
                                string customerName = "";
                                string phoneNumber = "";
                                string address = "";

                                // Lấy thông tin tên khách hàng
                                if (customer.TryGetProperty("CustomerName", out JsonElement nameElement))
                                    customerName = nameElement.GetString();

                                // Lấy thông tin số điện thoại
                                if (customer.TryGetProperty("PhoneNumber", out JsonElement phoneElement))
                                    phoneNumber = phoneElement.GetString();

                                // Lấy thông tin địa chỉ
                                if (customer.TryGetProperty("Address", out JsonElement addressElement))
                                    address = addressElement.GetString();

                                // Cập nhật thông tin vào form
                                txt_customerid.Text = customerId;
                                txt_contact.Text = phoneNumber;
                                txt_address.Text = address;

                                // Thêm dòng này để cập nhật tên khách hàng
                                txt_customername.Text = customerName; // Thay txt_customerName bằng tên control hiển thị tên khách hàng của bạn

                                return;
                            }
                        }

                        // Không tìm thấy khách hàng với ID này
                        MessageBox.Show($"Customer with ID '{customerId}' not found.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txt_contact.Clear();
                        txt_address.Clear();
                        txt_customerid.Text = "";

                        // Thêm dòng này để xóa tên khách hàng khi không tìm thấy
                        txt_customername.Clear(); // hoặc tên control hiển thị tên khách hàng của bạn
                    }
                    else
                    {
                        MessageBox.Show("JSON structure does not contain 'Customers' array", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_check_product_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txt_productId.Text))
            {
                // Chỉ cần gọi txt_productId_TextChanged vì nó đã chứa logic tra cứu sản phẩm
                txt_productId_TextChanged(sender, e);
            }
            else
            {
                MessageBox.Show("Please enter a product ID first", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txt_productId.Focus();
            }
        }

        private void txt_productId_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Luôn tìm kiếm sản phẩm khi ID thay đổi
                if (!string.IsNullOrWhiteSpace(txt_productId.Text))
                {
                    // Đường dẫn tới file JSON
                    string filePath = Path.Combine(Application.StartupPath, "Data", "products.json");

                    // Kiểm tra file tồn tại
                    if (!File.Exists(filePath))
                    {
                        MessageBox.Show($"Product data file not found at: {filePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Đọc nội dung file JSON
                    string jsonData = File.ReadAllText(filePath);

                    // Sử dụng JsonDocument để xử lý JSON
                    using (JsonDocument doc = JsonDocument.Parse(jsonData))
                    {
                        JsonElement root = doc.RootElement;

                        // Kiểm tra cấu trúc JSON với thuộc tính "Products"
                        if (root.TryGetProperty("Products", out JsonElement productsElement) &&
                            productsElement.ValueKind == JsonValueKind.Array)
                        {
                            foreach (JsonElement product in productsElement.EnumerateArray())
                            {
                                // Sử dụng trường "Id" để tìm sản phẩm
                                if (product.TryGetProperty("Id", out JsonElement idElement) &&
                                    idElement.GetString().Equals(txt_productId.Text, StringComparison.OrdinalIgnoreCase))
                                {
                                    // Tìm thấy sản phẩm theo ID
                                    string productName = "";
                                    string price = "";
                                    int availableQuantity = 0;

                                    // Lấy thông tin tên sản phẩm
                                    if (product.TryGetProperty("ProductName", out JsonElement nameElement))
                                        productName = nameElement.GetString();

                                    // Lấy thông tin giá sản phẩm
                                    if (product.TryGetProperty("Price", out JsonElement priceElement))
                                    {
                                        if (priceElement.ValueKind == JsonValueKind.Number)
                                            price = priceElement.GetDecimal().ToString();
                                        else
                                            price = priceElement.GetString();
                                    }

                                    // Lấy thông tin số lượng tồn kho
                                    if (product.TryGetProperty("Quantity", out JsonElement quantityElement))
                                        availableQuantity = quantityElement.GetInt32();

                                    // Cập nhật thông tin vào form (điều quan trọng là phải cập nhật các text box)
                                    txt_productname.Text = productName;
                                    txt_productprice.Text = price;

                                    // Thiết lập số lượng mặc định là 1 nếu có sẵn
                                    if (availableQuantity > 0)
                                        txt_productquantity.Text = "1";
                                    else
                                        txt_productquantity.Text = "0";

                                    // Tính giá trị tổng
                                    if (!string.IsNullOrEmpty(price) && availableQuantity > 0)
                                    {
                                        decimal priceValue;
                                        if (decimal.TryParse(price, out priceValue))
                                        {
                                            decimal totalValue = priceValue * 1;
                                            txt_totalprice.Text = totalValue.ToString("0.00");
                                        }
                                    }

                                    return; // Tìm thấy sản phẩm, thoát khỏi vòng lặp
                                }
                            }

                            // Chỉ xóa các trường khi không tìm thấy và đã nhập đủ ký tự để tìm kiếm
                            if (txt_productId.Text.Length >= 3)
                            {
                                txt_productname.Clear();
                                txt_productprice.Clear();
                                txt_productquantity.Clear();
                                txt_totalprice.Clear();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi lại lỗi nhưng không hiển thị cho người dùng (để tránh gián đoạn khi nhập liệu)
                Console.WriteLine("Error in txt_productId_TextChanged: " + ex.Message);
            }
        }

        private void txt_productname_TextChanged(object sender, EventArgs e)
        {
            // Chỉ tìm kiếm nếu txt_productId trống và ít nhất đã nhập 3 ký tự tên
            if (string.IsNullOrWhiteSpace(txt_productId.Text) && txt_productname.Text.Length >= 3)
            {
                LookupProductByName(txt_productname.Text);
            }
        }

        private void LookupProductByName(string productName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productName) || productName.Length < 3)
                {
                    return; // Không tìm kiếm nếu tên quá ngắn
                }

                // Đường dẫn tới file JSON
                string filePath = Path.Combine(Application.StartupPath, "Data", "products.json");

                // Kiểm tra file tồn tại
                if (!File.Exists(filePath))
                {
                    return; // Không hiển thị lỗi vì người dùng có thể đang nhập liệu
                }

                // Đọc nội dung file JSON
                string jsonData = File.ReadAllText(filePath);

                // Sử dụng JsonDocument để xử lý JSON
                using (JsonDocument doc = JsonDocument.Parse(jsonData))
                {
                    JsonElement root = doc.RootElement;

                    // Kiểm tra cấu trúc JSON
                    if (root.TryGetProperty("Products", out JsonElement productsElement) &&
                        productsElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement product in productsElement.EnumerateArray())
                        {
                            if (product.TryGetProperty("ProductName", out JsonElement nameElement) &&
                                nameElement.GetString().IndexOf(productName, StringComparison.OrdinalIgnoreCase) >= 0)
                            {
                                // Tìm thấy sản phẩm có tên chứa chuỗi tìm kiếm
                                string productId = "";
                                string price = "";
                                int availableQuantity = 0;

                                // Lấy ID sản phẩm
                                if (product.TryGetProperty("Id", out JsonElement idElement))
                                    productId = idElement.GetString();

                                // Lấy giá sản phẩm
                                if (product.TryGetProperty("Price", out JsonElement priceElement))
                                {
                                    if (priceElement.ValueKind == JsonValueKind.Number)
                                        price = priceElement.GetDecimal().ToString();
                                    else
                                        price = priceElement.GetString();
                                }

                                // Lấy số lượng tồn kho
                                if (product.TryGetProperty("Quantity", out JsonElement quantityElement))
                                    availableQuantity = quantityElement.GetInt32();

                                // Cập nhật thông tin
                                txt_productId.Text = productId;
                                txt_productname.Text = nameElement.GetString(); // Tên đầy đủ của sản phẩm
                                txt_productprice.Text = price;

                                // Thiết lập số lượng mặc định
                                if (availableQuantity > 0)
                                    txt_productquantity.Text = "1";
                                else
                                    txt_productquantity.Text = "0";

                                // Tính tổng giá
                                if (!string.IsNullOrEmpty(price) && availableQuantity > 0)
                                {
                                    decimal priceValue;
                                    if (decimal.TryParse(price, out priceValue))
                                    {
                                        decimal totalValue = priceValue * 1;
                                        txt_totalprice.Text = totalValue.ToString("0.00");
                                    }
                                }

                                return; // Tìm thấy sản phẩm đầu tiên phù hợp và thoát
                            }
                        }
                    }
                }
            }
            catch
            {
                // Không hiển thị lỗi khi người dùng đang nhập liệu
            }
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            try
            {
                // Tính lại tổng giá trị giỏ hàng trước khi lưu để đảm bảo dữ liệu chính xác
                CalculateCartTotal();

                // Kiểm tra thông tin đầu vào
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
                if (string.IsNullOrEmpty(txt_totalprice.Text))
                {
                    MessageBox.Show("Please, Enter Product Grand Total", "Try Again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_totalprice.Focus();
                    return;
                }

                // Và thay thế bằng đoạn code sau:
                decimal grandTotal;
                if (!decimal.TryParse(txt_totalprice.Text, out grandTotal))
                {
                    // Nếu parsing thất bại, thử loại bỏ các ký tự định dạng tiền tệ
                    string totalPriceText = txt_totalprice.Text.Replace("$", "").Replace(",", "").Replace("C", "");
                    if (!decimal.TryParse(totalPriceText, out grandTotal))
                    {
                        MessageBox.Show("Invalid total price format. Please check the total price.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txt_totalprice.Focus();
                        return;
                    }
                }

                // Tạo đối tượng đơn hàng mới
                var newOrder = new Model.Order
                {
                    InvoiceDate = dtp_invoicedate.Text,
                    InvoiceNo = txt_invoiceno.Text,
                    CustomerId = string.IsNullOrEmpty(txt_customerid.Text) ? txt_customerid.Text : txt_customerid.Text,
                    CustomerName = txt_customername.Text,
                    Contact = txt_contact.Text,
                    Address = txt_address.Text,
                    GrandTotal = grandTotal,
                    OrderDetails = new List<Model.OrderDetail>()
                };

                // Thêm chi tiết sản phẩm vào đơn hàng từ DataGridView
                if (db_dataGridView1 != null && db_dataGridView1.Rows.Count > 0)
                {
                    // Lấy DataTable từ DataSource nếu có
                    DataTable cartTable = null;
                    if (db_dataGridView1.DataSource is DataTable)
                        cartTable = (DataTable)db_dataGridView1.DataSource;
                    else if (db_dataGridView1.DataSource is BindingSource &&
                            ((BindingSource)db_dataGridView1.DataSource).DataSource is DataTable)
                        cartTable = (DataTable)((BindingSource)db_dataGridView1.DataSource).DataSource;

                    // Nếu có DataTable, lấy dữ liệu từ đó
                    if (cartTable != null)
                    {
                        foreach (DataRow row in cartTable.Rows)
                        {
                            // Biến để lưu trữ dữ liệu
                            decimal unitPrice = 0;
                            int quantity = 0;
                            string productId = "";
                            string productName = "";
                            decimal total = 0;

                            // Lấy dữ liệu từ các cột của DataTable
                            if (cartTable.Columns.Contains("ProductID") && row["ProductID"] != DBNull.Value)
                                productId = row["ProductID"].ToString();

                            if (cartTable.Columns.Contains("ProductName") && row["ProductName"] != DBNull.Value)
                                productName = row["ProductName"].ToString();

                            if (cartTable.Columns.Contains("Price") && row["Price"] != DBNull.Value)
                                unitPrice = Convert.ToDecimal(row["Price"]);

                            if (cartTable.Columns.Contains("Quantity") && row["Quantity"] != DBNull.Value)
                                quantity = Convert.ToInt32(row["Quantity"]);

                            if (cartTable.Columns.Contains("TotalPrice") && row["TotalPrice"] != DBNull.Value)
                                total = Convert.ToDecimal(row["TotalPrice"]);
                            else
                                total = unitPrice * quantity;

                            // Đảm bảo số lượng là ít nhất 1
                            if (quantity <= 0) quantity = 1;

                            // Thêm chi tiết đơn hàng
                            var detail = new Model.OrderDetail
                            {
                                ProductId = productId,
                                ProductName = productName,
                                Quantity = quantity,
                                Price = unitPrice,
                                Total = total
                            };
                            newOrder.OrderDetails.Add(detail);
                        }
                    }
                    else
                    {
                        // Nếu không có DataTable, lấy trực tiếp từ DataGridView
                        foreach (DataGridViewRow row in db_dataGridView1.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                // Biến để lưu trữ dữ liệu
                                decimal unitPrice = 0;
                                int quantity = 0;
                                string productId = "";
                                string productName = "";
                                decimal total = 0;

                                // Lấy dữ liệu từ các cột của DataGridView
                                if (db_dataGridView1.Columns.Contains("ProductID") && row.Cells["ProductID"].Value != null)
                                    productId = row.Cells["ProductID"].Value.ToString();

                                if (db_dataGridView1.Columns.Contains("ProductName") && row.Cells["ProductName"].Value != null)
                                    productName = row.Cells["ProductName"].Value.ToString();

                                if (db_dataGridView1.Columns.Contains("Price") && row.Cells["Price"].Value != null)
                                    decimal.TryParse(row.Cells["Price"].Value.ToString(), out unitPrice);

                                if (db_dataGridView1.Columns.Contains("Quantity") && row.Cells["Quantity"].Value != null)
                                    int.TryParse(row.Cells["Quantity"].Value.ToString(), out quantity);

                                if (db_dataGridView1.Columns.Contains("TotalPrice") && row.Cells["TotalPrice"].Value != null)
                                    decimal.TryParse(row.Cells["TotalPrice"].Value.ToString(), out total);
                                else
                                    total = unitPrice * quantity;

                                // Đảm bảo số lượng là ít nhất 1
                                if (quantity <= 0) quantity = 1;

                                // Thêm chi tiết đơn hàng
                                var detail = new Model.OrderDetail
                                {
                                    ProductId = productId,
                                    ProductName = productName,
                                    Quantity = quantity,
                                    Price = unitPrice,
                                    Total = total
                                };
                                newOrder.OrderDetails.Add(detail);
                            }
                        }
                    }
                }

                // Đường dẫn đầy đủ đến file orders.json
                string dataDirectory = Path.Combine(Application.StartupPath, "Data");
                // Đảm bảo thư mục Data tồn tại
                if (!Directory.Exists(dataDirectory))
                {
                    Directory.CreateDirectory(dataDirectory);
                }

                string ordersFilePath = Path.Combine(dataDirectory, "orders.json");

                // Khởi tạo đối tượng OrdersData
                Model.OrdersData ordersData;

                // Kiểm tra xem file có tồn tại không
                if (File.Exists(ordersFilePath))
                {
                    try
                    {
                        string jsonData = File.ReadAllText(ordersFilePath);

                        // Kiểm tra nội dung file
                        if (string.IsNullOrWhiteSpace(jsonData))
                        {
                            // Nếu file rỗng, tạo mới cấu trúc
                            ordersData = new Model.OrdersData
                            {
                                Orders = new List<Model.Order>()
                            };
                        }
                        else
                        {
                            try
                            {
                                // Cố gắng đọc file với cấu trúc đúng
                                ordersData = JsonConvert.DeserializeObject<Model.OrdersData>(jsonData);

                                // Nếu null hoặc không có danh sách Orders, tạo mới
                                if (ordersData == null)
                                {
                                    ordersData = new Model.OrdersData
                                    {
                                        Orders = new List<Model.Order>()
                                    };
                                }
                                else if (ordersData.Orders == null)
                                {
                                    ordersData.Orders = new List<Model.Order>();
                                }
                            }
                            catch
                            {
                                // Nếu không thể deserialize trực tiếp, tạo mới cấu trúc
                                ordersData = new Model.OrdersData
                                {
                                    Orders = new List<Model.Order>()
                                };

                                // Thử đọc JSON dưới dạng JObject
                                try
                                {
                                    JObject jObject = JObject.Parse(jsonData);

                                    // Kiểm tra và chuyển đổi dữ liệu đơn hàng nếu có
                                    if (jObject["Orders"] != null && jObject["Orders"].Type == JTokenType.Array)
                                    {
                                        foreach (var orderToken in jObject["Orders"])
                                        {
                                            var order = new Model.Order
                                            {
                                                InvoiceDate = orderToken["InvoiceDate"]?.ToString(),
                                                InvoiceNo = orderToken["InvoiceNo"]?.ToString(),
                                                CustomerId = orderToken["CustomerId"]?.ToString(),
                                                CustomerName = orderToken["CustomerName"]?.ToString(),
                                                Contact = orderToken["Contact"]?.ToString(),
                                                Address = orderToken["Address"]?.ToString(),
                                                GrandTotal = orderToken["GrandTotal"]?.ToObject<decimal>() ?? 0,
                                                OrderDetails = new List<Model.OrderDetail>()
                                            };

                                            // Chuyển đổi chi tiết đơn hàng
                                            if (orderToken["OrderDetails"] != null && orderToken["OrderDetails"].Type == JTokenType.Array)
                                            {
                                                foreach (var detailToken in orderToken["OrderDetails"])
                                                {
                                                    var detail = new Model.OrderDetail
                                                    {
                                                        ProductId = detailToken["ProductId"]?.ToString(),
                                                        ProductName = detailToken["ProductName"]?.ToString(),
                                                        Quantity = detailToken["Quantity"]?.ToObject<int>() ?? 0,
                                                        Price = detailToken["Price"]?.ToObject<decimal>() ?? 0,
                                                        Total = detailToken["Total"]?.ToObject<decimal>() ?? 0
                                                    };
                                                    order.OrderDetails.Add(detail);
                                                }
                                            }

                                            ordersData.Orders.Add(order);
                                        }
                                    }
                                }
                                catch
                                {
                                    // Nếu vẫn không đọc được, giữ nguyên cấu trúc mới tạo
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Nếu có lỗi khi đọc file, tạo mới cấu trúc
                        MessageBox.Show("Error reading orders file: " + ex.Message + ". Creating new file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        ordersData = new Model.OrdersData
                        {
                            Orders = new List<Model.Order>()
                        };
                    }
                }
                else
                {
                    // Nếu file không tồn tại, tạo mới cấu trúc
                    ordersData = new Model.OrdersData
                    {
                        Orders = new List<Model.Order>()
                    };
                }

                // Kiểm tra xem đơn hàng đã tồn tại hay chưa (dựa trên mã hóa đơn)
                var existingOrderIndex = -1;
                for (int i = 0; i < ordersData.Orders.Count; i++)
                {
                    if (ordersData.Orders[i].InvoiceNo == newOrder.InvoiceNo)
                    {
                        existingOrderIndex = i;
                        break;
                    }
                }

                if (existingOrderIndex >= 0)
                {
                    // Đơn hàng đã tồn tại, cập nhật nó
                    ordersData.Orders[existingOrderIndex] = newOrder;
                    MessageBox.Show("Invoice Updated Successfully...", "Thank You", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Đơn hàng mới, thêm vào danh sách
                    ordersData.Orders.Add(newOrder);
                    MessageBox.Show("Invoice Saved Successfully...", "Thank You", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Lưu file JSON
                string updatedJsonData = JsonConvert.SerializeObject(ordersData, Formatting.Indented);
                File.WriteAllText(ordersFilePath, updatedJsonData);

                // Cập nhật số lượng sản phẩm (giảm số lượng trong kho)
                UpdateProductQuantities(newOrder.OrderDetails);

                // Reset form sau khi lưu
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Phương thức cập nhật số lượng sản phẩm
        private void UpdateProductQuantities(List<Model.OrderDetail> orderDetails)
        {
            if (orderDetails == null || orderDetails.Count == 0)
                return;

            try
            {
                // Đường dẫn file JSON sản phẩm
                string productsFilePath = Path.Combine(Application.StartupPath, "Data", "products.json");

                // Kiểm tra file tồn tại
                if (!File.Exists(productsFilePath))
                {
                    MessageBox.Show("Products file not found. Quantities will not be updated.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Đọc nội dung file JSON
                string jsonData = File.ReadAllText(productsFilePath);
                JObject productData = JObject.Parse(jsonData);

                // Lấy danh sách sản phẩm
                if (productData["Products"] != null && productData["Products"].Type == JTokenType.Array)
                {
                    JArray productsArray = (JArray)productData["Products"];
                    bool isUpdated = false;

                    // Cập nhật số lượng sản phẩm
                    foreach (var detail in orderDetails)
                    {
                        for (int i = 0; i < productsArray.Count; i++)
                        {
                            var product = productsArray[i];
                            if (product["Id"] != null && product["Id"].ToString() == detail.ProductId)
                            {
                                // Lấy số lượng hiện tại
                                int currentQuantity = product["Quantity"]?.Value<int>() ?? 0;

                                // Giảm số lượng theo đơn hàng
                                int newQuantity = currentQuantity - detail.Quantity;
                                if (newQuantity < 0) newQuantity = 0;

                                // Cập nhật số lượng mới
                                product["Quantity"] = newQuantity;
                                isUpdated = true;
                                break;
                            }
                        }
                    }

                    // Lưu lại file nếu có cập nhật
                    if (isUpdated)
                    {
                        string updatedJsonData = productData.ToString(Formatting.Indented);
                        File.WriteAllText(productsFilePath, updatedJsonData);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product quantities: " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Phương thức để reset form sau khi lưu
        private void ResetForm()
        {
            // Làm mới tất cả các controls
            auto(); // Tạo mã hóa đơn mới

            // Clear tất cả các textbox
            txt_customername.Clear();
            txt_customerid.Clear();
            txt_contact.Clear();
            txt_address.Clear();
            txt_customerid.Text = "";

            // Clear tất cả các trường sản phẩm
            txt_productId.Clear();
            txt_productname.Clear();
            txt_productprice.Clear();
            txt_productquantity.Clear();
            txt_totalprice.Clear();

            // Reset ngày về ngày hiện tại
            dtp_invoicedate.Text = DateTime.Now.ToString("dd/MM/yyyy");

            // Làm mới DataGridView - Cách tốt nhất để đảm bảo xóa hoàn toàn dữ liệu
            if (db_dataGridView1 != null)
            {
                // Xóa DataSource trước
                db_dataGridView1.DataSource = null;

                // Xóa tất cả các hàng dữ liệu
                db_dataGridView1.Rows.Clear();

                // Tạo DataTable mới
                DataTable emptyTable = new DataTable();

                // Tạo các cột cần thiết
                emptyTable.Columns.Add("ProductID", typeof(string));
                emptyTable.Columns.Add("ProductName", typeof(string));
                emptyTable.Columns.Add("Price", typeof(decimal));
                emptyTable.Columns.Add("Quantity", typeof(int));
                emptyTable.Columns.Add("TotalPrice", typeof(decimal));

                // Gán DataTable mới làm DataSource
                db_dataGridView1.DataSource = emptyTable;
            }

            // Focus vào trường đầu tiên để sẵn sàng cho nhập liệu mới
            txt_customerid.Focus();
        }

        // Phương thức lấy số hóa đơn tiếp theo


        private void btn_update_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra các thông tin cần thiết
                if (string.IsNullOrWhiteSpace(txt_productname.Text))
                {
                    MessageBox.Show("Please select a product first", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txt_productquantity.Text))
                {
                    MessageBox.Show("Please enter quantity", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txt_productquantity.Focus();
                    return;
                }

                // Kiểm tra số lượng hợp lệ
                if (!int.TryParse(txt_productquantity.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Please enter a valid quantity (greater than 0)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_productquantity.Focus();
                    return;
                }

                // Kiểm tra DataGridView có dữ liệu không
                if (db_dataGridView1.DataSource == null || db_dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("No products in cart to update", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                bool productUpdated = false;
                DataTable cartTable = null;

                // Lấy DataTable từ DataSource
                if (db_dataGridView1.DataSource is DataTable)
                {
                    cartTable = (DataTable)db_dataGridView1.DataSource;
                }
                else if (db_dataGridView1.DataSource is BindingSource &&
                        ((BindingSource)db_dataGridView1.DataSource).DataSource is DataTable)
                {
                    cartTable = (DataTable)((BindingSource)db_dataGridView1.DataSource).DataSource;
                }

                if (cartTable != null)
                {
                    // Tìm sản phẩm trong DataTable theo tên sản phẩm
                    foreach (DataRow row in cartTable.Rows)
                    {
                        if (row["ProductName"].ToString() == txt_productname.Text)
                        {
                            // Lấy giá sản phẩm
                            decimal price = Convert.ToDecimal(row["Price"]);

                            // Cập nhật số lượng
                            row["Quantity"] = quantity;

                            // Cập nhật tổng giá
                            row["TotalPrice"] = price * quantity;

                            productUpdated = true;
                            break;
                        }
                    }
                }
                else
                {
                    // Nếu không thể lấy DataTable, tìm trực tiếp trong DataGridView
                    foreach (DataGridViewRow row in db_dataGridView1.Rows)
                    {
                        if (!row.IsNewRow && row.Cells["ProductName"].Value != null &&
                            row.Cells["ProductName"].Value.ToString() == txt_productname.Text)
                        {
                            // Lấy giá sản phẩm
                            decimal price = 0;
                            if (row.Cells["Price"].Value != null &&
                                decimal.TryParse(row.Cells["Price"].Value.ToString(), out price))
                            {
                                // Cập nhật số lượng
                                row.Cells["Quantity"].Value = quantity;

                                // Cập nhật tổng giá
                                row.Cells["TotalPrice"].Value = price * quantity;

                                productUpdated = true;
                                break;
                            }
                        }
                    }
                }

                if (productUpdated)
                {
                    // Cập nhật tổng giá giỏ hàng
                    CalculateCartTotal();

                    MessageBox.Show($"Product quantity updated to {quantity}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Xóa các trường để nhập sản phẩm mới
                    ClearProductFields();
                }
                else
                {
                    MessageBox.Show("Product not found in cart. Add it first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




        // Phương thức để lưu thông tin hóa đơn tạm thời (có thể lưu vào file JSON tạm thời)
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

        // Thêm phương thức này để xử lý sự kiện thay đổi trong DataGridView
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

        // Thêm phương thức này để xử lý khi người dùng xóa một hàng
        private void db_dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            // Cập nhật tổng giá sau khi xóa hàng
            CalculateCartTotal();
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

            // Tạo các cột cần thiết
            emptyTable.Columns.Add("ProductID", typeof(string));
            emptyTable.Columns.Add("ProductName", typeof(string));
            emptyTable.Columns.Add("Price", typeof(decimal));
            emptyTable.Columns.Add("Quantity", typeof(int));
            emptyTable.Columns.Add("TotalPrice", typeof(decimal));

            // Gán DataTable mới làm DataSource
            db_dataGridView1.DataSource = emptyTable;

            // Cập nhật tổng giá
            txt_totalprice.Text = "0.00";
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
                // Lưu ID hóa đơn hiện tại vào biến tĩnh để form hóa đơn có thể truy cập
                invoiceid = txt_invoiceno.Text;

                // Kiểm tra xem có sản phẩm nào trong giỏ hàng không
                if (db_dataGridView1.Rows.Count <= 1 || db_dataGridView1.DataSource == null)
                {
                    MessageBox.Show("No products in the cart to print invoice.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Tạo DataTable chứa thông tin của hóa đơn hiện tại
                DataTable invoiceData = new DataTable();
                invoiceData.Columns.Add("ProductID", typeof(string));
                invoiceData.Columns.Add("ProductName", typeof(string));
                invoiceData.Columns.Add("ProductQuantity", typeof(int));
                invoiceData.Columns.Add("ProductPrice", typeof(decimal));
                invoiceData.Columns.Add("TotalAmount", typeof(decimal));

                // Thêm dữ liệu từ giỏ hàng vào DataTable hóa đơn
                decimal grandTotal = 0;

                // Xác định vị trí của các cột trong DataGridView hoặc DataTable
                int idColumnIndex = -1;
                int nameColumnIndex = -1;
                int quantityColumnIndex = -1;
                int priceColumnIndex = -1;
                int totalPriceColumnIndex = -1;

                // Kiểm tra nguồn dữ liệu
                if (db_dataGridView1.DataSource is DataTable dataTable)
                {
                    // Nếu nguồn dữ liệu là DataTable, tìm kiếm các cột trong DataTable
                    for (int i = 0; i < dataTable.Columns.Count; i++)
                    {
                        string columnName = dataTable.Columns[i].ColumnName;

                        if (columnName.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                            columnName.Equals("ProductID", StringComparison.OrdinalIgnoreCase))
                        {
                            idColumnIndex = i;
                        }
                        else if (columnName.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("ProductName", StringComparison.OrdinalIgnoreCase))
                        {
                            nameColumnIndex = i;
                        }
                        else if (columnName.Equals("Quantity", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("Qty", StringComparison.OrdinalIgnoreCase))
                        {
                            quantityColumnIndex = i;
                        }
                        else if (columnName.Equals("Price", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("UnitPrice", StringComparison.OrdinalIgnoreCase))
                        {
                            priceColumnIndex = i;
                        }
                        else if (columnName.Equals("TotalPrice", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("Total", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("TotalAmount", StringComparison.OrdinalIgnoreCase))
                        {
                            totalPriceColumnIndex = i;
                        }
                    }

                    // Kiểm tra xem tìm thấy cột tên sản phẩm không
                    if (nameColumnIndex == -1)
                    {
                        // Nếu không tìm thấy cột tên sản phẩm, sử dụng cột đầu tiên hoặc tạo dữ liệu mẫu
                        foreach (DataRow row in dataTable.Rows)
                        {
                            // Lấy dữ liệu từ DataTable
                            string productId = (idColumnIndex >= 0 && row[idColumnIndex] != DBNull.Value)
                                ? row[idColumnIndex].ToString() : "N/A";

                            string productName = "Unknown Product"; // Vì không tìm thấy cột tên sản phẩm

                            int quantity = 1;
                            if (quantityColumnIndex >= 0 && row[quantityColumnIndex] != DBNull.Value)
                            {
                                int.TryParse(row[quantityColumnIndex].ToString(), out quantity);
                            }

                            decimal price = 0;
                            if (priceColumnIndex >= 0 && row[priceColumnIndex] != DBNull.Value)
                            {
                                decimal.TryParse(row[priceColumnIndex].ToString(), out price);
                            }

                            decimal totalPrice = 0;
                            if (totalPriceColumnIndex >= 0 && row[totalPriceColumnIndex] != DBNull.Value)
                            {
                                decimal.TryParse(row[totalPriceColumnIndex].ToString(), out totalPrice);
                            }
                            else
                            {
                                totalPrice = price * quantity;
                            }

                            // Thêm dữ liệu vào bảng hóa đơn
                            DataRow newRow = invoiceData.NewRow();
                            newRow["ProductID"] = productId;
                            newRow["ProductName"] = productName;
                            newRow["ProductQuantity"] = quantity;
                            newRow["ProductPrice"] = price;
                            newRow["TotalAmount"] = totalPrice;

                            invoiceData.Rows.Add(newRow);
                            grandTotal += totalPrice;
                        }
                    }
                    else
                    {
                        // Nếu tìm thấy cột tên sản phẩm, xử lý bình thường
                        foreach (DataRow row in dataTable.Rows)
                        {
                            string productId = (idColumnIndex >= 0 && row[idColumnIndex] != DBNull.Value)
                                ? row[idColumnIndex].ToString() : "N/A";

                            string productName = (nameColumnIndex >= 0 && row[nameColumnIndex] != DBNull.Value)
                                ? row[nameColumnIndex].ToString() : "Unknown Product";

                            int quantity = 1;
                            if (quantityColumnIndex >= 0 && row[quantityColumnIndex] != DBNull.Value)
                            {
                                int.TryParse(row[quantityColumnIndex].ToString(), out quantity);
                            }

                            decimal price = 0;
                            if (priceColumnIndex >= 0 && row[priceColumnIndex] != DBNull.Value)
                            {
                                decimal.TryParse(row[priceColumnIndex].ToString(), out price);
                            }

                            decimal totalPrice = 0;
                            if (totalPriceColumnIndex >= 0 && row[totalPriceColumnIndex] != DBNull.Value)
                            {
                                decimal.TryParse(row[totalPriceColumnIndex].ToString(), out totalPrice);
                            }
                            else
                            {
                                totalPrice = price * quantity;
                            }

                            // Thêm dữ liệu vào bảng hóa đơn
                            DataRow newRow = invoiceData.NewRow();
                            newRow["ProductID"] = productId;
                            newRow["ProductName"] = productName;
                            newRow["ProductQuantity"] = quantity;
                            newRow["ProductPrice"] = price;
                            newRow["TotalAmount"] = totalPrice;

                            invoiceData.Rows.Add(newRow);
                            grandTotal += totalPrice;
                        }
                    }
                }
                else
                {
                    // Nếu nguồn dữ liệu không phải DataTable, tìm kiếm trực tiếp trong DataGridView
                    for (int i = 0; i < db_dataGridView1.Columns.Count; i++)
                    {
                        string columnName = db_dataGridView1.Columns[i].Name;

                        if (columnName.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                            columnName.Equals("ProductID", StringComparison.OrdinalIgnoreCase))
                        {
                            idColumnIndex = i;
                        }
                        else if (columnName.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("ProductName", StringComparison.OrdinalIgnoreCase))
                        {
                            nameColumnIndex = i;
                        }
                        else if (columnName.Equals("Quantity", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("Qty", StringComparison.OrdinalIgnoreCase))
                        {
                            quantityColumnIndex = i;
                        }
                        else if (columnName.Equals("Price", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("UnitPrice", StringComparison.OrdinalIgnoreCase))
                        {
                            priceColumnIndex = i;
                        }
                        else if (columnName.Equals("TotalPrice", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("Total", StringComparison.OrdinalIgnoreCase) ||
                                columnName.Equals("TotalAmount", StringComparison.OrdinalIgnoreCase))
                        {
                            totalPriceColumnIndex = i;
                        }
                    }

                    // Xử lý trường hợp không tìm thấy cột tên sản phẩm
                    if (nameColumnIndex == -1 && db_dataGridView1.Columns.Count > 0)
                    {
                        // Sử dụng cột đầu tiên có dữ liệu làm cột tên sản phẩm
                        for (int i = 0; i < db_dataGridView1.Columns.Count; i++)
                        {
                            if (db_dataGridView1.Rows.Count > 0 &&
                                db_dataGridView1.Rows[0].Cells[i].Value != null &&
                                !string.IsNullOrEmpty(db_dataGridView1.Rows[0].Cells[i].Value.ToString()))
                            {
                                nameColumnIndex = i;
                                break;
                            }
                        }
                    }

                    // Duyệt qua các hàng trong DataGridView
                    foreach (DataGridViewRow row in db_dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            string productId = (idColumnIndex >= 0 && row.Cells[idColumnIndex].Value != null)
                                ? row.Cells[idColumnIndex].Value.ToString() : "N/A";

                            string productName = "Unknown Product";
                            if (nameColumnIndex >= 0 && row.Cells[nameColumnIndex].Value != null)
                            {
                                productName = row.Cells[nameColumnIndex].Value.ToString();
                            }

                            int quantity = 1;
                            if (quantityColumnIndex >= 0 && row.Cells[quantityColumnIndex].Value != null)
                            {
                                int.TryParse(row.Cells[quantityColumnIndex].Value.ToString(), out quantity);
                            }

                            decimal price = 0;
                            if (priceColumnIndex >= 0 && row.Cells[priceColumnIndex].Value != null)
                            {
                                decimal.TryParse(row.Cells[priceColumnIndex].Value.ToString(), out price);
                            }

                            decimal totalPrice = 0;
                            if (totalPriceColumnIndex >= 0 && row.Cells[totalPriceColumnIndex].Value != null)
                            {
                                decimal.TryParse(row.Cells[totalPriceColumnIndex].Value.ToString(), out totalPrice);
                            }
                            else
                            {
                                totalPrice = price * quantity;
                            }

                            // Thêm dữ liệu vào bảng hóa đơn
                            DataRow newRow = invoiceData.NewRow();
                            newRow["ProductID"] = productId;
                            newRow["ProductName"] = productName;
                            newRow["ProductQuantity"] = quantity;
                            newRow["ProductPrice"] = price;
                            newRow["TotalAmount"] = totalPrice;

                            invoiceData.Rows.Add(newRow);
                            grandTotal += totalPrice;
                        }
                    }
                }

                // Tạo đối tượng chứa thông tin hóa đơn để truyền sang form hóa đơn
                InvoiceData invoiceInfo = new InvoiceData()
                {
                    InvoiceNo = txt_invoiceno.Text,
                    InvoiceDate = dtp_invoicedate.Text,
                    CustomerName = txt_customername.Text,
                    CustomerContact = txt_contact.Text,
                    CustomerAddress = txt_address.Text,
                    GrandTotal = grandTotal,
                    Products = invoiceData
                };

                // Lưu thông tin hóa đơn vào cache để form hóa đơn có thể truy cập
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
