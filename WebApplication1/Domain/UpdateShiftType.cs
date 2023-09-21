using System.Text.Json.Serialization;

namespace TimeClock.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UpdateShiftType
    {
        StartLunch,
        EndLunch,
        StartBreak,
        EndBreak,
        EndShift
    }
}