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
        private void sum()
        {
            int A = 0, B = 0;
            for (A = 0; A < db_dataGridView1.Rows.Count; ++A)
            {
                B += Convert.ToInt32(db_dataGridView1.Rows[A].Cells[3].Value);
            }

            txt_totalprice.Text = B.ToString();
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
        private void btn_addtocard_Click(object sender, EventArgs e)
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
                if (string.IsNullOrWhiteSpace(txt_productquantity.Text))
                {
                    MessageBox.Show("Please, Enter Product Quantity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_productquantity.Focus();
                    return;
                }

                // Đường dẫn tới file JSON
                string jsonFilePath = Path.Combine(Application.StartupPath, "Data", "products.json");

                // Kiểm tra file tồn tại, nếu không có thì tạo file mới
                if (!File.Exists(jsonFilePath))
                {
                    File.WriteAllText(jsonFilePath, "{\"products\":[]}");
                }

                // Đọc nội dung file JSON
                string jsonData = File.ReadAllText(jsonFilePath);

                // Chuyển đổi JSON thành đối tượng
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                List<BaseProduct> allProducts = JsonConvert.DeserializeObject<List<BaseProduct>>(jsonObject["products"].ToString());

                // Tạo sản phẩm mới
                BaseProduct newProduct = new BaseProduct
                {
                    Id = txt_invoiceno.Text,
                    ProductName = txt_productname.Text,
                    Quantity = int.Parse(txt_productquantity.Text),
                    Price = decimal.Parse(txt_productprice.Text),
                    Description = "No description", // Có thể thay đổi tùy ý
                    Category = "Uncategorized" // Có thể thay đổi tùy ý
                };

                // Thêm sản phẩm mới vào danh sách
                allProducts.Add(newProduct);

                // Cập nhật lại JSON
                jsonObject["products"] = JArray.FromObject(allProducts);
                File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(jsonObject, Formatting.Indented));

                // Cập nhật giao diện
                showdata();  // Hiển thị lại danh sách sản phẩm
                sum();       // Cập nhật tổng giá trị

                // Xóa nội dung các textbox
                txt_productname.Clear();
                txt_productprice.Clear();
                txt_productquantity.Clear();

                MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                if (string.IsNullOrWhiteSpace(txt_productquantity.Text))
                {
                    MessageBox.Show("Please, Enter Product Quantity.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_productquantity.Focus();
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

        private void txt_customername_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // Nếu không có tên khách hàng, xóa thông tin liên quan
                if (string.IsNullOrWhiteSpace(txt_customername.Text))
                {
                    txt_contact.Clear();
                    txt_address.Clear();
                    lbl_customerid.Text = "";
                    return;
                }

                // Đường dẫn tới file JSON
                string jsonFilePath = Path.Combine(Application.StartupPath, "Data", "customers.json");

                // Kiểm tra file tồn tại
                if (!File.Exists(jsonFilePath))
                {
                    MessageBox.Show("Customer data file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Đọc nội dung file JSON
                string jsonData = File.ReadAllText(jsonFilePath);

                // Chuyển đổi JSON thành danh sách khách hàng
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
                List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(jsonObject["customers"].ToString());

                // Tìm khách hàng theo tên (phân biệt hoa thường)
                var customer = customers.FirstOrDefault(c => c.CustomerName == txt_customername.Text);

                // Nếu tìm thấy khách hàng, cập nhật thông tin
                if (customer != null)
                {
                    //  lbl_customerid.Text = customer.cusid;
                    txt_contact.Text = customer.PhoneNumber;
                    txt_address.Text = customer.Address;
                }
                else
                {
                    // Không tìm thấy, xóa thông tin
                    lbl_customerid.Text = "";
                    txt_contact.Clear();
                    txt_address.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //private void txt_productId_TextChanged(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrWhiteSpace(txt_productId.Text))
        //    {
        //        txt_productname.Clear();
        //        txt_productprice.Clear();
        //        txt_productquantity.Clear();
        //    }
        //    else
        //    {
        //        try
        //        {
        //            // Đọc file JSON
        //            string jsonData = File.ReadAllText("products.json");

        //            // Chuyển đổi JSON thành danh sách sản phẩm
        //            var productData = JsonConvert.DeserializeObject<BaseProduct>(jsonData);

        //            // Tìm sản phẩm theo ID
        //            var product = productData.Products.FirstOrDefault(p =>
        //                p.Id.Equals(txt_productId.Text, StringComparison.OrdinalIgnoreCase));

        //            if (product != null)
        //            {
        //                txt_productname.Text = product.ProductName;
        //                txt_productprice.Text = product.Price.ToString();
        //                txt_productquantity.Text = product.Quantity.ToString();
        //            }
        //            else
        //            {
        //                // Không tìm thấy sản phẩm
        //                txt_productname.Clear();
        //                txt_productprice.Clear();
        //                txt_productquantity.Clear();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Error loading product data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}

        //private void txt_productId_TextChanged(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrWhiteSpace(txt_productId.Text))
        //    {
        //        txt_productname.Clear();
        //        txt_productprice.Clear();
        //        txt_productquantity.Clear();
        //    }
        //    else
        //    {
        //        try
        //        {
        //            // Đường dẫn đến file JSON
        //            string filePath = Path.Combine(Application.StartupPath, "Data", "products.json");
        //            if (!File.Exists(filePath))
        //            {
        //                MessageBox.Show("Không tìm thấy file products.json", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                return;
        //            }

        //            // Đọc nội dung file
        //            string jsonData = File.ReadAllText(filePath);

        //            // Sử dụng JsonDocument để phân tích JSON
        //            using (JsonDocument doc = JsonDocument.Parse(jsonData))
        //            {
        //                JsonElement root = doc.RootElement;

        //                // Kiểm tra nếu root có thuộc tính "Products"
        //                if (root.TryGetProperty("Products", out JsonElement productsElement) &&
        //                    productsElement.ValueKind == JsonValueKind.Array)
        //                {
        //                    bool productFound = false;

        //                    // Tìm sản phẩm theo ID
        //                    foreach (JsonElement product in productsElement.EnumerateArray())
        //                    {
        //                        if (product.TryGetProperty("Id", out JsonElement idElement) &&
        //                            idElement.GetString().Equals(txt_productId.Text, StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            productFound = true;

        //                            // Lấy thông tin sản phẩm
        //                            if (product.TryGetProperty("ProductName", out JsonElement nameElement))
        //                                txt_productname.Text = nameElement.GetString();
        //                            else
        //                                txt_productname.Clear();

        //                            if (product.TryGetProperty("Price", out JsonElement priceElement))
        //                                txt_productprice.Text = priceElement.GetDouble().ToString();
        //                            else
        //                                txt_productprice.Clear();

        //                            if (product.TryGetProperty("Quantity", out JsonElement quantityElement))
        //                                txt_productquantity.Text = quantityElement.GetInt32().ToString();
        //                            else
        //                                txt_productquantity.Clear();

        //                            break;
        //                        }
        //                    }

        //                    // Nếu không tìm thấy sản phẩm, xóa các trường
        //                    if (!productFound)
        //                    {
        //                        txt_productname.Clear();
        //                        txt_productprice.Clear();
        //                        txt_productquantity.Clear();
        //                    }
        //                }
        //                else
        //                {
        //                    MessageBox.Show("File JSON không có cấu trúc Products mong đợi", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Lỗi khi tải dữ liệu sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}

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

                                if (product.TryGetProperty("Quantity", out JsonElement quantityElement))
                                {
                                    if (quantityElement.ValueKind == JsonValueKind.Number)
                                        txt_productquantity.Text = quantityElement.GetInt32().ToString();
                                    else
                                        txt_productquantity.Text = quantityElement.GetString();
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
                    // Kiểm tra xem số lượng nhập vào có vượt quá số lượng trong kho không
                    string jsonData = File.ReadAllText("products.json");
                    var productData = JsonConvert.DeserializeObject<BaseProduct>(jsonData);
                    var product = productData.Products.FirstOrDefault(p =>
                        p.ProductName.Equals(txt_productname.Text, StringComparison.OrdinalIgnoreCase));

                    if (product != null)
                    {
                        int requestedQuantity = int.Parse(txt_productquantity.Text);

                        if (requestedQuantity > product.Quantity)
                        {
                            MessageBox.Show($"Available quantity is only {product.Quantity}", "Quantity Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            txt_productquantity.Text = product.Quantity.ToString();
                            requestedQuantity = product.Quantity;
                        }

                        // Tính tổng giá
                        decimal tprice = product.Price * requestedQuantity;
                        txt_totalprice.Text = tprice.ToString("F2"); // Định dạng số với 2 chữ số thập phân
                    }
                    else
                    {
                        // Nếu không tìm thấy sản phẩm trong JSON (trường hợp hiếm gặp vì đã kiểm tra ở bước trước)
                        MessageBox.Show("Product not found in database", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        txt_totalprice.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating price: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btn_save_Click(object sender, EventArgs e)
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
                if (string.IsNullOrEmpty(txt_totalprice.Text))
                {
                    MessageBox.Show("Please, Enter Product Grand Total", "Try Again", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txt_totalprice.Focus();
                    return;
                }

                // Tạo đối tượng đơn hàng mới
                var newOrder = new Model.Order
                {
                    InvoiceDate = dtp_invoicedate.Text,
                    InvoiceNo = txt_invoiceno.Text,
                    CustomerId = lbl_customerid.Text,
                    CustomerName = txt_customername.Text,
                    Contact = txt_contact.Text,
                    Address = txt_address.Text,
                    GrandTotal = decimal.Parse(txt_totalprice.Text)
                };

                // Thêm chi tiết sản phẩm vào đơn hàng (nếu có DataGridView cho chi tiết đơn hàng)
                if (db_dataGridView1 != null && db_dataGridView1.Rows.Count > 0)
                {
                    newOrder.OrderDetails = new List<OrderDetail>();
                    foreach (DataGridViewRow row in db_dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            var detail = new OrderDetail
                            {
                                ProductId = row.Cells["ProductId"].Value?.ToString(),
                                ProductName = row.Cells["ProductName"].Value?.ToString(),
                                Quantity = int.Parse(row.Cells["Quantity"].Value?.ToString() ?? "0"),
                                Price = decimal.Parse(row.Cells["Price"].Value?.ToString() ?? "0"),
                                Total = decimal.Parse(row.Cells["Total"].Value?.ToString() ?? "0")
                            };
                            newOrder.OrderDetails.Add(detail);
                        }
                    }
                }

                // Đọc file JSON hiện có
                string ordersFilePath = "orders.json";
                OrdersData ordersData;

                if (File.Exists(ordersFilePath))
                {
                    string jsonData = File.ReadAllText(ordersFilePath);

                    ordersData = JsonConvert.DeserializeObject<MarketManagement.Model.OrdersData>(jsonData) ??
             new MarketManagement.Model.OrdersData
             {
                 Orders = new List<MarketManagement.Model.Order>()
             };
                }
                else
                {
                    ordersData = new MarketManagement.Model.OrdersData
                    {
                        Orders = new List<MarketManagement.Model.Order>()
                    };
                }

                // Thêm đơn hàng mới
                ordersData.Orders.Add(newOrder);

                // Lưu file JSON
                string updatedJsonData = JsonConvert.SerializeObject(ordersData, Formatting.Indented);
                File.WriteAllText(ordersFilePath, updatedJsonData);

                // Cập nhật số lượng sản phẩm (giảm số lượng trong kho)
                UpdateProductQuantities(newOrder.OrderDetails);

                MessageBox.Show("Invoice Saved Successfully...", "Thank You", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ResetForm(); // Phương thức để reset form sau khi lưu
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        // Phương thức cập nhật số lượng sản phẩm
        private void UpdateProductQuantities(List<OrderDetail> orderDetails)
        {
            if (orderDetails == null || orderDetails.Count == 0)
                return;

            try
            {
                // Đọc file JSON sản phẩm
                string productsFilePath = "products.json";
                string jsonData = File.ReadAllText(productsFilePath);
                var productData = JsonConvert.DeserializeObject<BaseProduct>(jsonData);

                // Cập nhật số lượng
                bool isUpdated = false;
                foreach (var detail in orderDetails)
                {
                    var product = productData.Products.FirstOrDefault(p => p.Id == detail.ProductId);
                    if (product != null)
                    {
                        product.Quantity -= detail.Quantity;
                        isUpdated = true;
                    }
                }

                // Lưu lại file nếu có cập nhật
                if (isUpdated)
                {
                    string updatedJsonData = JsonConvert.SerializeObject(productData, Formatting.Indented);
                    File.WriteAllText(productsFilePath, updatedJsonData);
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
            // Làm mới các controls
            txt_invoiceno.Text = GetNextInvoiceNumber(); // Phương thức để lấy số hóa đơn tiếp theo
            txt_customername.Clear();
            txt_contact.Clear();
            txt_address.Clear();
            lbl_customerid.Text = "";
            txt_totalprice.Clear();

            // Làm mới DataGridView nếu có
            if (db_dataGridView1 != null)
            {
                db_dataGridView1.Rows.Clear();
            }
        }

        // Phương thức lấy số hóa đơn tiếp theo
        private string GetNextInvoiceNumber()
        {
            try
            {
                string ordersFilePath = "orders.json";
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
                    cusid = lbl_customerid.Text
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
                    order.CustomerId == lbl_customerid.Text);

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
        // Class to match your database structure
        public class ProCard
        {
            public string invoid { get; set; }
            // Add other properties that match your SQL table columns
            // For example:
            // public string product_name { get; set; }
            // public decimal price { get; set; }
            // public int quantity { get; set; }
            // etc.
        }

       
    }
}
