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

            // Đăng ký sự kiện Load của UserControl
            this.Load += CustomerUC_Load;

            SetupDataGridView();
            ClearInputs();
        }


        private void CustomerUC_Load(object sender, EventArgs e)
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
            dvgProduct.AutoGenerateColumns = true;
            dvgProduct.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Thêm thuộc tính này để ngăn tự động chọn
            dvgProduct.TabStop = false;

            // Đăng ký sự kiện khi người dùng chọn 1 hàng trong DataGridView
            dvgProduct.SelectionChanged += DvgCustomer_SelectionChanged;
            dvgProduct.DataBindingComplete += DvgProduct_DataBindingComplete;

            btnDeleteCustomer.Click += btnDeleteCustomer_Click;
            btnAddCustomer.Click += btnAddCustomer_Click;
            btnUpdateCustomer.Click += btnUpdateCustomer_Click;
            btnUpdateCustomer.Enabled = false;
            btnDeleteCustomer.Enabled = false;

            // Thêm event handler cho double-click
            dvgProduct.CellDoubleClick += DvgProduct_CellDoubleClick;

            // Thêm context menu cho tính năng copy
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("Copy Value");
            copyMenuItem.Click += CopyMenuItem_Click;
            contextMenu.Items.Add(copyMenuItem);
            dvgProduct.ContextMenuStrip = contextMenu;

            // Thêm hỗ trợ phím tắt Ctrl+C
            dvgProduct.KeyDown += DvgProduct_KeyDown;
        }


        private void DvgProduct_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dvgProduct.ClearSelection();
            dvgProduct.CurrentCell = null;
        }


        public void LoadData()
        {
            // Tạm thời gỡ event handler để tránh trigger trong quá trình load
            dvgProduct.SelectionChanged -= DvgCustomer_SelectionChanged;

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

            // Clear selection after loading data
            dvgProduct.ClearSelection();
            dvgProduct.CurrentCell = null;

            // Gắn lại event handler
            dvgProduct.SelectionChanged += DvgCustomer_SelectionChanged;
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
                // Disable nút Add khi đang ở chế độ Update
                btnAddCustomer.Enabled = false;
            }
            else
            {
                currentCustomer = null;
                ClearInputs();
                btnUpdateCustomer.Enabled = false;
                btnDeleteCustomer.Enabled = false;
                btnAddCustomer.Enabled = true;
            }
        }


        private void DisplayCustomerInfo(BaseCustomer customer)
        {
            if (customer != null)
            {
                //txtCustomerID.Text = customer.Id;
                txtCustomerName.Text = customer.CustomerName;
                txtCustomerAddress.Text = customer.Address;
                txtCustomerPhone.Text = customer.PhoneNumber;
                txtCustomerEmail.Text = customer.Email;
                chkbIsVip.Checked = customer.IsVIP;
            }
        }


        private void btnAddCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                // Nếu đang trong chế độ update thì không cho phép Add
                if (currentCustomer != null)
                {
                    MessageBox.Show("Vui lòng xóa form trước khi thêm khách hàng mới!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                BaseCustomer customer = GetCustomerFromInputs();
                if (customer != null)
                {
                    var validationResult = customer.ValidateWithDetails();
                    if (validationResult.IsValid)
                    {
                        customerManager.Add(customer);
                        LoadData();
                        ClearInputs(); // Chỉ clear khi thêm thành công
                        if (dvgProduct.Rows.Count > 0)
                        {
                            dvgProduct.ClearSelection();
                            dvgProduct.CurrentCell = null;
                        }
                    }
                    else
                    {
                        MessageBox.Show(validationResult.GetErrorMessage(), "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // Không clear input để người dùng có thể sửa lỗi
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Không clear input khi có lỗi
            }
        }


        private void btnUpdateCustomer_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentCustomer == null)
                {
                    MessageBox.Show("Vui lòng chọn khách hàng cần cập nhật!", "Thông báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }


                BaseCustomer customer = GetCustomerFromInputs();
                if (customer != null)
                {
                    var validationResult = customer.ValidateWithDetails();
                    if (validationResult.IsValid)
                    {
                        customerManager.Update(customer);
                        LoadData();
                        ClearInputs(); // Chỉ clear khi cập nhật thành công
                        if (dvgProduct.Rows.Count > 0)
                        {
                            dvgProduct.ClearSelection();
                            dvgProduct.CurrentCell = null;
                        }
                    }
                    else
                    {
                        MessageBox.Show(validationResult.GetErrorMessage(), "Lỗi",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // Không clear input để người dùng có thể sửa lỗi
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Không clear input khi có lỗi
            }
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
                customer.IsVIP = chkbIsVip.Checked;


                return customer;
            }
            catch
            {
                return null;
            }
        }


        private void ClearInputs()
        {
            //txtCustomerID.Clear();
            txtCustomerName.Clear();
            txtCustomerEmail.Clear();
            txtCustomerAddress.Clear();
            txtCustomerPhone.Clear();
            chkbIsVip.Checked = false;
            currentCustomer = null;

            // Reset button states
            btnAddCustomer.Enabled = true;
            btnUpdateCustomer.Enabled = false;
            btnDeleteCustomer.Enabled = false;

            // Enable all input controls
            txtCustomerName.Enabled = true;
            txtCustomerAddress.Enabled = true;
            txtCustomerEmail.Enabled = true;
            txtCustomerPhone.Enabled = true;
            chkbIsVip.Enabled = true;
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


        private void DvgProduct_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu click vào cell hợp lệ (không phải header, không phải index -1)
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Lấy giá trị từ cell và copy vào clipboard
                object cellValue = dvgProduct.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (cellValue != null)
                {
                    Clipboard.SetText(cellValue.ToString());
                    MessageBox.Show("Copied to clipboard: " + cellValue.ToString(), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private void CopyMenuItem_Click(object sender, EventArgs e)
        {
            CopyCellValueToClipboard();
        }


        private void DvgProduct_KeyDown(object sender, KeyEventArgs e)
        {
            // Kiểm tra phím tắt Ctrl+C để copy
            if (e.Control && e.KeyCode == Keys.C)
            {
                CopyCellValueToClipboard();
                e.Handled = true;
            }
        }


        private void CopyCellValueToClipboard()
        {
            try
            {
                // Kiểm tra xem có cell nào được chọn không
                if (dvgProduct.CurrentCell != null && dvgProduct.CurrentCell.Value != null)
                {
                    // Copy giá trị vào clipboard
                    Clipboard.SetText(dvgProduct.CurrentCell.Value.ToString());
                }
                else if (dvgProduct.SelectedRows.Count > 0)
                {
                    // Nếu chọn cả hàng, lấy giá trị từ cột đầu tiên
                    DataGridViewRow row = dvgProduct.SelectedRows[0];
                    if (row.Cells[0].Value != null)
                    {
                        Clipboard.SetText(row.Cells[0].Value.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error copying value: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }




    }
}

