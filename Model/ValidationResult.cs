using System.Collections.Generic;


namespace MarketManagement.Model
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; } = new List<string>();


        public string GetErrorMessage()
        {
            return string.Join("\n", Errors);
        }
    }

    public class CustomerValidationResult : ValidationResult
    {
        public bool IsPhoneNumberDuplicated { get; set; }

        public CustomerValidationResult()
        {
            IsPhoneNumberDuplicated = false;
        }

        public void AddPhoneNumberDuplicationError(string phoneNumber)
        {
            IsPhoneNumberDuplicated = true;
            Errors.Add($"Số điện thoại {phoneNumber} đã được đăng ký cho khách hàng khác");
            IsValid = false;
        }
    }
}



