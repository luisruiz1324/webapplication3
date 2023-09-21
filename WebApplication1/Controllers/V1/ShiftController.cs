using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TimeClock.Controllers.V1.Requests;
using TimeClock.Controllers.V1.Responses;
using TimeClock.Domain;
using TimeClock.Extensions;
using TimeClock.Services;

namespace TimeClock.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Non-Admin")] 
    public class ShiftController : Controller
    {
        private readonly IShiftService _shiftService;

        public ShiftController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("api/v1/allshifts")]
        public async Task<IActionResult> GetAll([FromQuery] string userId, bool? isActive)
        {
            var shifts = await _shiftService.GetShiftsAsync(userId, isActive);
            if (!shifts.Any())
            {
                return NotFound($"No shifts found");
            }
            return Ok(shifts);
        }


        [HttpGet("api/v1/shifts")]
        public async Task<IActionResult> GetShiftsByUserId()
        {
            var userId = HttpContext.GetUserId();
            var shifts = await _shiftService.GetShiftsByUserIdAsync(userId);
            if (!shifts.Any())
            {
                return NotFound($"No shifts found for userID:{userId}");
            }
            return Ok(shifts);
        }

        [HttpPost("api/v1/shifts")] 
        public async Task<IActionResult> CreateShift([FromQuery] bool isAdmin)
        {
            try
            {
                var userId = HttpContext.GetUserId();
                var shift = await _shiftService.CreateShiftAsync(userId, isAdmin);
                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
                var locationUri = baseUrl + "/" + "v1/api/shifts/{userId}";
                return Created(locationUri, shift.Id);
            }catch(Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

        [HttpPut("api/v1/shifts")]
        public async Task<IActionResult> UpdateShift([FromBody] UpdateShiftRequest updateShiftRequest, [FromQuery] bool isAdmin)
        {
            UpdateShiftType updateShiftType;
            var enumExists = Enum.TryParse<UpdateShiftType>(updateShiftRequest.UpdateShiftType, out updateShiftType);
            if (!enumExists)
            {
                return Conflict(new UpdateShiftFailedResponse
                {
                    Error = $"{updateShiftRequest.UpdateShiftType} is not a valid shift update type."
                });
            }
            var updateShiftResult = await _shiftService.UpdateShiftAsync(updateShiftType, updateShiftRequest.ShiftId, isAdmin);
            if(!updateShiftResult.IsSuccess)
            {
                return Conflict(new UpdateShiftFailedResponse
                {
                    Error = updateShiftResult.Error,
                });
            }
            return Ok(new UpdateShiftSuccessResponse
            {
                Shift = updateShiftResult.Shift
            });


        }
  
        }
    }



