using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Praksa_projectV1.Commands
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }

}
