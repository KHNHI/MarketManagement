using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketManagement.Model.Exceptions
{
    public class StaffValidationException : Exception
    {
        public StaffValidationException() : base() { }

        public StaffValidationException(string message) : base(message) { }

        public StaffValidationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
