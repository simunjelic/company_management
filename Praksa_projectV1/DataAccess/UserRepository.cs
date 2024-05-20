using Microsoft.EntityFrameworkCore;
using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Praksa_projectV1.DataAccess
{
    public class UserRepository : IUserRepository
    {
        private readonly Context _dbContext;

        public async Task<User> AuthenticateUserAsync(NetworkCredential credential)
        {
            bool validUser = false;
            try
            {

                // Create a new instance of your DbContext
                using (var dbContext = new Context())
                {
                    // Check if there is a user with the provided username and password
                    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == credential.UserName && u.Password == HashPassword(credential.Password));

                    return user;
                }
            }catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return null;
            }

            

        }

        public IEnumerable<User> getAllUsers()
        {
            try { 
            using (var dbContext = new Context())
            {
                return dbContext.Users.ToList();
            }
            }catch (Exception ex)
            {
                ExceptionHandlerRepository.LogUnhandledExceptionSync(ex, ex.Source ?? "Source null");
                return Enumerable.Empty<User>();
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return null;

            }
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
                ExceptionHandlerRepository.LogUnhandledExceptionSync(ex, ex.Source ?? "Source null");
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return null;
            }
        }

        internal async Task<bool> RemoveUserRoleAsync(UserRole selectedRole)
        {
            try
            {
                using (var context = new Context())
                {

                    context.UserRoles.Remove(selectedRole);
                    var RowsAffected = await context.SaveChangesAsync();
                    return RowsAffected > 0;

                }

            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            }
        }

        internal async Task<bool> AddUserRoleAsync(UserRole newUserRole)
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
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return null;
            }
        }

        internal User GetUserByUsername(string username)
        {
            try
            {
                using (var dbContext = new Context())
                {
                    return dbContext.Users.FirstOrDefault(i => i.Username == username);
                }

            }
            catch (Exception ex)
            {
                ExceptionHandlerRepository.LogUnhandledExceptionSync(ex, ex.Source ?? "Source null");
                return null;

            }
        }

        internal async Task<bool> AddUser(NetworkCredential networkCredential)
        {
            try
            {
                using (var context = new Context())
                {
                    var check = context.Users.Any(i => i.Username == networkCredential.UserName);
                    if (!check)
                    {
                        User user = new();
                        user.Username = networkCredential.UserName;
                        user.Password = HashPassword(networkCredential.Password);
                        context.Users.Add(user);
                        int rowsAffected = await context.SaveChangesAsync();
                        return rowsAffected > 0;
                    }
                    return false;



                }

            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            }
        }

        internal async Task<bool> EditUserAsync(NetworkCredential networkCredential, int id)
        {
            try
            {
                using (var context = new Context())
                {

                    User user = await context.Users.FirstOrDefaultAsync(i =>  i.Id == id);
                    if (user != null)
                    {
                        user.Username = networkCredential.UserName;
                        user.Password = HashPassword(networkCredential.Password);
                        user.Id = id;
                        context.Users.Update(user);
                        int rowsAffected = await context.SaveChangesAsync();
                        return rowsAffected > 0;
                    }
                    return false;
                }

            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            }
        }

        internal async Task<bool> EditUserUsername(string username, int id)
        {
            try
            {
                using (var context = new Context())
                {
                    var user = context.Users.FirstOrDefault(i => i.Id == id);
                    if (user != null)
                    {
                        user.Username = username;
                        context.Users.Update(user);
                        int rowsAffected = await context.SaveChangesAsync();
                        return rowsAffected > 0;
                    }
                    return false;



                }

            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            }
        }

        internal async Task<bool> RemoveByIdAsync(int id)
        {
            try
            {
                using (var context = new Context())
                {
                    var user = await context.Users.FirstOrDefaultAsync(i => i.Id == id);
                    if (user != null)
                    {
                        context.Users.Remove(user);
                        await context.SaveChangesAsync();
                        return true;
                    }
                    return false;

                }

            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return false;
            }
        }
        private string HashPassword(string password)
        {
            // Use a secure hashing algorithm (e.g., SHA-256) to hash the password
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        internal async Task<IEnumerable<UserRole>> GetUserRolesAsync(int id)
        {
            try
            {
                using (var dbContext = new Context())
                {
                    var roles = await dbContext.UserRoles
                        .Include(e => e.Role)
              .Where(u => u.UserId == id)
              .ToListAsync();

                    return roles;
                }
            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return null;
            }
        }

        public async Task<IEnumerable<User>> getAllUsersAsync()
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
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return Enumerable.Empty<User>();
            }
        }

        public async Task<Employee> getEmployeeByUsernameAsync(string username)
        {
            try
            {
                using (var dbContext = new Context())
                {
                    return await dbContext.Employees.FirstOrDefaultAsync(i => i.User.Username == username);
                }

            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");
                return null;

            }
        }
    }
}



