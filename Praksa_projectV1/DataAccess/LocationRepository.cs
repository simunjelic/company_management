using Praksa_projectV1.Models;
using Microsoft.EntityFrameworkCore;

namespace Praksa_projectV1.DataAccess
{
    public class LocationRepository : ILocationRepository
    {
        public async Task<bool> AddAsync(Location location)
        {
            try
            {
                using (var dContext = new Context())
                {
                    await dContext.Locations.AddAsync(location);
                    var RowsAffected = await dContext.SaveChangesAsync();
                    return RowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");

                return false;
            }
        }

        public async Task<bool> DeleteAsync(Location selectedItem)
        {
            try
            {
                using(var dContext = new Context())
                {
                    dContext.Locations.Remove(selectedItem);
                    var RowsAffected = await dContext.SaveChangesAsync();
                    return RowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");

                return false;
            }
        }

        public async Task<bool> EditAsync(Location selectedItem)
        {
            try
            {
                using (var dContext = new Context())
                {
                    dContext.Locations.Update(selectedItem);
                    var RowsAffected = await dContext.SaveChangesAsync();
                    return RowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");

                return false;
            }
        }

        public async Task<IEnumerable<Location>> GetAllLocationsAsync()
        {
            try
            {
                using (var dContext = new Context())
                {

                    return await dContext.Locations.ToListAsync();
                }
            }
            catch (Exception ex)
            {
                await ExceptionHandlerRepository.LogUnhandledException(ex, ex.Source ?? "Source null");

                return Enumerable.Empty<Location>();
            }
        }
    }


}


