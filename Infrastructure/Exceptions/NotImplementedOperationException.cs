using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Exceptions
{
    public class NotImplementedOperationException: Exception
    {
        public NotImplementedOperationException(string message): base(message) { }
    }
}
