using TimeClock.Controllers.V1.Requests;
using TimeClock.Domain;

namespace TimeClock.Services
{
    public interface IShiftService
    {
        public  Task<List<Shift>> GetShiftsAsync(string userId, bool? isActive);
        public Task<List<Shift>> GetShiftsByUserIdAsync(string userId);
        public Task<Shift> CreateShiftAsync(string userId);
        public Task<UpdateShiftResult> UpdateShiftAsync(UpdateShiftType updateShiftType, int shiftId);

    }
}
