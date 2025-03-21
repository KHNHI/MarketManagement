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


namespace MarketManagement.UserControls
{
    public partial class Billing : UserControl
    {
        public static string invoiceid;
        public Billing()
        {
            InitializeComponent();
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
                                            txt_totalprice.Text = tprice.ToString("F2"); // Định dạng số với 2 chữ số thập phân
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
                ClearProductInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding product to cart: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // This function now calculates the total without updating txt_totalprice
        private void CalculateCartTotal()
        {
            decimal total = 0;

            // Check if DataGridView has a data source
            if (db_dataGridView1.DataSource != null)
            {
                DataTable cartTable;

                // Get the DataTable from the DataSource
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
                    // If we can't get the DataTable, fall back to iterating through rows
                    foreach (DataGridViewRow row in db_dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            if (row.Cells["Price"].Value != null && row.Cells["Quantity"].Value != null)
                            {
                                decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                                int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                                total += price * quantity;
                            }
                        }
                    }

                    txt_totalprice.Text = total.ToString("C2");
                    return;
                }

                // Calculate total from the DataTable by multiplying price by quantity
                if (cartTable.Columns.Contains("Price") && cartTable.Columns.Contains("Quantity"))
                {
                    foreach (DataRow row in cartTable.Rows)
                    {
                        if (row["Price"] != DBNull.Value && row["Quantity"] != DBNull.Value)
                        {
                            decimal price = Convert.ToDecimal(row["Price"]);
                            int quantity = Convert.ToInt32(row["Quantity"]);
                            total += price * quantity;
                        }
                    }
                }
            }
            else
            {
                // If no DataSource, iterate through grid rows directly
                foreach (DataGridViewRow row in db_dataGridView1.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        if (row.Cells["Price"].Value != null && row.Cells["Quantity"].Value != null)
                        {
                            decimal price = Convert.ToDecimal(row.Cells["Price"].Value);
                            int quantity = Convert.ToInt32(row.Cells["Quantity"].Value);
                            total += price * quantity;
                        }
                    }
                }
            }

            txt_totalprice.Text = total.ToString("C2");
        }

        // Phương thức xóa các ô nhập liệu
        private void ClearProductInputs()
        {
            txt_productname.Clear();
            txt_productprice.Clear();
            txt_productquantity.Clear();
            txt_totalprice.Clear();
            txt_productname.Focus();
        }



        private void btn_remove_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra các trường đầu vào không được để trống
                if (string.IsNullOrWhiteSpace(txt_invoiceno.Text))
                {
                    MessageBox.Show("Please, Enter Invoice No.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_invoiceno.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txt_productname.Text))
                {
                    MessageBox.Show("Please, Enter Product Name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_productname.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txt_productprice.Text))
                {
                    MessageBox.Show("Please, Enter Product Price.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_productprice.Focus();
                    return;
                }


                // Đường dẫn tới file JSON
                string jsonFilePath = Path.Combine(Application.StartupPath, "Data", "products.json");

                // Kiểm tra file tồn tại
                if (!File.Exists(jsonFilePath))
                {
                    MessageBox.Show("JSON file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Đọc nội dung file JSON
                string jsonData = File.ReadAllText(jsonFilePath);

                // Chuyển đổi JSON thành danh sách sản phẩm
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                List<BaseProduct> allProducts = JsonConvert.DeserializeObject<List<BaseProduct>>(jsonObject["products"].ToString());

                // Tìm và xóa sản phẩm phù hợp với invoiceno và productname
                int initialCount = allProducts.Count;
                allProducts.RemoveAll(p => p.Id == txt_invoiceno.Text && p.ProductName == txt_productname.Text);

                // Kiểm tra xem có sản phẩm nào bị xóa không
                if (allProducts.Count == initialCount)
                {
                    MessageBox.Show("Product not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Cập nhật lại JSON
                jsonObject["products"] = JArray.FromObject(allProducts);
                File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(jsonObject, Formatting.Indented));

                // Cập nhật giao diện
                showdata(); // Hiển thị lại danh sách sản phẩm

                // Xóa nội dung các textbox
                txt_productname.Clear();
                txt_productprice.Clear();
                txt_productquantity.Clear();

                MessageBox.Show("Product removed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btn_check_customer_Click(object sender, EventArgs e)
        {
            LookupCustomer(txt_customerid.Text);
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
            LookupProduct(txt_productId.Text);
        }

        private void LookupProduct(string productId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(productId))
                {
                    txt_productname.Clear();
                    txt_productprice.Clear();
                    txt_productquantity.Clear();
                    return;
                }

                string filePath = Path.Combine(Application.StartupPath, "Data", "products.json");

                if (!File.Exists(filePath))
                {
                    MessageBox.Show("Không tìm thấy file: " + filePath, "Lỗi");
                    return;
                }

                string jsonData = File.ReadAllText(filePath);

                using (JsonDocument doc = JsonDocument.Parse(jsonData))
                {
                    JsonElement root = doc.RootElement;

                    if (root.TryGetProperty("Products", out JsonElement productsElement) &&
                        productsElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (JsonElement product in productsElement.EnumerateArray())
                        {
                            if (product.TryGetProperty("Id", out JsonElement idElement) &&
                                idElement.GetString().Equals(productId, StringComparison.OrdinalIgnoreCase))
                            {
                                // Tìm thấy sản phẩm
                                if (product.TryGetProperty("ProductName", out JsonElement nameElement))
                                    txt_productname.Text = nameElement.GetString();

                                if (product.TryGetProperty("Price", out JsonElement priceElement))
                                {
                                    if (priceElement.ValueKind == JsonValueKind.Number)
                                        txt_productprice.Text = priceElement.GetDouble().ToString();
                                    else
                                        txt_productprice.Text = priceElement.GetString();
                                }

                                MessageBox.Show("Đã tìm thấy sản phẩm: " + txt_productname.Text, "Thông báo");
                                return;
                            }
                        }

                        // Không tìm thấy sản phẩm
                        txt_productname.Clear();
                        txt_productprice.Clear();
                        txt_productquantity.Clear();
                        MessageBox.Show("Không tìm thấy sản phẩm với ID: " + productId, "Thông báo");
                    }
                    else
                    {
                        MessageBox.Show("Cấu trúc JSON không chứa mảng Products", "Lỗi");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Lỗi");
            }
        }

        private void txt_productId_TextChanged(object sender, EventArgs e)
        {
            LookupProduct(txt_productId.Text);
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

                // Parse the total price without currency symbols
                string totalPriceText = txt_totalprice.Text.Replace("$", "").Replace(",", "");
                decimal grandTotal;
                if (!decimal.TryParse(totalPriceText, out grandTotal))
                {
                    // If parsing fails, try to use the original string
                    decimal.TryParse(txt_totalprice.Text, out grandTotal);
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
                    foreach (DataGridViewRow row in db_dataGridView1.Rows)
                    {
                        if (!row.IsNewRow && row.Cells[0].Value != null)
                        {
                            // Biến để lưu trữ dữ liệu
                            decimal unitPrice = 0;
                            int quantity = 0;
                            string productId = "";
                            string productName = "";
                            decimal total = 0;

                            // Tìm và lấy dữ liệu từ các cột
                            for (int i = 0; i < row.Cells.Count; i++)
                            {
                                if (row.Cells[i].Value == null) continue;

                                string columnName = db_dataGridView1.Columns[i].Name;

                                // Kiểm tra ID/ProductID
                                if (columnName.Equals("Id", StringComparison.OrdinalIgnoreCase) ||
                                    columnName.Equals("ProductID", StringComparison.OrdinalIgnoreCase))
                                {
                                    productId = row.Cells[i].Value.ToString();
                                }
                                // Kiểm tra Name/ProductName
                                else if (columnName.Equals("Name", StringComparison.OrdinalIgnoreCase) ||
                                        columnName.Equals("ProductName", StringComparison.OrdinalIgnoreCase))
                                {
                                    productName = row.Cells[i].Value.ToString();
                                }
                                // Kiểm tra Price
                                else if (columnName.Equals("Price", StringComparison.OrdinalIgnoreCase))
                                {
                                    decimal.TryParse(row.Cells[i].Value.ToString(), out unitPrice);
                                }
                                // Kiểm tra Quantity
                                else if (columnName.Equals("Quantity", StringComparison.OrdinalIgnoreCase) ||
                                        columnName.Equals("Qty", StringComparison.OrdinalIgnoreCase))
                                {
                                    int.TryParse(row.Cells[i].Value.ToString(), out quantity);
                                }
                                // Kiểm tra TotalPrice
                                else if (columnName.Equals("TotalPrice", StringComparison.OrdinalIgnoreCase) ||
                                        columnName.Equals("Total", StringComparison.OrdinalIgnoreCase))
                                {
                                    decimal.TryParse(row.Cells[i].Value.ToString(), out total);
                                }
                            }

                            // Nếu không tìm thấy dữ liệu theo tên cột, thử lấy theo vị trí
                            if (string.IsNullOrEmpty(productId) && row.Cells.Count > 0 && row.Cells[0].Value != null)
                            {
                                productId = row.Cells[0].Value.ToString();
                            }

                            if (string.IsNullOrEmpty(productName) && row.Cells.Count > 1 && row.Cells[1].Value != null)
                            {
                                productName = row.Cells[1].Value.ToString();
                            }

                            if (unitPrice == 0 && row.Cells.Count > 2 && row.Cells[2].Value != null)
                            {
                                decimal.TryParse(row.Cells[2].Value.ToString(), out unitPrice);
                            }

                            if (quantity == 0 && row.Cells.Count > 3 && row.Cells[3].Value != null)
                            {
                                int.TryParse(row.Cells[3].Value.ToString(), out quantity);
                            }

                            // Đảm bảo số lượng là ít nhất 1
                            if (quantity <= 0) quantity = 1;

                            // Nếu tổng giá trị chưa được tính, tính lại
                            if (total == 0)
                            {
                                total = unitPrice * quantity;
                            }

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
        private string GetNextInvoiceNumber()
        {
            try
            {
                string ordersFilePath = Path.Combine(Application.StartupPath, "Data", "orders.json");
                if (File.Exists(ordersFilePath))
                {
                    string jsonData = File.ReadAllText(ordersFilePath);
                    var ordersData = JsonConvert.DeserializeObject<OrdersData>(jsonData);

                    // Tìm số hóa đơn lớn nhất và tăng lên 1
                    if (ordersData?.Orders != null && ordersData.Orders.Count > 0)
                    {
                        // Giả sử định dạng hóa đơn là "INV-XXXXXX"
                        var maxInvoice = ordersData.Orders
                            .Select(o => o.InvoiceNo)
                            .Where(i => i.StartsWith("INV-"))
                            .Select(i => int.TryParse(i.Substring(4), out int num) ? num : 0)
                            .DefaultIfEmpty(0)
                            .Max();

                        return $"INV-{(maxInvoice + 1):D6}";
                    }
                }

                // Nếu không có file hoặc không có hóa đơn nào
                return "INV-000001";
            }
            catch
            {
                return "INV-" + DateTime.Now.ToString("yyMMddHHmmss");
            }
        }


        private void Clearall()
        {
            txt_invoiceno.Clear();
            txt_customername.Clear();
            txt_address.Clear();
            txt_contact.Clear();
            txt_productname.Clear();
            txt_productprice.Clear();
            txt_productquantity.Clear();
            txt_totalprice.Clear();
            txt_totalprice.Clear();
            showdata();
        }



        private void btn_update_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txt_invoiceno.Text))
                {
                    MessageBox.Show("Please, Enter Invoice No.", "Try Again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_invoiceno.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txt_customername.Text))
                {
                    MessageBox.Show("Please, Enter Customer Name", "Try Again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_customername.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txt_contact.Text))
                {
                    MessageBox.Show("Please, Enter Customer Contact", "Try Again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_contact.Focus();
                    return;
                }
                if (string.IsNullOrWhiteSpace(txt_address.Text))
                {
                    MessageBox.Show("Please, Enter Customer Address", "Try Again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (string.IsNullOrWhiteSpace(txt_totalprice.Text))
                {
                    MessageBox.Show("Please, Enter Product Grand Total", "Try Again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_totalprice.Focus();
                    return;
                }

                var invoiceData = new
                {
                    invodate = dtp_invoicedate.Text,
                    cusname = txt_customername.Text,
                    contact = txt_contact.Text,
                    address = txt_address.Text,
                    grandtotal = txt_totalprice.Text,
                    invono = txt_invoiceno.Text,
                    cusid = txt_customerid.Text
                };

                string jsonString = System.Text.Json.JsonSerializer.Serialize(invoiceData, new JsonSerializerOptions { WriteIndented = true });

                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "invoices.json");

                File.WriteAllText(filePath, jsonString);

                MessageBox.Show("Invoice Updated Successfully and Saved to JSON", "Thank You", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Clearall();
                auto();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btn_remove__Click(object sender, EventArgs e)
        {
            try
            {
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

                // Đọc dữ liệu từ file JSON
                string jsonFilePath = Path.Combine(Application.StartupPath, "orders.json");
                OrdersData ordersData = new OrdersData { Orders = new List<MarketManagement.Model.Order>() };


                if (File.Exists(jsonFilePath))
                {
                    string jsonData = File.ReadAllText(jsonFilePath);
                    ordersData = JsonConvert.DeserializeObject<OrdersData>(jsonData) ?? new OrdersData { Orders = new List<MarketManagement.Model.Order>() };
                }

                // Tìm và xóa đơn hàng theo invoiceno và cusid
                ordersData.Orders.RemoveAll(order =>
                    order.InvoiceNo == txt_invoiceno.Text &&
                    order.CustomerId == txt_customerid.Text);

                // Ghi lại file JSON
                string updatedJson = JsonConvert.SerializeObject(ordersData, Formatting.Indented);
                File.WriteAllText(jsonFilePath, updatedJson);

                // Hiển thị thông báo thành công
                Clearall();
                MessageBox.Show("Invoice Deleted Successfully...", "Thank You", MessageBoxButtons.OK, MessageBoxIcon.Information);
                auto();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //private void loadinvoice()
        //{
        //    invoiceid = txt_invoiceno.Text;
        //    frm_invoice fi = new frm_invoice();
        //    fi.ShowDialog();
        //}
        //private void btn_print_Click(object sender, EventArgs e)
        //{
        //    loadinvoice();
        //    Clearall();
        //}


        private void db_procardsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;

            if (index >= 0 && index < db_dataGridView1.Rows.Count)
            {
                DataGridViewRow dgvr = db_dataGridView1.Rows[index];

                if (dgvr.Cells[1].Value != null)
                    txt_productname.Text = dgvr.Cells[1].Value.ToString();
                if (dgvr.Cells[2].Value != null)
                    txt_productprice.Text = dgvr.Cells[2].Value.ToString();
                if (dgvr.Cells[3].Value != null)
                    txt_productquantity.Text = dgvr.Cells[3].Value.ToString();
            }
        }


    }
}
