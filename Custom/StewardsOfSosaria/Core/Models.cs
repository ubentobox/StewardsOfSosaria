using System;
using System.Collections.Generic;

namespace StewardsOfSosaria.Core
{
    public enum TownOwnerType
    {
        Player,
        Guild
    }

    public enum SimProfile
    {
        HiFi,
        LoFi
    }

    public enum TownTaskType
    {
        Gather,
        Haul,
        Craft,
        Build,
        Repair,
        Patrol,
        Train,
        Trade,
        Heal,
        Mission
    }

    public enum TownTaskStatus
    {
        Queued,
        Reserved,
        InProgress,
        Blocked,
        Done,
        Failed,
        Canceled
    }

    public enum NpcAutonomyMode
    {
        Auto,
        Directed,
        Possessed
    }

    public enum ThreatState
    {
        Normal,
        Warning,
        Raid,
        Siege
    }

    public sealed class TownMeters
    {
        public int Prosperity { get; set; }
        public int Security { get; set; }
        public int Health { get; set; }
        public int Morale { get; set; }
    }

    public sealed class TownAggregate
    {
        public TownAggregate()
        {
            Name = string.Empty;
            BoundaryRegionId = string.Empty;
            Width = 60;
            Height = 61;
            SimProfile = SimProfile.HiFi;
            ThreatState = ThreatState.Normal;
            Meters = new TownMeters();
            Version = 1;
        }

        public Guid TownId { get; set; }
        public string Name { get; set; }
        public TownOwnerType OwnerType { get; set; }
        public Guid OwnerId { get; set; }
        public string BoundaryRegionId { get; set; }
        public int CenterX { get; set; }
        public int CenterY { get; set; }
        public int CenterZ { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public SimProfile SimProfile { get; set; }
        public ThreatState ThreatState { get; set; }
        public TownMeters Meters { get; set; }
        public int Version { get; set; }
    }

    public sealed class TownNpcProfile
    {
        public TownNpcProfile()
        {
            AutonomyMode = NpcAutonomyMode.Auto;
        }

        public int NpcSerial { get; set; }
        public Guid TownId { get; set; }
        public NpcAutonomyMode AutonomyMode { get; set; }
        public DateTime PossessionCooldownUntilUtc { get; set; }
        public DateTime LastCombatUtc { get; set; }
    }

    public sealed class ReservationToken
    {
        public Guid TokenId { get; set; }
        public Guid TaskId { get; set; }
        public int ItemSerial { get; set; }
        public int Amount { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }

    public sealed class TownTask
    {
        public TownTask()
        {
            Status = TownTaskStatus.Queued;
            DependencyTaskIds = new List<Guid>();
            Reservations = new List<ReservationToken>();
            FailureReason = null;
        }

        public Guid TaskId { get; set; }
        public Guid TownId { get; set; }
        public TownTaskType Type { get; set; }
        public int Priority { get; set; }
        public TownTaskStatus Status { get; set; }
        public List<Guid> DependencyTaskIds { get; private set; }
        public List<ReservationToken> Reservations { get; private set; }
        public string FailureReason { get; set; }
    }
}
