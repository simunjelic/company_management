using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Praksa_projectV1.DataAccess
{
    public class UserRepository
    {
        private readonly Context _dbContext;
        public void add(User user)
        {
            throw new NotImplementedException();
        }

        public bool AuthenticateUser(NetworkCredential credential)
        {
            bool validUser = false;

            // Create a new instance of your DbContext
            using (var dbContext = new Context())
            {
                // Check if there is a user with the provided username and password
                var user = dbContext.Users.FirstOrDefault(u => u.Username == credential.UserName && u.Password == credential.Password);

                // If a user is found, set validUser to true
                if (user != null)
                {
                    validUser = true;
                }
            }

            return validUser;

        }

        public IEnumerable<User> getAllUsers()
        {
            using (var dbContext = new Context())
            {
                return dbContext.Users.ToList();
            }
        }

        public async Task<User> getByUsernameAsync(string username)
        {
            try
            {
                using (var dbContext = new Context())
                {
                    return await dbContext.Users.FirstOrDefaultAsync(i => i.Username == username);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("User not found");
                return null;

            }
        }
        public Employee getEmployeeByUsername(string username)
        {
            try
            {
                using (var dbContext = new Context())
                {
                    return dbContext.Employees.FirstOrDefault(i => i.User.Username == username);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }

        public User getUser(int id)
        {
            throw new NotImplementedException();
        }

        public void remove(User user)
        {
            throw new NotImplementedException();
        }

        public void update(User user)
        {
            throw new NotImplementedException();
        }
        public List<string> GetUserRoles(string username)
        {
            try
            {
                using (var dbContext = new Context())
                {
                    var roles = dbContext.Users
              .Where(u => u.Username == username)
              .SelectMany(u => u.UserRoles)
              .Select(ur => ur.Role.RoleName)
              .ToList();

                    return roles;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("GREŠKA");
                return null;
            }
        }

        internal async Task<IEnumerable<User>> getAllUsersAsync()
        {
            try
            {
                using (var dbContext = new Context())
                {
                    return await dbContext.Users
                 .Include(u => u.Employees)
                .Include(u => u.UserRoles)
                .ToListAsync();
                }

            }
            catch (Exception ex) 
            {
                
                return null;
            }
        }

        internal async Task<IEnumerable<UserRole>> GetUserRolesObject(int id)
        {
            try
            {
                using (var dbContext = new Context())
                {
                    return await dbContext.UserRoles
                 .Include(u => u.User)
                .Include(u => u.Role)
                .Where(u => u.User.Id == id)
                .ToListAsync();
                }

            }
            catch (Exception ex)
            {

                return null;
            }
        }

        internal async Task<IEnumerable<Role>> GetRoles()
        {
            try
            {
                using (var dbContext = new Context())
                {
                    return await dbContext.Roles
                .ToListAsync();
                }

            }
            catch (Exception ex)
            {

                return null;
            }
        }

        internal async Task<bool> RemoveUserRole(UserRole selectedRole)
        {
            try
            {
                using (var context = new Context())
                {

                    context.UserRoles.Remove(selectedRole);
                    await context.SaveChangesAsync();
                    return true;

                }

            }
            catch
            {
                return false;
            }
        }

        internal async Task<bool> AddUserRole(UserRole newUserRole)
        {
            try
            {
                using (var context = new Context())
                {


                    context.UserRoles.Add(newUserRole);


                    int rowsAffected = await context.SaveChangesAsync();
                    return rowsAffected > 0;

                }

            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        internal async Task<IEnumerable<User>> FilterData(string searchQuery)
        {
            try
            {
                using (var context = new Context())
                {
                    return await context.Users
                              .Include(u => u.Employees)
                              .Include(u => u.UserRoles)
                              .Where(p => p.Username.Contains(searchQuery) || p.Id.ToString().Contains(searchQuery))
                              .ToListAsync();
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    }



