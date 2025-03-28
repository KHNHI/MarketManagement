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
            
            // Đăng ký sự kiện Load của UserControl
            this.Load += Product_Load;
            
            SetupDataGridView();
            
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
            
            LoadCategories(); // Load danh mục vào ComboBox
            
            // Ẩn tất cả các trường đặc biệt ban đầu
            HideAllSpecificFields();
            
            cboCategory.SelectedIndexChanged += cboCategory_SelectedIndexChanged;
            
            // Clear all inputs initially
            ClearInputs();
        }

        private void Product_Load(object sender, EventArgs e)
        {
            LoadData();
            if (dvgProduct.Rows.Count > 0)
            {
                dvgProduct.ClearSelection();
                dvgProduct.CurrentCell = null;
            }
        }

        private void SetupDataGridView()
        {
            // Prevent auto selection of first row
            dvgProduct.MultiSelect = false;
            dvgProduct.AllowUserToAddRows = false;
            dvgProduct.EnableHeadersVisualStyles = false;
            dvgProduct.ReadOnly = true;
            dvgProduct.AutoGenerateColumns = false;  // Set to false to manually define columns
            dvgProduct.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            
            // Thêm thuộc tính này để ngăn tự động chọn
            dvgProduct.TabStop = false;

            // Clear existing columns
            dvgProduct.Columns.Clear();

            // Add basic columns that all products have
            dvgProduct.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Id",
                Name = "Id",
                HeaderText = "ID",
                Width = 80
            });

            dvgProduct.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductName",
                Name = "ProductName",
                HeaderText = "Product Name",
                Width = 150
            });

            dvgProduct.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Price",
                Name = "Price",
                HeaderText = "Price",
                Width = 100,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });

            dvgProduct.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Quantity",
                Name = "Quantity",
                HeaderText = "Quantity",
                Width = 80
            });

            dvgProduct.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Category",
                Name = "Category",
                HeaderText = "Category",
                Width = 100
            });

            dvgProduct.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AdditionalInfo",
                HeaderText = "Additional Information",
                Width = 250
            });

            dvgProduct.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Description",
                Name = "Description",
                HeaderText = "Description",
                Width = 200
            });
            
            // Đăng ký sự kiện khi người dùng chọn 1 hàng trong DataGridView
            dvgProduct.SelectionChanged += DvgProducts_SelectionChanged;
            dvgProduct.DataBindingComplete += DvgProduct_DataBindingComplete;
            
            btnRemoveProduct.Click += btnDeleteProduct_Click;
            btnAddProduct.Click += btnAddProduct_Click;
            btnUpdateProduct.Click += btnUpdateProduct_Click;
            btnUpdateProduct.Enabled = false;
            btnRemoveProduct.Enabled = false;

            // Add CellFormatting event handler
            dvgProduct.CellFormatting += DvgProduct_CellFormatting;
        }

        private void DvgProduct_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            try
            {
                // Cập nhật cột Additional Information cho mỗi hàng
                foreach (DataGridViewRow row in dvgProduct.Rows)
                {
                    BaseProduct product = row.DataBoundItem as BaseProduct;
                    if (product != null)
                    {
                        string additionalInfo = "";
                        switch (product.Category)
                        {
                            case ProductCategory.Food:
                                if (product is FoodProduct food)
                                {
                                    additionalInfo = $"Expiry: {food.ExpiryDate:dd/MM/yyyy}, Storage: {food.StorageCondition}";
                                }
                                break;
                            case ProductCategory.Drink:
                                if (product is DrinkProduct drink)
                                {
                                    additionalInfo = $"Alcoholic: {(drink.IsAlcoholic ? "Yes" : "No")}, Volume: {drink.Volume}ml";
                                }
                                break;
                            case ProductCategory.Appliance:
                                if (product is ApplianceProduct appliance)
                                {
                                    additionalInfo = $"Brand: {appliance.Brand}, Warranty: {appliance.WarrantyMonths} months";
                                }
                                break;
                            case ProductCategory.Clothes:
                                if (product is ClothesProduct clothes)
                                {
                                    additionalInfo = $"Material: {clothes.Material}, Sizes: {string.Join(", ", clothes.AvailableSizes)}";
                                }
                                break;
                        }
                        row.Cells["AdditionalInfo"].Value = additionalInfo;
                    }
                }

                // Clear selection
                dvgProduct.ClearSelection();
                dvgProduct.CurrentCell = null;
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
        }

        private void DvgProduct_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                // Format Additional Information
                if (dvgProduct.Columns[e.ColumnIndex].Name == "AdditionalInfo")
                {
                    if (e.RowIndex >= 0 && e.RowIndex < dvgProduct.Rows.Count)  // Check valid row index
                    {
                        DataGridViewRow row = dvgProduct.Rows[e.RowIndex];
                        BaseProduct product = row.DataBoundItem as BaseProduct;
                        if (product != null)
                        {
                            string additionalInfo = "";
                            switch (product.Category)
                            {
                                case ProductCategory.Food:
                                    if (product is FoodProduct food)
                                    {
                                        additionalInfo = $"Expiry: {food.ExpiryDate:dd/MM/yyyy}, Storage: {food.StorageCondition}";
                                    }
                                    break;
                                case ProductCategory.Drink:
                                    if (product is DrinkProduct drink)
                                    {
                                        additionalInfo = $"Alcoholic: {(drink.IsAlcoholic ? "Yes" : "No")}, Volume: {drink.Volume}ml";
                                    }
                                    break;
                                case ProductCategory.Appliance:
                                    if (product is ApplianceProduct appliance)
                                    {
                                        additionalInfo = $"Brand: {appliance.Brand}, Warranty: {appliance.WarrantyMonths} months";
                                    }
                                    break;
                                case ProductCategory.Clothes:
                                    if (product is ClothesProduct clothes)
                                    {
                                        additionalInfo = $"Material: {clothes.Material}, Sizes: {string.Join(", ", clothes.AvailableSizes)}";
                                    }
                                    break;
                            }
                            e.Value = additionalInfo;
                            e.FormattingApplied = true;
                        }
                    }
                }
                // Format Price to show thousand separator
                else if (dvgProduct.Columns[e.ColumnIndex].Name == "Price" && e.Value != null)
                {
                    if (decimal.TryParse(e.Value.ToString(), out decimal price))
                    {
                        e.Value = price.ToString("N0");
                        e.FormattingApplied = true;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error if needed
                e.Value = "";
                e.FormattingApplied = true;
            }
        }

        public void LoadData()
        {
            // Tạm thời gỡ event handler để tránh trigger trong quá trình load
            dvgProduct.SelectionChanged -= DvgProducts_SelectionChanged;
            dvgProduct.CellFormatting -= DvgProduct_CellFormatting;
            
            try
            {
                List<BaseProduct> products = productManager.GetAll();
                dvgProduct.DataSource = null;  // Clear the current data source
                dvgProduct.DataSource = products;  // Set new data source
                
                // Force refresh the grid
                dvgProduct.Refresh();
                
                foreach (DataGridViewColumn col in dvgProduct.Columns)
                {
                    col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    if (col.Name != "Description" && col.Name != "AdditionalInfo")
                    {
                        col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                }
                
                // Clear selection after loading data
                dvgProduct.ClearSelection();
                dvgProduct.CurrentCell = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading data: " + ex.Message);
            }
            finally
            {
                // Gắn lại event handler
                dvgProduct.SelectionChanged += DvgProducts_SelectionChanged;
                dvgProduct.CellFormatting += DvgProduct_CellFormatting;
            }
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
                    // Disable nút Add khi đang ở chế độ Update
                    btnAddProduct.Enabled = false;
                }
            }
            else
            {
                currentProduct = null;
                ClearInputs();
                btnUpdateProduct.Enabled = false;
                btnRemoveProduct.Enabled = false;
                btnAddProduct.Enabled = true;
            }
        }

        private void DisplayProductInfo(BaseProduct product)
        {
            if (product != null)
            {
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
                if (currentProduct != null)
                {
                    // Nếu đang có sản phẩm được chọn, không cho phép thêm mới
                    return;
                }

                BaseProduct newProduct = GetProductFromInputs();
                if (newProduct != null)
                {
                    productManager.Add(newProduct);
                    MessageBox.Show("Product added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearInputs();
                    // Select empty row
                    if (dvgProduct.Rows.Count > 0)
                    {
                        dvgProduct.ClearSelection();
                        dvgProduct.CurrentCell = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            if (currentProduct == null)
            {
                MessageBox.Show("Please select a product to update!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Validate required fields
                if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                    string.IsNullOrWhiteSpace(txtProductPrice.Text) ||
                    string.IsNullOrWhiteSpace(txtProductQuantity.Text) ||
                    cboCategory.SelectedItem == null)
                {
                    MessageBox.Show("Please fill in all required fields!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Parse and validate price
                if (!decimal.TryParse(txtProductPrice.Text, out decimal price) || price < 0)
                {
                    MessageBox.Show("Please enter a valid price!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Parse and validate quantity
                if (!int.TryParse(txtProductQuantity.Text, out int quantity) || quantity < 0)
                {
                    MessageBox.Show("Please enter a valid quantity!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Update basic properties
                currentProduct.ProductName = txtProductName.Text;
                currentProduct.Price = price;
                currentProduct.Quantity = quantity;
                currentProduct.Description = txtProductDiscription.Text;

                // Update specific properties based on product type
                switch (currentProduct.Category)
                {
                    case ProductCategory.Food:
                        if (currentProduct is FoodProduct foodProduct)
                        {
                            foodProduct.ExpiryDate = dtp_expiryDate.Value;
                            foodProduct.StorageCondition = cboStorage.SelectedItem?.ToString() ?? string.Empty;
                        }
                        break;

                    case ProductCategory.Drink:
                        if (currentProduct is DrinkProduct drinkProduct)
                        {
                            drinkProduct.IsAlcoholic = chk_isAlcoholic.Checked;
                            drinkProduct.Volume = string.IsNullOrEmpty(txt_volume.Text) ? 0 : double.Parse(txt_volume.Text);
                        }
                        break;

                    case ProductCategory.Appliance:
                        if (currentProduct is ApplianceProduct applianceProduct)
                        {
                            applianceProduct.Brand = txt_brand.Text;
                            applianceProduct.WarrantyMonths = string.IsNullOrEmpty(txt_warranty.Text) ? 0 : int.Parse(txt_warranty.Text);
                        }
                        break;

                    case ProductCategory.Clothes:
                        if (currentProduct is ClothesProduct clothesProduct)
                        {
                            clothesProduct.Material = txt_material.Text;
                            clothesProduct.AvailableSizes = new List<string>();
                            foreach (var item in chklst_sizes.CheckedItems)
                            {
                                clothesProduct.AvailableSizes.Add(item.ToString());
                            }
                        }
                        break;
                }

                productManager.Update(currentProduct);
                MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadData();
                ClearInputs();
                
                // Select empty row
                if (dvgProduct.Rows.Count > 0)
                {
                    dvgProduct.ClearSelection();
                    dvgProduct.CurrentCell = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            txtProductName.Clear();
            txtProductPrice.Clear();
            txtProductQuantity.Clear();
            txtProductDiscription.Clear();
            
            // Không reset Category để tránh trigger validation
            //cboCategory.SelectedIndex = -1;
            
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
            
            // Reset currentProduct và cập nhật trạng thái nút
            currentProduct = null;
            btnAddProduct.Enabled = true;
            btnUpdateProduct.Enabled = false;
            btnRemoveProduct.Enabled = false;
            
            // Hiển thị các trường đặc biệt dựa trên category đã chọn
            if (cboCategory.SelectedItem != null)
            {
                ShowSpecificFieldsForCategory((ProductCategory)cboCategory.SelectedItem);
            }
        }

        private BaseProduct GetProductFromInputs()
        {
            try
            {
                // Validate required fields
                if (cboCategory.SelectedItem == null || 
                    string.IsNullOrWhiteSpace(txtProductName.Text) ||
                    string.IsNullOrWhiteSpace(txtProductPrice.Text) ||
                    string.IsNullOrWhiteSpace(txtProductQuantity.Text))
                {
                    return null;
                }

                if (!decimal.TryParse(txtProductPrice.Text, out decimal price) || price < 0)
                {
                    return null;
                }

                if (!int.TryParse(txtProductQuantity.Text, out int quantity) || quantity < 0)
                {
                    return null;
                }

                ProductCategory selectedCategory = (ProductCategory)cboCategory.SelectedItem;
                
                // Sử dụng Factory Method để tạo sản phẩm
                BaseProduct product = ProductManager.CreateProduct(selectedCategory);
                
                // Thiết lập Category trước khi generate ID
                product.Category = selectedCategory;
                product.Id = product.GenerateId();  // Generate ID sau khi đã set Category
                
                // Thiết lập các thuộc tính cơ bản
                product.ProductName = txtProductName.Text;
                product.Quantity = quantity;
                product.Price = price;
                product.Description = txtProductDiscription.Text;

                // Thiết lập các thuộc tính đặc biệt dựa trên loại sản phẩm
                switch (selectedCategory)
                {
                    case ProductCategory.Food:
                        if (product is FoodProduct foodProduct)
                        {
                            foodProduct.ExpiryDate = dtp_expiryDate.Value;
                            foodProduct.StorageCondition = cboStorage.SelectedItem?.ToString() ?? string.Empty;
                        }
                        break;
                        
                    case ProductCategory.Drink:
                        if (product is DrinkProduct drinkProduct)
                        {
                            drinkProduct.IsAlcoholic = chk_isAlcoholic.Checked;
                            drinkProduct.Volume = string.IsNullOrEmpty(txt_volume.Text) ? 0 : double.Parse(txt_volume.Text);
                        }
                        break;
                        
                    case ProductCategory.Appliance:
                        if (product is ApplianceProduct applianceProduct)
                        {
                            applianceProduct.Brand = txt_brand.Text;
                            applianceProduct.WarrantyMonths = string.IsNullOrEmpty(txt_warranty.Text) ? 0 : int.Parse(txt_warranty.Text);
                        }
                        break;
                        
                    case ProductCategory.Clothes:
                        if (product is ClothesProduct clothesProduct)
                        {
                            clothesProduct.Material = txt_material.Text;
                            clothesProduct.AvailableSizes = new List<string>();
                            foreach (var item in chklst_sizes.CheckedItems)
                            {
                                clothesProduct.AvailableSizes.Add(item.ToString());
                            }
                        }
                        break;
                }
                
                return product;
            }
            catch
            {
                return null;
            }
        }

        // Xử lý sự kiện khi có thay đổi trong ProductManager
        private void ProductManager_ProductChanged(object sender, EventArgs e)
        {
            // Cập nhật dữ liệu khi có thay đổi
            LoadData();
        }

        private void lbl_storageCondition_Click(object sender, EventArgs e)
        {

        }

        private void cboStorage_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void txt_volume_TextChanged(object sender, EventArgs e)
        {

        }

        private void lbl_volume_Click(object sender, EventArgs e)
        {

        }

        private void txt_material_TextChanged(object sender, EventArgs e)
        {

        }

        private void lbl_material_Click(object sender, EventArgs e)
        {

        }

        private void txt_brand_TextChanged(object sender, EventArgs e)
        {

        }

        private void lbl_brand_Click(object sender, EventArgs e)
        {

        }

        private void chklst_sizes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lbl_expiryDate_Click(object sender, EventArgs e)
        {

        }

        private void lbl_isAlcoholic_Click(object sender, EventArgs e)
        {

        }

        private void dvgProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Product_Load_1(object sender, EventArgs e)
        {

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


