# Stewards of Sosaria v1 - Development Kickoff

## Implemented in this commit

- C# core domain scaffolding for `TownAggregate`, `TownTask`, `TownNpcProfile`, and related enums.
- Service contracts for town creation/claim checks, task queue/dependency handling, and possession policy checks.
- Initial possession policy implementation aligned to v1 kickoff constraints:
  - allowed roles: Governor/Captain/Quartermaster/Magistrate
  - blocked region restriction
  - 30-second recent-combat restriction
  - cooldown gate handling

## Next coding steps

1. Implement queue storage + stable priority ordering and reservation expiry sweeper.
2. Implement deed placement command/item behavior using approved IDs:
   - deed ItemID: `0x14EF`
   - town center markers: `0x144C` and `0x144B`
3. Implement boundary checks using the fixed 60x61 rectangle policy around center point.
4. Add audit event sink and append entries for governance and possession actions.

## Notes

This scaffold is intentionally additive and designed to wrap upstream Ultima-Adventures primitives instead of replacing core server systems.


## RunUO script placement

For shard script compilation, place sources under `Custom/StewardsOfSosaria/...`.

- Added concrete `TownService` and `TaskService` implementations for v1 overlap checks, task prioritization, dependency resolution, and reservation-expiry sweeping.

- Added `docs/testing-checkpoints.md` with explicit “when to test” milestones for compile, deed placement, boundaries, tasks, reservations, and possession gates.

- Added `AuditService` with append/recent/by-town query support and wired it into runtime + town/task/deed flows for early governance telemetry.

- Added `[TownAudit]` command registration for player-visible audit inspection of recent event history.

- Added `[TownTaskReprio]` and `[TownInfo]` commands to support in-game queue tuning and quick settlement metadata checks.

- Added dependency/reservation helper commands (`[TownTaskDepend]`, `[TownTaskResolve]`, `[TownTaskReserveTest]`, `[TownTaskExpire]`) for rapid in-shard task-system validation.

- Added `[TownTaskSetStatus]` helper command to toggle task state and validate dependency resolution transitions.

- Added v1 shell read-model commands `[Town]`, `[TownTask]`, and `[TownNpc]` to provide a stable operator-facing entry point while gumps are pending.

- Added `StewardsPersistenceSerializer` under `Custom/StewardsOfSosaria/Persistence` to provide versioned shard-compatible read/write helpers for `TownAggregate`, `TownTask`, `ReservationToken`, and `TownNpcProfile`.

- Added `[TownPossessCheck]` command to exercise possession policy gates in-shard and append `PossessionAttempt` audit events.
