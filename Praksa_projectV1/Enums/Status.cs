using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.Enums
{
    public enum Status
    {
        [Description("U pripremi")]
        Priprema,
        [Description("U tijeku")]
        Aktivan,
        [Description("završen")]
        Završen,

    }
}
