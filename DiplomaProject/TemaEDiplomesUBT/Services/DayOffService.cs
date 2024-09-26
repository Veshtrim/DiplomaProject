using TemaEDiplomesUBT.Data;
using TemaEDiplomesUBT.Models.ViewModels;
using TemaEDiplomesUBT.Models;
using TemaEDiplomesUBT.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace TemaEDiplomesUBT.Services
{
    public class DayOffService : IDayOffService
    {
        private readonly ApplicationDbContext _context;

        public DayOffService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DayOffViewModel>> GetAllAsync()
        {
            return await _context.DayOff
                .Select(d => new DayOffViewModel
                {
                    Id = d.Id,
                    DayOffDate = d.DayOffDate,
                    Reason = d.Reason
                }).ToListAsync();
        }

        public async Task<DayOffViewModel> GetByIdAsync(int id)
        {
            return await _context.DayOff
                .Where(d => d.Id == id)
                .Select(d => new DayOffViewModel
                {
                    Id = d.Id,
                    DayOffDate = d.DayOffDate,
                    Reason = d.Reason
                }).FirstOrDefaultAsync();
        }

        public async Task AddAsync(DayOffViewModel dayOffViewModel)
        {
            var dayOff = new DayOff
            {
                DayOffDate = dayOffViewModel.DayOffDate,
                Reason = dayOffViewModel.Reason
            };
            _context.DayOff.Add(dayOff);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(DayOffViewModel dayOffViewModel)
        {
            var dayOff = await _context.DayOff.FindAsync(dayOffViewModel.Id);
            if (dayOff != null)
            {
                dayOff.DayOffDate = dayOffViewModel.DayOffDate;
                dayOff.Reason = dayOffViewModel.Reason;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var dayOff = await _context.DayOff.FindAsync(id);
            if (dayOff != null)
            {
                _context.DayOff.Remove(dayOff);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<DayOffViewModel> GetDayOffForTomorrowAsync()
        {
            var tomorrow = DateTime.Now.AddDays(1).Date;

            return await _context.DayOff
                .Where(d => d.DayOffDate == tomorrow)
                .Select(d => new DayOffViewModel
                {
                    Id = d.Id,
                    DayOffDate = d.DayOffDate,
                    Reason = d.Reason
                })
                .FirstOrDefaultAsync();
        }
    }
}
