using System;

namespace StewardsOfSosaria.Core
{
    public enum AuditEventType
    {
        TownFounded,
        TaskQueued,
        TaskReprioritized,
        TaskStarted,
        TaskCompleted,
        TaskFailed,
        ReservationExpired,
        PossessionAttempt
    }

    public sealed class AuditEntry
    {
        public DateTime UtcTime { get; set; }
        public AuditEventType EventType { get; set; }
        public string Actor { get; set; }
        public Guid TownId { get; set; }
        public string Details { get; set; }
    }
}
