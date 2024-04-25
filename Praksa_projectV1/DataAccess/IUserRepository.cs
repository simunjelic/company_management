
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.DataAccess
{
    interface IUserRepository
    {
        bool AuthenticateUser(NetworkCredential credential);
        void add(User user);
        void remove(User user);
        void update(User user);
        User getUser(int id);
        Task<User> getByUsernameAsync(string username);
        IEnumerable<User> getAllUsers();
        Task<Employee> getEmployeeByUsernameAsync(string username);
        List<string> GetUserRoles(string username);
    }
}
