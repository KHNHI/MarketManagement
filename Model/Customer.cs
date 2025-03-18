using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement.Model
{
    public class Customer : BaseEntity, ISerializable
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsVIP { get; set; }

        public Customer(string name, string address, string phoneNumber, string email, bool isVIP = false)
        {
            CustomerName = name;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
            IsVIP = isVIP;
        }

        protected Customer(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetString("Id");
            CustomerName = info.GetString("Name");
            Address = info.GetString("Address");
            PhoneNumber = info.GetString("PhoneNumber");
            Email = info.GetString("Email");
            IsVIP = info.GetBoolean("IsVIP");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Name", CustomerName);
            info.AddValue("Address", Address);
            info.AddValue("PhoneNumber", PhoneNumber);
            info.AddValue("Email", Email);
            info.AddValue("IsVIP", IsVIP);
        }

        public override string GenerateId()
        {
            if (IsVIP)
            {
                return "VIP" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            else
            {
                return "CUS" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        public override bool Validate()
        {
           
            return !string.IsNullOrEmpty(CustomerName) && !string.IsNullOrEmpty(PhoneNumber);
        }
    }
}