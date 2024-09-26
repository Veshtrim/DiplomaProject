using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Models.ViewModels;

namespace TemaEDiplomesUBT.Services.IServices
{
    public interface IDayOffService
    {
        Task<List<DayOffViewModel>> GetAllAsync();
        Task<DayOffViewModel> GetByIdAsync(int id);
        Task AddAsync(DayOffViewModel dayOffViewModel);
        Task UpdateAsync(DayOffViewModel dayOffViewModel);
        Task DeleteAsync(int id);
        Task<DayOffViewModel> GetDayOffForTomorrowAsync();
    }
}
