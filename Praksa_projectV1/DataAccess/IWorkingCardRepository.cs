using Praksa_projectV1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Praksa_projectV1.DataAccess
{
    public interface IWorkingCardRepository
    {
        Task<bool> Add(WorkingCard newCard);
        Task<bool> EditAsync(WorkingCard updateCard);
        Task<bool> DeleteByIdAsync(int id);
        Task<IEnumerable<WorkingCard>> GetByStartAndEndDate(DateOnly? startDate, DateOnly? endDate);
        Task<IEnumerable<MonthlySummary>> GetSummarizedDataByMonth(DateOnly startDate, DateOnly endDate);
        }
}
