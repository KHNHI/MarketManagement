using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace MarketManagement.Model
{
    public class BaseCustomer : BaseEntity, ISerializable
    {
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsVIP { get; set; }


        public BaseCustomer()
        {
            Id = GenerateId();
        }
        public List<BaseCustomer> Customers { get; set; }


        protected BaseCustomer(SerializationInfo info, StreamingContext context)
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


        public ValidationResult ValidateWithDetails()
        {
            CustomerValidationResult result = new CustomerValidationResult();
            result.IsValid = true;

            // Kiểm tra CustomerName
            if (string.IsNullOrEmpty(CustomerName))
            {
                result.Errors.Add("Tên khách hàng không được để trống");
                result.IsValid = false;
            }
            else if (CustomerName.Length < 2)
            {
                result.Errors.Add("Tên khách hàng phải có ít nhất 2 ký tự");
                result.IsValid = false;
            }

            // Kiểm tra PhoneNumber
            if (string.IsNullOrEmpty(PhoneNumber))
            {
                result.Errors.Add("Số điện thoại không được để trống");
                result.IsValid = false;
            }
            else
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(PhoneNumber, @"^\d{10}$"))
                {
                    result.Errors.Add("Số điện thoại phải có đúng 10 chữ số");
                    result.IsValid = false;
                }
            }

            // Kiểm tra Address - bắt buộc và phải có ít nhất 5 ký tự
            if (string.IsNullOrEmpty(Address))
            {
                result.Errors.Add("Địa chỉ không được để trống");
                result.IsValid = false;
            }
            else if (Address.Length < 5)
            {
                result.Errors.Add("Địa chỉ phải có ít nhất 5 ký tự");
                result.IsValid = false;
            }

            // Kiểm tra Email - bắt buộc và phải đúng định dạng
            if (string.IsNullOrEmpty(Email))
            {
                result.Errors.Add("Email không được để trống");
                result.IsValid = false;
            }
            else
            {
                string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(Email, emailPattern))
                {
                    result.Errors.Add("Địa chỉ email không hợp lệ");
                    result.IsValid = false;
                }
            }

            return result;
        }


        public override bool Validate()
        {
            ValidationResult result = ValidateWithDetails();
            if (!result.IsValid)
            {
                MessageBox.Show(result.GetErrorMessage(), "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result.IsValid;
        }
    }
}

