namespace TimeClock.Domain
{
    public class UpdateShiftResult
    {
        public Shift Shift { get; set; }
        public bool IsSuccess { get; set; }

        public string Error { get; set; }
    }
}
