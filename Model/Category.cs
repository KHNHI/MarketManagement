using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement.Model
{
    public class Category: BaseEntity
    {
        public string CategoryName { get; set; }
        public Category(string name)

        {
            CategoryName = name;
        }
        protected Category(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetString("Id");
            CategoryName = info.GetString("Name");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("ProductName", CategoryName);
        }
        public override string GenerateId()
        {
            if(CategoryName == "Electronic")
            {
                return "EL" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            else if(CategoryName == "Food")
            {
                return "FO" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            else
            {
                return "CL" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }

        }
        public override bool Validate()
        {
            return !string.IsNullOrEmpty(CategoryName);
        }
    }
}
