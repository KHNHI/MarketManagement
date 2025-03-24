using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement.Model
{
    public class OtherProduct : BaseProduct
    {
        public OtherProduct() : base()
        {
            Category = ProductCategory.Other;
        }
    }
}
