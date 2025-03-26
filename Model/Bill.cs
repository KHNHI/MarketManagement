using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MarketManagement.Model
{
    [Serializable]
    public class Bill : BaseEntity, ISerializable
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
            Id = GenerateId();
            BillId = Id;
            Date = DateTime.Now;
            Items = new List<BillItem>();
            TotalAmount = 0;
        }

        protected Bill(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetString("Id");
            BillId = info.GetString("BillId");
            Date = info.GetDateTime("Date");
            CustomerId = info.GetString("CustomerId");
            CustomerName = info.GetString("CustomerName");
            Contact = info.GetString("Contact");
            Address = info.GetString("Address");
            Items = (List<BillItem>)info.GetValue("Items", typeof(List<BillItem>));
            TotalAmount = info.GetDecimal("TotalAmount");
            PaymentMethod = info.GetString("PaymentMethod");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("BillId", BillId);
            info.AddValue("Date", Date);
            info.AddValue("CustomerId", CustomerId);
            info.AddValue("CustomerName", CustomerName);
            info.AddValue("Contact", Contact);
            info.AddValue("Address", Address);
            info.AddValue("Items", Items);
            info.AddValue("TotalAmount", TotalAmount);
            info.AddValue("PaymentMethod", PaymentMethod);
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

        public override string GenerateId()
        {
            return "BILL" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(CustomerName) && Items.Count > 0;
        }
    }

    [Serializable]
    public class BillItem : ISerializable
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

        protected BillItem(SerializationInfo info, StreamingContext context)
        {
            ProductId = info.GetString("ProductId");
            ProductName = info.GetString("ProductName");
            Quantity = info.GetInt32("Quantity");
            UnitPrice = info.GetDecimal("UnitPrice");
            TotalPrice = info.GetDecimal("TotalPrice");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ProductId", ProductId);
            info.AddValue("ProductName", ProductName);
            info.AddValue("Quantity", Quantity);
            info.AddValue("UnitPrice", UnitPrice);
            info.AddValue("TotalPrice", TotalPrice);
        }
    }
} 