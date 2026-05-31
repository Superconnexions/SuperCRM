using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCRM.Domain.Enums
{
    public enum SalesOrderStatus : byte
    {
        Unknown = 0,
        Created = 1,
        SentToProvider = 2,
        ProviderAccepted = 3,
        ProviderRejected = 4,
        Processing = 5,
        Shipped = 6,
        Completed = 7,
        PartialDelivered = 8,
        OnHold = 9,
        Cancelled = 10
    }
}
