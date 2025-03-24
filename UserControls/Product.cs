using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MarketManagement.UserControls;
using MarketManagement.Model;
using MarketManagement;

namespace MarketManagement.UseControl
{
    public partial class Product : System.Windows.Forms.UserControl

    {
        private ProductManager productManager;
        private BaseProduct currentProduct;

        // Thư - 20/03. Thêm vào đề xài trong Billing 
        public List<Product> Products { get; set; }
        private void LoadCategories()
        {
            cboCategory.DataSource = Enum.GetValues(typeof(ProductCategory));
        }
        public Product()
        {
            InitializeComponent();
            
            // Sử dụng Singleton Pattern
            productManager = ProductManager.Instance;
            
            // Đăng ký sự kiện ProductChanged
            productManager.ProductChanged += ProductManager_ProductChanged;
            
            // Khởi tạo danh sách kích thước cho quần áo
            chklst_sizes.Items.Clear();
            chklst_sizes.Items.Add("XS");
            chklst_sizes.Items.Add("S");
            chklst_sizes.Items.Add("M");
            chklst_sizes.Items.Add("L");
            chklst_sizes.Items.Add("XL");
            chklst_sizes.Items.Add("XXL");

            // Khởi tạo các tùy chọn lưu trữ cho thực phẩm
            cboStorage.Items.Clear();
            cboStorage.Items.Add("Room Temperature");
            cboStorage.Items.Add("Refrigerated");
            cboStorage.Items.Add("Frozen");
            
            LoadData();
            SetupDataGridView();
            LoadCategories(); // Load danh mục vào ComboBox
            
            // Ẩn tất cả các trường đặc biệt ban đầu
            HideAllSpecificFields();
            
            cboCategory.SelectedIndexChanged += cboCategory_SelectedIndexChanged;
        }
        private void SetupDataGridView()
        {
            // Đăng ký sự kiện khi người dùng chọn 1 hàng trong DataGridView
            dvgProduct.SelectionChanged += DvgProducts_SelectionChanged;
            btnRemoveProduct.Click += btnDeleteProduct_Click;
            btnAddProduct.Click += btnAddProduct_Click;
            btnUpdateProduct.Click += btnUpdateProduct_Click;
            btnUpdateProduct.Enabled = false;
            btnRemoveProduct.Enabled = false;
        }
        public void LoadData()
        {
            try
            {
                List<BaseProduct> products = productManager.GetAll();
                dvgProduct.DataSource = null;
                dvgProduct.DataSource = products;

                // Định dạng các cột
                foreach (DataGridViewColumn col in dvgProduct.Columns)
                {
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    
                    // Đổi tên cột cho dễ đọc
                    switch (col.Name)
                    {
                        case "Id":
                            col.HeaderText = "Product ID";
                            break;
                        case "ProductName":
                            col.HeaderText = "Product Name";
                            break;
                        case "Price":
                            col.HeaderText = "Price";
                            break;
                        case "Quantity":
                            col.HeaderText = "Quantity";
                            break;
                        case "Category":
                            col.HeaderText = "Category";
                            break;
                        case "Description":
                            col.HeaderText = "Description";
                            break;
                    }
                    
                    // Căn giữa nội dung cho các cột số liệu
                    if (col.Name != "Description" && col.Name != "ProductName")
                    {
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }

                // Thêm cột thông tin đặc biệt
                if (!dvgProduct.Columns.Contains("SpecificInfo"))
                {
                    DataGridViewTextBoxColumn specificInfoColumn = new DataGridViewTextBoxColumn();
                    specificInfoColumn.Name = "SpecificInfo";
                    specificInfoColumn.HeaderText = "Additional Info";
                    specificInfoColumn.ReadOnly = true;
                    dvgProduct.Columns.Add(specificInfoColumn);
                }

                // Điền thông tin đặc biệt vào cột mới
                foreach (DataGridViewRow row in dvgProduct.Rows)
                {
                    if (row.DataBoundItem is BaseProduct product)
                    {
                        row.Cells["SpecificInfo"].Value = GetSpecificInfo(product);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading products: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string GetSpecificInfo(BaseProduct product)
        {
            try
            {
                switch (product.Category)
                {
                    case ProductCategory.Food:
                        if (product is FoodProduct foodProduct)
                        {
                            return "Expiry: " + foodProduct.ExpiryDate.ToShortDateString() + 
                                   ", Storage: " + foodProduct.StorageCondition;
                        }
                        break;

                    case ProductCategory.Drink:
                        if (product is DrinkProduct drinkProduct)
                        {
                            return "Volume: " + drinkProduct.Volume + "ml, Alcoholic: " + 
                                   (drinkProduct.IsAlcoholic ? "Yes" : "No");
                        }
                        break;

                    case ProductCategory.Appliance:
                        if (product is ApplianceProduct applianceProduct)
                        {
                            return "Brand: " + applianceProduct.Brand + 
                                   ", Warranty: " + applianceProduct.WarrantyMonths + " months";
                        }
                        break;

                    case ProductCategory.Clothes:
                        if (product is ClothesProduct clothesProduct)
                        {
                            string sizes = "";
                            if (clothesProduct.AvailableSizes != null)
                            {
                                for (int i = 0; i < clothesProduct.AvailableSizes.Count; i++)
                                {
                                    sizes += clothesProduct.AvailableSizes[i];
                                    if (i < clothesProduct.AvailableSizes.Count - 1)
                                    {
                                        sizes += ", ";
                                    }
                                }
                            }
                            return "Sizes: " + sizes + ", Material: " + clothesProduct.Material;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                return "Error getting specific info";
            }
            
            return string.Empty;
        }
        private void DvgProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dvgProduct.SelectedRows.Count > 0)
            {
                // Lấy product được chọn
                currentProduct = dvgProduct.SelectedRows[0].DataBoundItem as BaseProduct;
                if (currentProduct != null)
                {
                    // Hiển thị thông tin lên các textbox
                    DisplayProductInfo(currentProduct);
                    // Enable nút Edit và Delete
                    btnUpdateProduct.Enabled = true;
                    btnRemoveProduct.Enabled = true;
                }
            }
            else
            {
                currentProduct = null;
                ClearInputs();
                btnUpdateProduct.Enabled = false;
                btnRemoveProduct.Enabled = false;
            }
        }
        private void DisplayProductInfo(BaseProduct product)
        {
            if (product != null)
            {
                txtProductID.Text = product.Id;
                txtProductName.Text = product.ProductName;
                txtProductQuantity.Text = product.Quantity.ToString();
                txtProductPrice.Text = product.Price.ToString();
                txtProductDiscription.Text = product.Description;
                cboCategory.SelectedItem = product.Category; // Gán trực tiếp enum vào ComboBox
                
                // Hiển thị các trường đặc biệt dựa trên loại sản phẩm
                ShowSpecificFieldsForCategory(product.Category);
            }
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            try
            {
            //    if (string.IsNullOrWhiteSpace(txtProductName.Text))
            //    {
            //        MessageBox.Show("Please enter a product name", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return;
            //    }

                BaseProduct product = GetProductFromInputs();
                if (product != null)
                {
                    if (currentProduct == null)
                    {
                        productManager.Add(product);
                        MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        productManager.Update(product);
                        MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    LoadData();
                    ClearInputs();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {


            // Enable các controls để sửa
            txtProductName.Enabled = true;
            txtProductQuantity.Enabled = true;
            txtProductPrice.Enabled = true;
            txtProductDiscription.Enabled = true;
            cboCategory.Enabled = true;
            btnAddProduct.Enabled = true;


        }
        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (currentProduct == null)
            {
                MessageBox.Show("Please select a product to delete!");
                return;
            }
            else
            {
                productManager.Remove(currentProduct.Id);
                LoadData();
                ClearInputs();
            }


        }
        private BaseProduct GetProductFromInputs()
        {
            try
            {
                // Lấy danh mục sản phẩm được chọn
                if (cboCategory.SelectedItem == null)
                {
                    MessageBox.Show("Please select a product category", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                ProductCategory selectedCategory = (ProductCategory)cboCategory.SelectedItem;
                BaseProduct product;
                
                // Tạo đối tượng sản phẩm phù hợp dựa trên danh mục
                switch (selectedCategory)
                {
                    case ProductCategory.Food:
                        FoodProduct foodProduct = new FoodProduct();
                        foodProduct.ExpiryDate = dtp_expiryDate.Value;
                        foodProduct.StorageCondition = cboStorage.SelectedItem?.ToString() ?? string.Empty;
                        product = foodProduct;
                        break;
                        
                    case ProductCategory.Drink:
                        DrinkProduct drinkProduct = new DrinkProduct();
                        drinkProduct.IsAlcoholic = chk_isAlcoholic.Checked;
                        drinkProduct.Volume = string.IsNullOrEmpty(txt_volume.Text) ? 0 : double.Parse(txt_volume.Text);
                        product = drinkProduct;
                        break;
                        
                    case ProductCategory.Appliance:
                        ApplianceProduct applianceProduct = new ApplianceProduct();
                        applianceProduct.Brand = txt_brand.Text;
                        applianceProduct.WarrantyMonths = string.IsNullOrEmpty(txt_warranty.Text) ? 0 : int.Parse(txt_warranty.Text);
                        product = applianceProduct;
                        break;
                        
                    case ProductCategory.Clothes:
                        ClothesProduct clothesProduct = new ClothesProduct();
                        clothesProduct.Material = txt_material.Text;
                        clothesProduct.AvailableSizes = new List<string>();
                        foreach (var item in chklst_sizes.CheckedItems)
                        {
                            clothesProduct.AvailableSizes.Add(item.ToString());
                        }
                        product = clothesProduct;
                        break;
                        
                    default:
                        product = new BaseProduct();
                        break;
                }
                
                // Thiết lập các thuộc tính cơ bản cho sản phẩm
                if (currentProduct != null)
                    product.Id = currentProduct.Id;
                else
                    product.Id = product.GenerateId();

                product.ProductName = txtProductName.Text;
                product.Quantity = int.Parse(txtProductQuantity.Text);
                product.Price = decimal.Parse(txtProductPrice.Text);
                product.Description = txtProductDiscription.Text;
                product.Category = selectedCategory;
                
                return product;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating product: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ẩn tất cả các trường đặc biệt
            HideAllSpecificFields();

            // Lấy danh mục được chọn
            if (cboCategory.SelectedItem != null)
            {
                if (Enum.TryParse(cboCategory.SelectedItem.ToString(), out ProductCategory selectedCategory))
                {
                    // Hiển thị các trường đặc biệt tương ứng với danh mục
                    ShowSpecificFieldsForCategory(selectedCategory);
                }
            }
        }
        private void HideAllSpecificFields()
        {
            // Ẩn tất cả các trường đặc biệt của FoodProduct
            lbl_expiryDate.Visible = false;
            dtp_expiryDate.Visible = false;
            lbl_storageCondition.Visible = false;
            cboStorage.Visible= false;

            // Ẩn tất cả các trường đặc biệt của DrinkProduct
             lbl_isAlcoholic.Visible = false;
             chk_isAlcoholic.Visible = false;
             lbl_volume.Visible = false;
             txt_volume.Visible = false;

            // Ẩn tất cả các trường đặc biệt của ApplianceProduct
            lbl_brand.Visible = false;
            txt_brand.Visible = false;
          
            lbl_warranty.Visible = false;
            txt_warranty.Visible = false;

            // Ẩn tất cả các trường đặc biệt của ClothesProduct
            lbl_sizes.Visible = false;
            chklst_sizes.Visible = false;
            txt_material.Visible = false;
            lbl_material.Visible = false;
        }
        private void ShowSpecificFieldsForCategory(ProductCategory category)
        {
            switch (category)
            {
                case ProductCategory.Food:
                    lbl_expiryDate.Visible = true;
                    dtp_expiryDate.Visible = true;
                    lbl_storageCondition.Visible = true;
                    cboStorage.Visible = true;
                    break;

                case ProductCategory.Drink:
                    lbl_isAlcoholic.Visible = true;
                    chk_isAlcoholic.Visible = true;
                    lbl_volume.Visible = true;
                    txt_volume.Visible = true;
                    break;

                case ProductCategory.Appliance:
                    lbl_brand.Visible = true;
                    txt_brand.Visible = true;
                    lbl_warranty.Visible = true;
                    txt_warranty.Visible = true;
                    break;

                case ProductCategory.Clothes:
                    lbl_sizes.Visible = true;
                    chklst_sizes.Visible = true;
                    txt_material.Visible = true;
                    lbl_material.Visible = true;
                    break;
            }
        }
        private void ClearInputs()
        {
            txtProductID.Clear();
            txtProductName.Clear();
            txtProductPrice.Clear();
            txtProductQuantity.Clear();
            txtProductDiscription.Clear();
            cboCategory.SelectedIndex = -1; // Reset ComboBox về trạng thái mặc định
            
            // Xóa dữ liệu trong các trường đặc biệt
            // Food
            dtp_expiryDate.Value = DateTime.Now;
            cboStorage.SelectedIndex = -1;
            
            // Drink
            chk_isAlcoholic.Checked = false;
            txt_volume.Clear();
            
            // Appliance
            txt_brand.Clear();
            txt_warranty.Clear();
            
            // Clothes
            for (int i = 0; i < chklst_sizes.Items.Count; i++)
            {
                chklst_sizes.SetItemChecked(i, false);
            }
            txt_material.Clear();
            
            // Ẩn tất cả các trường đặc biệt
            HideAllSpecificFields();
            
            currentProduct = null;
        }

        // Xử lý sự kiện khi có thay đổi trong ProductManager
        private void ProductManager_ProductChanged(object sender, EventArgs e)
        {
            // Cập nhật dữ liệu khi có thay đổi
            LoadData();
        }

        // Hủy đăng ký sự kiện khi UserControl bị disposed
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        // Hủy đăng ký sự kiện để tránh memory leak
        //        if (productManager != null)
        //        {
        //            productManager.ProductChanged -= ProductManager_ProductChanged;
        //        }
        //    }
        //    base.Dispose(disposing);
        //}
    }

}

