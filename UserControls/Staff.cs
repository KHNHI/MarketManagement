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
            dataGridViewStaff.Columns.Add("UserName", "Tên nhân viên");
            dataGridViewStaff.Columns.Add("PhoneNumber", "Số điện thoại");
            dataGridViewStaff.Columns.Add("DateOfBirth", "Ngày sinh");
            dataGridViewStaff.Columns.Add("Position", "Vị trí");
            dataGridViewStaff.Columns.Add("Shift", "Ca làm việc");

            dataGridViewStaff.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewStaff.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridViewStaff.MultiSelect = false;
            dataGridViewStaff.ReadOnly = true;
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
    }
}