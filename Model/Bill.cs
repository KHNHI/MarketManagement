using System;
using System.Collections.Generic;
using System.Linq;

namespace MarketManagement.Model
{
    public class Bill
    {
        public string BillId { get; set; }
        public DateTime Date { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public List<BillItem> Items { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; }

        public Bill()
        {
            BillId = DateTime.Now.ToString("yyyyMMddHHmmss");
            Date = DateTime.Now;
            Items = new List<BillItem>();
            TotalAmount = 0;
        }

        public void AddItem(BillItem item)
        {
            Items.Add(item);
            CalculateTotalAmount();
        }

        public void RemoveItem(BillItem item)
        {
            Items.Remove(item);
            CalculateTotalAmount();
        }

        private void CalculateTotalAmount()
        {
            decimal total = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                total += Items[i].TotalPrice;
            }
            TotalAmount = total;
        }
    }

    public class BillItem
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }

        public BillItem(string productId, string productName, int quantity, decimal unitPrice)
        {
            ProductId = productId;
            ProductName = productName;
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalPrice = quantity * unitPrice;
        }
    }
} 