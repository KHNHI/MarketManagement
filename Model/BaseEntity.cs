using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement
{
    public abstract class BaseEntity
    {
        protected string id;
        public string Id
        {
            get { return id; }
            set { id = value ?? GenerateId(); }
        }

        protected BaseEntity()
        {
            Id = GenerateId();
        }

        public abstract string GenerateId();
       
        public abstract bool Validate();
    }
}
