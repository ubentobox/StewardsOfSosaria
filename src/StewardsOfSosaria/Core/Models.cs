using System;
using System.Collections.Generic;

namespace StewardsOfSosaria.Core;

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
    public Guid TownId { get; set; }
    public string Name { get; set; } = string.Empty;
    public TownOwnerType OwnerType { get; set; }
    public Guid OwnerId { get; set; }
    public string BoundaryRegionId { get; set; } = string.Empty;
    public int CenterX { get; set; }
    public int CenterY { get; set; }
    public int CenterZ { get; set; }
    public int Width { get; set; } = 60;
    public int Height { get; set; } = 61;
    public SimProfile SimProfile { get; set; } = SimProfile.HiFi;
    public ThreatState ThreatState { get; set; } = ThreatState.Normal;
    public TownMeters Meters { get; set; } = new();
    public int Version { get; set; } = 1;
}

public sealed class TownNpcProfile
{
    public int NpcSerial { get; set; }
    public Guid TownId { get; set; }
    public NpcAutonomyMode AutonomyMode { get; set; } = NpcAutonomyMode.Auto;
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
    public Guid TaskId { get; set; }
    public Guid TownId { get; set; }
    public TownTaskType Type { get; set; }
    public int Priority { get; set; }
    public TownTaskStatus Status { get; set; } = TownTaskStatus.Queued;
    public List<Guid> DependencyTaskIds { get; } = new();
    public List<ReservationToken> Reservations { get; } = new();
    public string? FailureReason { get; set; }
}
