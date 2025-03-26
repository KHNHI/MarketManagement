using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MarketManagement.Model.Exceptions;

namespace MarketManagement.Model
{
    public delegate void ShiftChangedEventHandler(object sender, ShiftChangedEventArgs e);
    public class ShiftChangedEventArgs : EventArgs
    {
        public ShiftType OldShift { get; private set; }
        public ShiftType NewShift { get; private set; }

        public ShiftChangedEventArgs(ShiftType oldShift, ShiftType newShift)
        {
            OldShift = oldShift;
            NewShift = newShift;
        }
    }
    public class Staff : BaseEntity, ISerializable
    {
        private string _userName;
        private string _phoneNumber;
        private DateTime _dateOfBirth;
        private StaffPosition _position;
        private ShiftType _shift;

        public event ShiftChangedEventHandler ShiftChanged;

        // Delegate để kiểm tra điều kiện tuổi
        private delegate bool AgeValidationHandler(DateTime birthDate);
        private readonly AgeValidationHandler _validateAge;

        public string UserName
        {
            get { return _userName; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new StaffValidationException("Tên người dùng không được để trống");
                _userName = value;
            }
        }
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new StaffValidationException("Số điện thoại không được để trống");
                if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{10}$"))
                    throw new StaffValidationException("Số điện thoại phải có 10 chữ số");
                _phoneNumber = value;
            }
        }
        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                if (!_validateAge(value))
                    throw new StaffValidationException("Tuổi nhân viên phải từ 18 đến 60");
                _dateOfBirth = value;
            }
        }
        public StaffPosition Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public ShiftType Shift
        {
            get { return _shift; }
            set
            {
                if (_shift != value)
                {
                    ShiftType oldShift = _shift;
                    _shift = value;
                    OnShiftChanged(oldShift, value);
                }
            }
        }
        public Staff()
        {
            Id = GenerateId();
            _validateAge = birthDate =>
            {
                int age = CalculateAge(birthDate);
                return age >= 18 && age <= 60;
            };
        }
        protected Staff(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetString("Id");
            UserName = info.GetString("UserName");
            PhoneNumber = info.GetString("PhoneNumber");
            DateOfBirth = info.GetDateTime("DateOfBirth");
            Position = (StaffPosition)info.GetInt32("Position");
            Shift = (ShiftType)info.GetInt32("Shift");

            _validateAge = birthDate =>
            {
                int age = CalculateAge(birthDate);
                return age >= 18 && age <= 60;
            };
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("UserName", UserName);
            info.AddValue("PhoneNumber", PhoneNumber);
            info.AddValue("DateOfBirth", DateOfBirth);
            info.AddValue("Position", (int)Position);
            info.AddValue("Shift", (int)Shift);
        }
        public override bool Validate()
        {
            try
            {
                if (string.IsNullOrEmpty(UserName))
                    throw new StaffValidationException("Tên người dùng không được để trống");

                if (string.IsNullOrEmpty(PhoneNumber))
                    throw new StaffValidationException("Số điện thoại không được để trống");

                if (!_validateAge(DateOfBirth))
                    throw new StaffValidationException("Tuổi nhân viên phải từ 18 đến 60");

                return true;
            }
            catch (StaffValidationException)
            {
                return false;
            }
        }
        public override string GenerateId()
        {
            string tienTo = "";
            switch(Position)
            {
                case StaffPosition.Manager:
                    tienTo = "MA";
                    break;
                case StaffPosition.Seller:
                    tienTo = "SE";
                    break;
                case StaffPosition.Accountant:
                    tienTo = "AC";
                    break;
                case StaffPosition.InventoryManager:
                    tienTo = "IM";
                    break;
                default:
                    tienTo = "ST";
                    break;
            }
            return tienTo + DateTime.Now.ToString("yyyyMMdd");
        }

        public int CalculateAge()
        {
            return CalculateAge(DateOfBirth);
        }
        private int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }

        // Phương thức protected virtual để cho phép lớp con override
        protected virtual void OnShiftChanged(ShiftType oldShift, ShiftType newShift)
        {
            // Kiểm tra xem có subscriber nào không
            ShiftChanged?.Invoke(this, new ShiftChangedEventArgs(oldShift, newShift));
        }
    }
}
