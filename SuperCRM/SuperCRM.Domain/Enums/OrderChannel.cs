using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCRM.Domain.Enums
{
    public enum OrderChannel: byte
    {
        Unknown = 0,
        WebPortal = 1,
        MobileApp = 2,
        Facebook = 3,
        Instagram = 4,
        Advertisement = 5,
        Affiliation = 6,
        SocialMedia = 7,
        Adsense = 8

    }
}
