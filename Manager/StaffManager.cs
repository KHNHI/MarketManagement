using MarketManagement.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace MarketManagement.Manager
{
    public class StaffManager : IManager<Staff>
    {
        private List<Staff> staffs;
        private readonly FileHandler fileHandler;
        private readonly JsonSerializerSettings jsonSettings;

        // Singleton 
        private static StaffManager instance;
        // Lock object for thread safety
        private static readonly object lockObject = new object();

        // Singleton accessor with thread safety
        public static StaffManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new StaffManager();
                        }
                    }
                }
                return instance;
            }
        }

        // Event đơn giản
        public event EventHandler StaffChanged;

        // Đổi constructor thành private
        private StaffManager()
        {
            fileHandler = new FileHandler("staffs");
            jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            LoadStaffs();
        }

        // Phương thức để kích hoạt sự kiện
        protected virtual void OnStaffChanged()
        {
            StaffChanged?.Invoke(this, EventArgs.Empty);
        }

        private void LoadStaffs()
        {
            try
            {
                // Sử dụng FileHandler để tải dữ liệu
                staffs = fileHandler.LoadFromFile<List<Staff>>(jsonSettings) ?? new List<Staff>();
            }
            catch (Exception)
            {
                staffs = new List<Staff>();
            }
        }

        private void SaveToFile()
        {
            try
            {
                // Sử dụng FileHandler để lưu dữ liệu
                fileHandler.SaveToFile(staffs, jsonSettings);
            }
            catch (Exception) { }
        }

        public bool Add(Staff staff)
        {
            for (int i = 0; i < staffs.Count; i++)
            {
                if (staffs[i].Id == staff.Id)
                {
                    throw new Exception("Staff ID already exists!");
                }
            }
            staffs.Add(staff);
            SaveToFile();

            OnStaffChanged();
            return true;
        }

        public bool Update(Staff staff)
        {
            for (int i = 0; i < staffs.Count; i++)
            {
                if (staffs[i].Id == staff.Id)
                {
                    staffs[i] = staff;
                    SaveToFile();
                    OnStaffChanged();
                    return true;
                }
            }
            return false;
        }

        public bool Remove(string staffId)
        {
            for (int i = 0; i < staffs.Count; i++)
            {
                if (staffs[i].Id == staffId)
                {
                    staffs.RemoveAt(i);
                    SaveToFile();
                    OnStaffChanged();
                    return true;
                }
            }
            return false;
        }

        public Staff GetById(string staffId)
        {
            for (int i = 0; i < staffs.Count; i++)
            {
                if (staffs[i].Id == staffId)
                {
                    return staffs[i];
                }
            }
            return null;
        }

        public List<Staff> GetAll()
        {
            return staffs;
        }

        public List<Staff> GetByPosition(StaffPosition position)
        {
            List<Staff> result = new List<Staff>();
            for (int i = 0; i < staffs.Count; i++)
            {
                if (staffs[i].Position == position)
                {
                    result.Add(staffs[i]);
                }
            }
            return result;
        }

        public List<Staff> GetByShift(ShiftType shift)
        {
            List<Staff> result = new List<Staff>();
            for (int i = 0; i < staffs.Count; i++)
            {
                if (staffs[i].Shift == shift)
                {
                    result.Add(staffs[i]);
                }
            }
            return result;
        }
    }
}