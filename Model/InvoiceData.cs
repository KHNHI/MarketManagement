using System;
using System.Data;

namespace MarketManagement
{
    [Serializable]
    public class InvoiceData
    {
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerContact { get; set; }
        public string CustomerAddress { get; set; }
        public decimal GrandTotal { get; set; }
        public DataTable Products { get; set; }

        public InvoiceData()
        {
            Products = new DataTable();
        }
    }
} 