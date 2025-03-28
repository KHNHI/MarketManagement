using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MarketManagement.Model
{
    [Serializable]
    public class OrdersData
    {
        public List<Order> Orders { get; set; }
        
        public OrdersData()
        {
            Orders = new List<Order>();
        }
    }

    [Serializable]
    public class Order : BaseEntity, ISerializable
    {
        public string InvoiceDate { get; set; }
        public string InvoiceNo { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public decimal GrandTotal { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public Order()
        {
            Id = GenerateId();
            OrderDetails = new List<OrderDetail>();
        }

        protected Order(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetString("Id");
            InvoiceDate = info.GetString("InvoiceDate");
            InvoiceNo = info.GetString("InvoiceNo");
            CustomerId = info.GetString("CustomerId");
            CustomerName = info.GetString("CustomerName");
            Contact = info.GetString("Contact");
            Address = info.GetString("Address");
            GrandTotal = info.GetDecimal("GrandTotal");
            OrderDetails = (List<OrderDetail>)info.GetValue("OrderDetails", typeof(List<OrderDetail>));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("InvoiceDate", InvoiceDate);
            info.AddValue("InvoiceNo", InvoiceNo);
            info.AddValue("CustomerId", CustomerId);
            info.AddValue("CustomerName", CustomerName);
            info.AddValue("Contact", Contact);
            info.AddValue("Address", Address);
            info.AddValue("GrandTotal", GrandTotal);
            info.AddValue("OrderDetails", OrderDetails);
        }

        public override string GenerateId()
        {
            return "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        public override bool Validate()
        {
            return !string.IsNullOrEmpty(InvoiceNo) && !string.IsNullOrEmpty(CustomerName);
        }
    }

    [Serializable]
    public class OrderDetail : ISerializable
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }

        public OrderDetail()
        {
        }

        protected OrderDetail(SerializationInfo info, StreamingContext context)
        {
            ProductId = info.GetString("ProductId");
            ProductName = info.GetString("ProductName");
            Quantity = info.GetInt32("Quantity");
            Price = info.GetDecimal("Price");
            Total = info.GetDecimal("Total");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ProductId", ProductId);
            info.AddValue("ProductName", ProductName);
            info.AddValue("Quantity", Quantity);
            info.AddValue("Price", Price);
            info.AddValue("Total", Total);
        }
    }
}
