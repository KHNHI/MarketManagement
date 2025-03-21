using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace MarketManagement
{
    public partial class frm_invoice : Form
    {
        private InvoiceData _invoiceData;
        private PrintDocument printDocument = new PrintDocument();
        private PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();

        public frm_invoice()
        {
            InitializeComponent();

            // Cài đặt sự kiện để vẽ hóa đơn
            printDocument.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);

            // Cài đặt hộp thoại xem trước bản in
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.Size = new Size(800, 600);
            printPreviewDialog.Text = "Invoice Preview";
        }

        private void frm_invoice_Load(object sender, EventArgs e)
        {
            try
            {
                LoadInvoiceData();
                printPreviewDialog.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading invoice: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadInvoiceData()
        {
            string tempPath = Path.Combine(Application.StartupPath, "Data", "temp_invoice.json");
            if (File.Exists(tempPath))
            {
                string jsonData = File.ReadAllText(tempPath);
                _invoiceData = JsonConvert.DeserializeObject<InvoiceData>(jsonData);
            }
            else
            {
                throw new FileNotFoundException("Invoice data not found.");
            }
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titleFont = new Font("Arial", 16, FontStyle.Bold);
            Font companyFont = new Font("Arial", 14, FontStyle.Bold);
            Font headerFont = new Font("Arial", 12, FontStyle.Bold);
            Font normalFont = new Font("Arial", 10);
            Font smallFont = new Font("Arial", 8);

            // Định dạng bút vẽ và các thuộc tính vẽ khác
            Pen pen = new Pen(Color.Black, 1);
            SolidBrush brush = new SolidBrush(Color.Black);
            StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center };
            StringFormat rightFormat = new StringFormat { Alignment = StringAlignment.Far };

            // Vùng in
            int margin = 20;
            int y = margin;
            int width = e.PageBounds.Width - 2 * margin;

            // Vẽ tiêu đề
            g.DrawString("Supermarket Management System", titleFont, brush, margin + width / 2, y, centerFormat);
            y += 30;

            // Vẽ tên công ty - thay "MJ Technical Solution" thành "Mean Token Girls"
            g.DrawString("Mean Token Girls", companyFont, brush, margin + width / 2, y, centerFormat);
            y += 30;

            // Vẽ tiêu đề hóa đơn
            g.DrawString("INVOICE", headerFont, brush, margin + width / 2, y, centerFormat);
            y += 30;

            // Vẽ đường kẻ
            g.DrawLine(pen, margin, y, margin + width, y);
            y += 5;

            // Vẽ phần thông tin khách hàng và hóa đơn
            int halfWidth = width / 2;

            // Cột thông tin khách hàng
            g.DrawString("Customer Detail :", headerFont, brush, margin, y);
            y += 20;
            g.DrawString("Name :", normalFont, brush, margin, y);
            g.DrawString(_invoiceData.CustomerName, normalFont, brush, margin + 80, y);
            y += 20;
            g.DrawString("Contact :", normalFont, brush, margin, y);
            g.DrawString(_invoiceData.CustomerContact, normalFont, brush, margin + 80, y);
            y += 20;
            g.DrawString("Address :", normalFont, brush, margin, y);
            g.DrawString(_invoiceData.CustomerAddress, normalFont, brush, margin + 80, y);

            // Đặt lại vị trí y cho cột thông tin hóa đơn
            y -= 60;

            // Cột thông tin hóa đơn
            g.DrawString("Billing Detail :", headerFont, brush, margin + halfWidth, y);
            y += 20;
            g.DrawString("Bill No :", normalFont, brush, margin + halfWidth, y);
            g.DrawString(_invoiceData.InvoiceNo, normalFont, brush, margin + halfWidth + 80, y);
            y += 20;
            g.DrawString("Date :", normalFont, brush, margin + halfWidth, y);
            g.DrawString(_invoiceData.InvoiceDate, normalFont, brush, margin + halfWidth + 80, y);
            y += 20;
            g.DrawString("Amount :", normalFont, brush, margin + halfWidth, y);
            g.DrawString(_invoiceData.GrandTotal.ToString("0.00"), normalFont, brush, margin + halfWidth + 80, y);

            // Đặt lại vị trí y cho phần tiếp theo
            y += 30;

            // Vẽ đường kẻ
            g.DrawLine(pen, margin, y, margin + width, y);
            y += 10;

            // Vẽ tiêu đề bảng sản phẩm
            int colWidth = width / 4;
            g.DrawString("Product ID", headerFont, brush, margin, y);
            g.DrawString("Product Name", headerFont, brush, margin + colWidth, y);
            g.DrawString("Product Quantity", headerFont, brush, margin + 2 * colWidth, y);
            g.DrawString("Product Price", headerFont, brush, margin + 3 * colWidth, y);
            y += 25;

            // Vẽ danh sách sản phẩm
            foreach (DataRow row in _invoiceData.Products.Rows)
            {
                g.DrawString(row["ProductID"].ToString(), normalFont, brush, margin, y);
                g.DrawString(row["ProductName"].ToString(), normalFont, brush, margin + colWidth, y);
                g.DrawString(row["ProductQuantity"].ToString(), normalFont, brush, margin + 2 * colWidth, y);
                g.DrawString(row["ProductPrice"].ToString(), normalFont, brush, margin + 3 * colWidth, y);
                y += 20;
            }

            y += 10;
            // Vẽ tổng cộng
            g.DrawString("Total Amount :", headerFont, brush, margin + 2 * colWidth, y);
            g.DrawString(_invoiceData.GrandTotal.ToString("0.00"), headerFont, brush, margin + 3 * colWidth, y);
            y += 30;

            // Vẽ thông tin GST (Tax ID)
            g.DrawString("GST NO : 00000000000000000", normalFont, brush, margin + width / 2, y, centerFormat);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDocument;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDocument.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error printing: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}