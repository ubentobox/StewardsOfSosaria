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

1. Add persistence adapters that serialize `TownAggregate`, `TownTask`, and `TownNpcProfile` using shard-compatible serializers.
2. Implement queue storage + stable priority ordering and reservation expiry sweeper.
3. Implement deed placement command/item behavior using approved IDs:
   - deed ItemID: `0x14EF`
   - town center markers: `0x144C` and `0x144B`
4. Implement boundary checks using the fixed 60x61 rectangle policy around center point.
5. Add initial gump shell commands (`[Town]`, `[TownTask]`, `[TownNpc]`) wired to read-model stubs.
6. Add audit event sink and append entries for governance and possession actions.

## Notes

This scaffold is intentionally additive and designed to wrap upstream Ultima-Adventures primitives instead of replacing core server systems.


## RunUO script placement

For shard script compilation, place sources under `Custom/StewardsOfSosaria/...`.

- Added concrete `TownService` and `TaskService` implementations for v1 overlap checks, task prioritization, dependency resolution, and reservation-expiry sweeping.

- Added `docs/testing-checkpoints.md` with explicit “when to test” milestones for compile, deed placement, boundaries, tasks, reservations, and possession gates.

- Added `AuditService` with append/recent/by-town query support and wired it into runtime + town/task/deed flows for early governance telemetry.

- Added `[TownAudit]` command registration for player-visible audit inspection of recent event history.
