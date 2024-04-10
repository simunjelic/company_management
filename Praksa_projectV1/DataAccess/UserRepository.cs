using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.DataAccess
{
    public class UserRepository : IUserRepository
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
            throw new NotImplementedException();
        }

        public User getByUsername(string username)
        {
            throw new NotImplementedException();
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
    }

       
    }
