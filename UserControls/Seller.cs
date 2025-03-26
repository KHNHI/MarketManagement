using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MarketManagement.Manager;

namespace MarketManagement.UserControls
{
    public partial class Seller : UserControl
    {
        private SellerManager sellerManager;

        public Seller()
        {
            InitializeComponent();
            
            // Sử dụng Singleton Pattern
            sellerManager = SellerManager.Instance;
            
            // Đăng ký sự kiện SellerChanged
            sellerManager.SellerChanged += SellerManager_SellerChanged;
            
            // Load dữ liệu ban đầu
            LoadData();
        }
        
        // Phương thức load dữ liệu
        private void LoadData()
        {
            // Code để load dữ liệu từ sellerManager vào UI
            // Ví dụ: cập nhật DataGridView với danh sách người bán
        }

        // Xử lý sự kiện khi có thay đổi trong SellerManager
        private void SellerManager_SellerChanged(object sender, EventArgs e)
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
        //        if (sellerManager != null)
        //        {
        //            sellerManager.SellerChanged -= SellerManager_SellerChanged;
        //        }
        //    }
        //    base.Dispose(disposing);
        //}

        // Thêm phương thức này vào lớp Seller trong file Seller.cs
        private void label1_Click(object sender, EventArgs e)
        {
            // Xử lý khi label1 được click
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Seller_Load(object sender, EventArgs e)
        {

        }
    }
}