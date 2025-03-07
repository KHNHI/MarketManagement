using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement
{
    public interface IManager<T> where T : BaseEntity
    {
        bool Add(T item);
        bool Update(T item);
        bool Remove(string id);
        T GetById(string id);
        List<T> GetAll();
    }
}
