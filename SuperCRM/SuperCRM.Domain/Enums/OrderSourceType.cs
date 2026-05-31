using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCRM.Domain.Enums
{
    public enum OrderSourceType : byte
    {
        Unknown = 0,
        Admin = 1,
        Agent = 2,
        Customer = 3,
        Imported = 4
    }
}
