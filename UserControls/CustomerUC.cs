using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MarketManagement.Manager;
using MarketManagement.Model;

namespace MarketManagement.UseControl
{
    public partial class CustomerUC : System.Windows.Forms.UserControl
    {
        private CustomerManager customerManager;
        private BaseCustomer currentCustomer;

        public List<CustomerUC> Customers { get; set; }

        public CustomerUC()
        {
            InitializeComponent();
            customerManager = new CustomerManager();
            LoadData();
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            // Đăng ký sự kiện khi người dùng chọn 1 hàng trong DataGridView
            dvgProduct.SelectionChanged += DvgCustomer_SelectionChanged;
            btnRemoveProduct.Click += btnDeleteCustomer_Click;
            btnAddProduct.Click += btnAddCustomer_Click;
            btnUpdateProduct.Click += btnUpdateCustomer_Click;
            btnUpdateProduct.Enabled = false;
            btnRemoveProduct.Enabled = false;
        }

        public void LoadData()
        {
            dvgProduct.DataSource = null;
            dvgProduct.DataSource = customerManager.GetAll();
            foreach (DataGridViewColumn col in dvgProduct.Columns)
            {
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                if (col.Name != "Address")
                {
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }
            }
        }

        private void DvgCustomer_SelectionChanged(object sender, EventArgs e)
        {
            if (dvgProduct.SelectedRows.Count > 0)
            {
                // Lấy customer được chọn
                currentCustomer = (BaseCustomer)dvgProduct.SelectedRows[0].DataBoundItem;
                // Hiển thị thông tin lên các textbox
                DisplayCustomerInfo(currentCustomer);
                // Enable nút Edit và Delete
                btnUpdateProduct.Enabled = true;
                btnRemoveProduct.Enabled = true;
            }
            else
            {
                currentCustomer = null;
                ClearInputs();
                btnUpdateProduct.Enabled = false;
                btnRemoveProduct.Enabled = false;
            }
        }

        private void DisplayCustomerInfo(BaseCustomer customer)
        {
            if (customer != null)
            {
                txtProductID.Text = customer.Id;
                txtProductName.Text = customer.CustomerName;
                txtProductQuantity.Text = customer.Address;
                txtProductDiscription.Text = customer.PhoneNumber;
                txtProductPrice.Text = customer.Email;
                txtProductCategory.Text = customer.IsVIP ? "Yes" : "No";
            }
        }

        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            BaseCustomer customer = GetCustomerFromInputs();
            if (customer != null)
            {
                if (currentCustomer == null)
                    customerManager.Add(customer);
                else
                    customerManager.Update(customer);

                LoadData();
                ClearInputs();
            }
        }

        private void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            // Enable các controls để sửa
            txtProductName.Enabled = true;
            txtProductQuantity.Enabled = true;
            txtProductPrice.Enabled = true;
            txtProductDiscription.Enabled = true;
            txtProductCategory.Enabled = true;
            btnAddProduct.Enabled = true;
        }

        private void btnDeleteCustomer_Click(object sender, EventArgs e)
        {
            if (currentCustomer == null)
            {
                MessageBox.Show("Please select a customer to delete!");
                return;
            }
            else
            {
                customerManager.Remove(currentCustomer.Id);
                LoadData();
                ClearInputs();
            }
        }

        private BaseCustomer GetCustomerFromInputs()
        {
            try
            {
                BaseCustomer customer = new BaseCustomer();
                if (currentCustomer != null)
                    customer.Id = currentCustomer.Id;

                customer.CustomerName = txtProductName.Text;
                customer.Address = txtProductQuantity.Text;
                customer.PhoneNumber = txtProductDiscription.Text;
                customer.Email = txtProductPrice.Text;
                customer.IsVIP = txtProductCategory.Text.ToLower() == "yes";

                return customer;
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
            txtProductCategory.Clear();
            currentCustomer = null;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện click vào label1 nếu cần
        }
    }
}