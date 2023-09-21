using TimeClock.Domain;

namespace TimeClock.Controllers.V1.Requests
{
    public class UpdateShiftRequest
    {
        public string UpdateShiftType { get; set; }
        public int ShiftId { get; set; }
    }
}
