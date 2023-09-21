using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection.Metadata.Ecma335;
using TimeClock.Controllers.V1.Requests;
using TimeClock.Domain;
using TimeClock.Extensions;
using WebApplication1.Data;

namespace TimeClock.Services
{
    public class ShiftService : IShiftService
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<ShiftService> _logger;

        public ShiftService(DataContext dataContext, ILogger<ShiftService> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }
        public async Task<Shift> CreateShiftAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogError("User does not exist");
                    throw new Exception("User does not exist");
                }

                bool isAdmin = _dataContext.IsAdmin(userId);

                if (_dataContext.Shifts.Any(x => x.UserId == userId && x.IsActive == true) && isAdmin == false)
                {
                    _logger.LogError("User already has an active shift. Can't start new one");
                    throw new Exception("User already has an active shift. Can't start new one");
                }

                Shift shift = new Shift
                {
                    UserId = userId,
                    StartTime = DateTime.Now,
                    EndTime = null,
                    LunchStartTime = null,
                    LunchEndTime = null,
                    BreakStartTime = null,
                    BreakEndTime = null,
                    IsActive = true

                };

                await _dataContext.AddAsync(shift);
                var created = await _dataContext.SaveChangesAsync();
                return shift;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to create shift for userId:{userId}");
                throw;
            }
        }

        public async Task<List<Shift>> GetShiftsAsync(string userId = null, bool? isActive = null)
        {
            var queryable = _dataContext.Shifts.AsQueryable();
            if (!string.IsNullOrEmpty(userId))
            {
                queryable = queryable.Where(x => x.UserId == userId);
            }
            if (isActive != null)
            {
                queryable = queryable.Where(x => x.IsActive == isActive);
            }
            return await queryable.OrderByDescending(x => x.IsActive).OrderByDescending(x => x.StartTime).ToListAsync();
        }

        public async Task<List<Shift>> GetShiftsByUserIdAsync(string userId)
        {
            return await _dataContext.Shifts.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<UpdateShiftResult> UpdateShiftAsync(UpdateShiftType updateShiftType, int shiftId)
        {
            var shift = await _dataContext.Shifts.FirstOrDefaultAsync(x => x.Id == shiftId);

            if (shift == null)
            {
                _logger.LogError("Shift not found. Cannot update shift that doesn't exist");
                throw new Exception();
            }

            var updateShiftResult = UpdateShiftHandler(updateShiftType, shift);
            if (updateShiftResult.IsSuccess)
            {
                var updatedShift = updateShiftResult.Shift;
                _dataContext.Update(updatedShift);
                await _dataContext.SaveChangesAsync();
                return updateShiftResult;
            }
            return updateShiftResult;

        }

        private UpdateShiftResult UpdateShiftHandler(UpdateShiftType updateShiftType, Shift shift)
        {
            try
            {
                bool isAdmin = _dataContext.IsAdmin(shift.UserId);

                if (updateShiftType == UpdateShiftType.StartLunch)
                {
                    StartLunchTime(shift, isAdmin);
                }
                else if (updateShiftType == UpdateShiftType.EndLunch)
                {
                    EndLunchTime(shift, isAdmin);
                }
                else if (updateShiftType == UpdateShiftType.StartBreak)
                {
                    StartBreakTime(shift, isAdmin);
                }
                else if (updateShiftType == UpdateShiftType.EndBreak)
                {
                    EndBreakTime(shift, isAdmin);
                }
                else if (updateShiftType == UpdateShiftType.EndShift)
                {
                    EndShift(shift, isAdmin);
                }
                return new UpdateShiftResult
                {
                    IsSuccess = true,
                    Shift = shift
                };
            }
            catch (Exception ex)
            {
                return new UpdateShiftResult { IsSuccess = false, Error = ex.Message };
            }
        }

        private void StartLunchTime(Shift shift, bool isAdmin)
        {
            if (shift.IsActive == false)
            {
                _logger.LogError("Shift is not active. Cannot start lunch on inactive shift");
                throw new Exception("Shift is not active. Cannot start lunch on inactive shift");
            }
            else if (shift.LunchStartTime != null && !isAdmin)
            {
                _logger.LogError("Lunch break is already active");
                throw new Exception("Lunch break is already active");
            }
            shift.LunchStartTime = DateTime.Now;
        }
        private void EndLunchTime(Shift shift, bool isAdmin)
        {
            if (shift.IsActive == false)
            {
                _logger.LogError("Shift is not active. Cannot start lunch on inactive shift");
                throw new Exception("Shift is not active. Cannot start lunch on inactive shift");
            }
            else if (shift.LunchStartTime == null && !isAdmin)
            {
                _logger.LogError("Lunch break was never started so cannot end it");
                throw new Exception("Lunch break was never started so cannot end it");
            }
            shift.LunchEndTime = DateTime.Now;
        }

        private void StartBreakTime(Shift shift, bool isAdmin)
        {
            if (shift.IsActive == false)
            {
                _logger.LogError("Shift is not active. Cannot start break on inactive shift");
                throw new Exception("Shift is not active. Cannot start break on inactive shift");
            }
            else if (shift.BreakStartTime != null && !isAdmin)
            {
                _logger.LogError("Break is already active");
                throw new Exception("Break is already active");
            }
            shift.BreakStartTime = DateTime.Now;
        }
        private void EndBreakTime(Shift shift, bool isAdmin)
        {
            if (shift.IsActive == false)
            {
                _logger.LogError("Shift is not active. Cannot end break on inactive shift");
                throw new Exception("Shift is not active. Cannot end break on inactive shift");
            }
            else if (shift.BreakStartTime == null && !isAdmin)
            {
                _logger.LogError("Break was never started so cannot end it");
                throw new Exception("Break was never started so cannot end it");
            }
            shift.BreakEndTime = DateTime.Now;
        }

        private void EndShift(Shift shift, bool isAdmin)
        {
            if (shift.IsActive == false)
            {
                _logger.LogError("Shift is not active. Cannot end shift that's not active");
                throw new Exception("Shift is not active. Cannot end shift that's not active");
            }
            if (LunchOrBreakIsActive(shift, isAdmin))
            {
                _logger.LogError("Lunch or break is currently active. Cannot end shift while lunch or break is active");
                throw new Exception("Lunch or break is currently active. Cannot end shift while lunch or break is active");
            }
            shift.EndTime = DateTime.Now;
            shift.IsActive = false;
            shift.TotalHours = CalculateTotalHours(shift);
        }

        private bool LunchOrBreakIsActive(Shift shift, bool isAdmin)
        {
            if (((shift.BreakStartTime != null && shift.BreakEndTime == null) || (shift.LunchStartTime != null && shift.LunchEndTime == null)) && !isAdmin)
            {
                return true;
            }
            return false;
        }

        private double CalculateTotalHours(Shift shift)
        {
            double totalHours = 0;
            if (shift.StartTime != null && shift.EndTime != null)
            {
                totalHours = (shift.EndTime - shift.StartTime).Value.TotalHours;
            }
            if (shift.BreakStartTime != null && shift.BreakEndTime != null)
            {
                totalHours -= (shift.BreakEndTime - shift.BreakStartTime).Value.TotalHours;
            }
            if (shift.LunchStartTime != null && shift.LunchEndTime != null)
            {
                totalHours -= (shift.LunchStartTime - shift.LunchEndTime).Value.TotalHours;
            }
            return totalHours;
        }

    }
}

