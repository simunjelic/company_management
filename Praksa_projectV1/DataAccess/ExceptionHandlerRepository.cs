using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.DataAccess
{
    public static class ExceptionHandlerRepository
    {
        public static async Task LogUnhandledException(Exception exception, string source)
        {
            using (var dbContext = new Context())
            {

                dbContext.ExceptionLogs.Add(new ExceptionLog
                {
                    Message = "User: " + LoggedUserData.Username + " Exception: " + exception.ToString(),
                    StackTrace = exception.StackTrace,
                    Source = source,
                    Timestamp = DateTime.UtcNow
                });
                await dbContext.SaveChangesAsync();
            }
        }
        public static void LogUnhandledExceptionSync(Exception exception, string source)
        {
            using (var dbContext = new Context())
            {

                dbContext.ExceptionLogs.Add(new ExceptionLog
                {
                    Message = "User: " + LoggedUserData.Username + " Exception: " + exception.ToString(),
                    StackTrace = exception.StackTrace,
                    Source = source,
                    Timestamp = DateTime.UtcNow
                });
                dbContext.SaveChanges();
            }
        }
    }
}
