using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.DataAccess
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> getAllUsersAsync();
    }
}
