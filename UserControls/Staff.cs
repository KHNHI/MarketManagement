using MarketManagement.Manager;
using MarketManagement.Model;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MarketManagement.UserControls
{
    public partial class Staff : UserControl
    {
        private readonly StaffManager staffManager;

        public Staff()
        {
            InitializeComponent();
            staffManager = StaffManager.Instance;
            
            // Khởi tạo dữ liệu cho comboBoxPosition
            comboBoxPosition.DataSource = Enum.GetValues(typeof(StaffPosition));
            
            // Khởi tạo dữ liệu cho comboBoxShift
            comboBoxShift.DataSource = Enum.GetValues(typeof(ShiftType));
            
            InitializeDataGridView();
            LoadStaffData();
        }

        private void InitializeDataGridView()
        {
            dataGridViewStaff.Columns.Clear();
            dataGridViewStaff.Columns.Add("Id", "ID");
            dataGridViewStaff.Columns.Add("UserName", "User Name");
            dataGridViewStaff.Columns.Add("PhoneNumber", "Phone Number");
            dataGridViewStaff.Columns.Add("DateOfBirth", "Date of Birth");
            dataGridViewStaff.Columns.Add("Position", "Position");
            dataGridViewStaff.Columns.Add("Shift", "Shift");

            dataGridViewStaff.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewStaff.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewStaff.MultiSelect = false;
            dataGridViewStaff.ReadOnly = true;
            
            // Thêm event handler cho double-click
            dataGridViewStaff.CellDoubleClick += DataGridViewStaff_CellDoubleClick;
            
            // Thêm context menu cho tính năng copy
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem copyMenuItem = new ToolStripMenuItem("Copy Value");
            copyMenuItem.Click += CopyMenuItem_Click;
            contextMenu.Items.Add(copyMenuItem);
            dataGridViewStaff.ContextMenuStrip = contextMenu;
            
            // Thêm hỗ trợ phím tắt Ctrl+C
            dataGridViewStaff.KeyDown += DataGridViewStaff_KeyDown;
        }

        private void LoadStaffData()
        {
            dataGridViewStaff.Rows.Clear();
            System.Collections.Generic.List<Model.Staff> staffs = staffManager.GetAll();
            foreach (Model.Staff staff in staffs)
            {
                dataGridViewStaff.Rows.Add(
                    staff.Id,
                    staff.UserName,
                    staff.PhoneNumber,
                    staff.DateOfBirth.ToShortDateString(),
                    staff.Position.ToString(),
                    staff.Shift.ToString()
                );
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Model.Staff staff = new Model.Staff
                {
                    UserName = txtUserName.Text,
                    PhoneNumber = txtPhoneNumber.Text,
                    DateOfBirth = dateTimePickerDOB.Value,
                    Position = (StaffPosition)comboBoxPosition.SelectedItem,
                    Shift = (ShiftType)comboBoxShift.SelectedItem
                };

                if (staff.Validate())
                {
                    staffManager.Add(staff);
                    LoadStaffData();
                    ClearInputs();
                }
                else
                {
                    MessageBox.Show("Dữ liệu không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridViewStaff.SelectedRows.Count > 0)
                {
                    string staffId = dataGridViewStaff.SelectedRows[0].Cells["Id"].Value.ToString();
                    Model.Staff staff = staffManager.GetById(staffId);

                    if (staff != null)
                    {
                        staff.UserName = txtUserName.Text;
                        staff.PhoneNumber = txtPhoneNumber.Text;
                        staff.DateOfBirth = dateTimePickerDOB.Value;
                        staff.Position = (StaffPosition)comboBoxPosition.SelectedItem;
                        staff.Shift = (ShiftType)comboBoxShift.SelectedItem;

                        if (staff.Validate())
                        {
                            staffManager.Update(staff);
                            LoadStaffData();
                            ClearInputs();
                        }
                        else
                        {
                            MessageBox.Show("Dữ liệu không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridViewStaff.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa?", "Xác nhận",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string staffId = dataGridViewStaff.SelectedRows[0].Cells["Id"].Value.ToString();
                    staffManager.Remove(staffId);
                    LoadStaffData();
                    ClearInputs();
                }
            }
        }

        private void dataGridViewStaff_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewStaff.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridViewStaff.SelectedRows[0];
                txtUserName.Text = row.Cells["UserName"].Value.ToString();
                txtPhoneNumber.Text = row.Cells["PhoneNumber"].Value.ToString();
                dateTimePickerDOB.Value = DateTime.Parse(row.Cells["DateOfBirth"].Value.ToString());
                comboBoxPosition.SelectedItem = Enum.Parse(typeof(StaffPosition), row.Cells["Position"].Value.ToString());
                comboBoxShift.SelectedItem = Enum.Parse(typeof(ShiftType), row.Cells["Shift"].Value.ToString());
            }
        }

        private void ClearInputs()
        {
            txtUserName.Clear();
            txtPhoneNumber.Clear();
            dateTimePickerDOB.Value = DateTime.Now;
            comboBoxPosition.SelectedIndex = -1;
            comboBoxShift.SelectedIndex = -1;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void dateTimePickerDOB_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void DataGridViewStaff_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Kiểm tra nếu click vào cell hợp lệ (không phải header, không phải index -1)
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Lấy giá trị từ cell và copy vào clipboard
                object cellValue = dataGridViewStaff.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
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

        private void DataGridViewStaff_KeyDown(object sender, KeyEventArgs e)
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
                if (dataGridViewStaff.CurrentCell != null && dataGridViewStaff.CurrentCell.Value != null)
                {
                    // Copy giá trị vào clipboard
                    Clipboard.SetText(dataGridViewStaff.CurrentCell.Value.ToString());
                }
                else if (dataGridViewStaff.SelectedRows.Count > 0)
                {
                    // Nếu chọn cả hàng, lấy giá trị từ cột đầu tiên
                    DataGridViewRow row = dataGridViewStaff.SelectedRows[0];
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