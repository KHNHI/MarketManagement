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
            
            // Sử dụng Singleton Pattern
            customerManager = CustomerManager.Instance;
            
            // Đăng ký sự kiện CustomerChanged
            customerManager.CustomerChanged += CustomerManager_CustomerChanged;
            
            LoadData();
            SetupDataGridView();
        }

        private void SetupDataGridView()
        {
            // Đăng ký sự kiện khi người dùng chọn 1 hàng trong DataGridView
            dvgProduct.SelectionChanged += DvgCustomer_SelectionChanged;
            btnDeleteCustomer.Click += btnDeleteCustomer_Click;
            btnAddCustomer.Click += btnAddCustomer_Click;
            btnUpdateCustomer.Click += btnUpdateCustomer_Click;
            btnUpdateCustomer.Enabled = false;
            btnDeleteCustomer.Enabled = false;
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
                btnUpdateCustomer.Enabled = true;
                btnDeleteCustomer.Enabled = true;
            }
            else
            {
                currentCustomer = null;
                ClearInputs();
                btnUpdateCustomer.Enabled = false;
                btnDeleteCustomer.Enabled = false;
            }
        }

        private void DisplayCustomerInfo(BaseCustomer customer)
        {
            if (customer != null)
            {
                txtCustomerID.Text = customer.Id;
                txtCustomerName.Text = customer.CustomerName;
                txtCustomerAddress.Text = customer.Address;
                txtCustomerPhone.Text = customer.PhoneNumber;
                txtCustomerEmail.Text = customer.Email;
                txtCustomerVIP.Text = customer.IsVIP ? "Yes" : "No";
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
            txtCustomerName.Enabled = true;
            txtCustomerAddress.Enabled = true;
            txtCustomerEmail.Enabled = true;
            txtCustomerPhone.Enabled = true;
            txtCustomerVIP.Enabled = true;
            btnAddCustomer.Enabled = true;
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

                customer.CustomerName = txtCustomerName.Text;
                customer.Address = txtCustomerAddress.Text;
                customer.PhoneNumber = txtCustomerPhone.Text;
                customer.Email = txtCustomerEmail.Text;
                customer.IsVIP = txtCustomerVIP.Text.ToLower() == "yes";

                return customer;
            }
            catch
            {
                return null;
            }
        }

        private void ClearInputs()
        {
            txtCustomerID.Clear();
            txtCustomerName.Clear();
            txtCustomerEmail.Clear();
            txtCustomerAddress.Clear();
            txtCustomerPhone.Clear();
            txtCustomerVIP.Clear();
            currentCustomer = null;
        }

        private void label1_Click(object sender, EventArgs e)
        {
            // Xử lý sự kiện click vào label1 nếu cần
        }

        // Xử lý sự kiện khi có thay đổi trong CustomerManager
        private void CustomerManager_CustomerChanged(object sender, EventArgs e)
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
        //        if (customerManager != null)
        //        {
        //            customerManager.CustomerChanged -= CustomerManager_CustomerChanged;
        //        }
        //    }
        //    base.Dispose(disposing);
        //}
    }
}