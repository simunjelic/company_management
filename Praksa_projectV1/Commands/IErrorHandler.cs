using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.Commands
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }

}
