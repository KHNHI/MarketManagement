using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement.Model
{
    public class Seller : BaseEntity
    {
        public string SellerName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public Seller(string name, string email, string password)
        {
            SellerName = name;
            Email = email;
            Password = password;
        }

        protected Seller(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetString("Id");
            SellerName = info.GetString("Name");
            Email = info.GetString("Email");
            Password = info.GetString("Password");
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("Name", SellerName);
            info.AddValue("Email", Email);
            info.AddValue("Password", Password);
        }

        public override string GenerateId()
        {
            // Tạo ID dựa trên tên người bán
            if (SellerName != null && SellerName.Length >= 2)
            {
                // Lấy 2 ký tự đầu tiên của tên người bán
                string prefix = SellerName.Substring(0, 2).ToUpper();
                return prefix + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            else
            {
                return "SL" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }

        public override bool Validate()
        {
            // Kiểm tra tính hợp lệ: tên, email và mật khẩu không được rỗng
            return !string.IsNullOrEmpty(SellerName) &&
                   !string.IsNullOrEmpty(Email) &&
                   !string.IsNullOrEmpty(Password);
        }
    }
}
