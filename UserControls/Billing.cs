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



namespace MarketManagement.UserControls
{
    public partial class Billing: UserControl
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
            for (A = 0; A < db_procardsDataGridView.Rows.Count; ++A)
            {
                B += Convert.ToInt32(db_procardsDataGridView.Rows[A].Cells[3].Value);
            }

            txt_grandtotal.Text = B.ToString();
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
                db_procardsDataGridView.DataSource = filteredProducts;
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

        //private void txt_customername_TextChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        // Nếu không có tên khách hàng, xóa thông tin liên quan
        //        if (string.IsNullOrWhiteSpace(txt_customername.Text))
        //        {
        //            txt_contact.Clear();
        //            txt_address.Clear();
        //            lbl_customerid.Text = "";
        //            return;
        //        }

        //        // Đường dẫn tới file JSON
        //        string jsonFilePath = Path.Combine(Application.StartupPath, "Data", "customers.json");

        //        // Kiểm tra file tồn tại
        //        if (!File.Exists(jsonFilePath))
        //        {
        //            MessageBox.Show("Customer data file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }

        //        // Đọc nội dung file JSON
        //        string jsonData = File.ReadAllText(jsonFilePath);

        //        // Chuyển đổi JSON thành danh sách khách hàng
        //        var jsonObject = JsonConvert.DeserializeObject<dynamic>(jsonData);
        //        List<Customer> customers = JsonConvert.DeserializeObject<List<Customer>>(jsonObject["customers"].ToString());

        //        // Tìm khách hàng theo tên (phân biệt hoa thường)
        //        var customer = customers.FirstOrDefault(c => c.cusname == txt_customername.Text);

        //        // Nếu tìm thấy khách hàng, cập nhật thông tin
        //        if (customer != null)
        //        {
        //            lbl_customerid.Text = customer.cusid;
        //            txt_contact.Text = customer.cuscontact;
        //            txt_address.Text = customer.cusaddress;
        //        }
        //        else
        //        {
        //            // Không tìm thấy, xóa thông tin
        //            lbl_customerid.Text = "";
        //            txt_contact.Clear();
        //            txt_address.Clear();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}


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
        private void dvgProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void txtProductID_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {

        }

        private void txt_invoiceno_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
    }
}
