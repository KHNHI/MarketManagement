using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement
{
    //public abstract class BaseManager<T> : IManager<T> where T : BaseEntity
    //{
    //    protected List<T> items;
    //    protected readonly FileHandler fileHandler;

    //    protected BaseManager(string fileName)
    //    {
    //        fileHandler = new FileHandler(fileName);
    //        LoadData();
    //    }

    //    protected virtual void LoadData()
    //    {
    //        items = fileHandler.LoadFromFile<List<T>>();
    //        if (items == null)
    //        {
    //            items = new List<T>();
    //        }
    //    }

    //    protected virtual void SaveChanges()
    //    {
    //        fileHandler.SaveToFile(items);
    //    }

    //    public virtual void Add(T item)
    //    {
    //        if (!item.Validate())
    //        {
    //            throw new Exception("Invalid data");
    //        }

    //        // Kiểm tra ID trùng
    //        for (int i = 0; i < items.Count; i++)
    //        {
    //            if (items[i].Id == item.Id)
    //            {
    //                throw new Exception("ID already exists");
    //            }
    //        }

    //        items.Add(item);
    //        SaveChanges();
    //    }

    //    public virtual void Update(T item)
    //    {
    //        if (!item.Validate())
    //        {
    //            throw new Exception("Invalid data");
    //        }

    //        bool found = false;
    //        for (int i = 0; i < items.Count; i++)
    //        {
    //            if (items[i].Id == item.Id)
    //            {
    //                items[i] = item;
    //                found = true;
    //                break;
    //            }
    //        }

    //        if (!found)
    //        {
    //            throw new Exception("Item not found");
    //        }

    //        SaveChanges();
    //    }

    //    public virtual void Remove(string id)
    //    {
    //        bool found = false;
    //        for (int i = 0; i < items.Count; i++)
    //        {
    //            if (items[i].Id == id)
    //            {
    //                items.RemoveAt(i);
    //                found = true;
    //                break;
    //            }
    //        }

    //        if (found)
    //        {
    //            SaveChanges();
    //        }
    //    }

    //    public virtual T GetById(string id)
    //    {
    //        for (int i = 0; i < items.Count; i++)
    //        {
    //            if (items[i].Id == id)
    //            {
    //                return items[i];
    //            }
    //        }
    //        return null;
    //    }

    //    public virtual List<T> GetAll()
    //    {
    //        return items;
    //    }
    //}
}
