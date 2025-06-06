﻿using MarketManagement.UseControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MarketManagement.Manager;

namespace MarketManagement
{
    public partial class panelMain : Form
    {
        UseControl.Product product = new UseControl.Product();
        UseControl.CustomerUC customer = new UseControl.CustomerUC();
        UserControls.Order order = new UserControls.Order();
        UserControls.Dashboard dashboard = new UserControls.Dashboard();
        UserControls.Billing billing = new UserControls.Billing();
        UserControls.Staff staff = new UserControls.Staff();


        public panelMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnDashboard_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(dashboard);
            dashboard.Dock = DockStyle.Fill;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(product);
            product.Dock = DockStyle.Fill;
        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void lblUsẻName_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnStockEntry_Click(object sender, EventArgs e)
        {

        }

        private void btnStockAdjustment_Click(object sender, EventArgs e)
        {

        }

        private void btnSaleHistory_Click(object sender, EventArgs e)
        {

        }

        private void btnPOSRecord_Click(object sender, EventArgs e)
        {

        }

        private void lblTitleName_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            CustomerUC customer = new CustomerUC();
            customer.Dock = DockStyle.Fill; // Đảm bảo hiển thị đầy đủ
            panel1.Controls.Clear(); // Xóa nội dung cũ nếu có
            panel1.Controls.Add(customer);
            customer.BringToFront(); // Đưa lên trước


        }

        private void panel1_Paint_1(object sender, PaintEventArgs e)
        {
            
            //panel1.Controls.Add(dashboard);
            //dashboard.Dock = DockStyle.Fill;

        }

        private void panelTitle_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(order);
            order.Dock = DockStyle.Fill;
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(billing);
            billing.Dock = DockStyle.Fill;
        }

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    panel1.Controls.Clear();
        //    panel1.Controls.Add(category);
        //    category.Dock = DockStyle.Fill;
        //}

        private void button1_Click_1(object sender, EventArgs e)
        {
            panel1.Controls.Clear();
            panel1.Controls.Add(staff);
            staff.Dock = DockStyle.Fill;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Wellcome wellcome = new Wellcome();
            wellcome.Show();
            this.Hide();
        }
    }
}
