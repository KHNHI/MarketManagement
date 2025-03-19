using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MarketManagement.UseControl
{
    public partial class Product : System.Windows.Forms.UserControl

    {
        private ProductManager productManager;
        private BaseProduct currentProduct;
        public Product()
        {
            InitializeComponent();
            productManager = new ProductManager();
            LoadData();
            SetupDataGridView();
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
            dvgProduct.DataSource = null;
            dvgProduct.DataSource = productManager.GetAll();
            foreach (DataGridViewColumn col in dvgProduct.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (col.Name != "Description")
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }
        private void DvgProducts_SelectionChanged(object sender, EventArgs e)
        {
            if (dvgProduct.SelectedRows.Count > 0)
            {
                // Lấy product được chọn
                currentProduct = (BaseProduct)dvgProduct.SelectedRows[0].DataBoundItem;
                // Hiển thị thông tin lên các textbox
                DisplayProductInfo(currentProduct);
                // Enable nút Edit và Delete
                btnUpdateProduct.Enabled = true;
                btnRemoveProduct.Enabled = true;
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
                txtProductCategory.Text = product.Category;
            }
        }
       
        private void btnAddProduct_Click(object sender, EventArgs e)
        {

            BaseProduct product = GetProductFromInputs();
            if (product != null)
            {
                if (currentProduct == null)
                    productManager.Add(product);
                else
                    productManager.Update(product);

                LoadData();
                ClearInputs();
            }

        }
        private void btnUpdateProduct_Click(object sender, EventArgs e)
        {
            

            // Enable các controls để sửa
            txtProductName.Enabled = true;
            txtProductQuantity.Enabled = true;
            txtProductPrice.Enabled = true;
            txtProductDiscription.Enabled = true;
            txtProductCategory.Enabled = true;
            btnAddProduct.Enabled = true;

            
        }
        private void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (currentProduct == null)
            {
                MessageBox.Show("Please select a product to delete!");
                return;
            } else
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
                BaseProduct product = new BaseProduct();
                if (currentProduct != null)
                    product.Id = currentProduct.Id;

                product.ProductName = txtProductName.Text;
                product.Quantity = int.Parse(txtProductQuantity.Text);
                product.Price = decimal.Parse(txtProductPrice.Text);
                product.Description = txtProductDiscription.Text;
                product.Category = txtProductCategory.Text;

                return product;
            }
            catch
            {
                return null;
            }
        }
        private void ClearInputs()
        {
            txtProductID.Clear();
            txtProductName.Clear();
            txtProductPrice.Clear();
            txtProductQuantity.Clear();
            txtProductDiscription.Clear();
            //cboCategory.SelectedIndex = -1;
            currentProduct = null;
            currentProduct = null;
        }

       
    }
       
    }

