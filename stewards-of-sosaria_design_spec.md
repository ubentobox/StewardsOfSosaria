# Stewards of Sosaria
## FOSS Town-Management + NPC Command Mod for Ultima-Adventures (ServUO/RunUO-style)

---

## 0) Product Goals

**Design target:** a modular, open-source shard subsystem where players can found and run towns, while NPC citizens and guards execute autonomous and directed work.

**Core loop:**
1. Found or claim a town.
2. Set policies and designate zones.
3. Populate a persistent task queue.
4. NPCs self-assign/reserve work, haul goods, construct, patrol, and react to threats.
5. Player can intervene directly by temporary NPC possession during missions/combat.
6. Town metrics (prosperity/security/health/morale) shift based on outcomes.

**Non-goals (phase 1):** full grand-strategy economy simulation, infinite AI planning depth, and high-frequency simulation of unloaded regions.

---

# Stewards of Sosaria — v1 Kickoff Packet

This document describes the **Stewards of Sosaria** vertical slice and core decisions for `v1` of the project 
as implemented on the RunUO-based Ultima-Adventures server.

The sections below incorporate both the original design spec and the finalized 
decisions on ownership, PvP posture, possession rules, assets, boundaries, economy, 
roles/permissions, and region controls.

---

## 1) Shard Scope & Rules (V1)

**Ownership Modes Enabled:** Player | Guild  
(= no Faction in V1)

**PvP Posture (V1):** PvE-only threats (monster/bandit raids);  
No player or guild takeovers.

**NPC Possession Rules:**  
- Cooldowns: 15m/player, 10m/NPC  
- Must not have been in combat last 30s  
- Disallowed within dungeons / PvP regions / combat panic  
- Role requirements: Governor, Captain, Quartermaster, Magistrate

**Control Persistence:**  
Settlement ownership cannot be overridden by NPC raids.

---

## 2) Content IDs / Placeholders (Approved)

**Stewardship Deed**  
- ItemID: `0x14EF`  
- On double-click: target location → place `0x144C` at `<0,0,0>`,  
  and `0x144B` at `<-1,0,0>` to mark town center

**Settlement Boundary**  
- Rectangle 60x61 tiles around town center

**Gatehouse (placeholder)**  
- Multi ID `102 (0x66)` — "Small Fieldstone House"

**Fence Statics**  
- North corners: `0x0290`  
- South corners: `0x028F`  
- East/West fencing: `0x028D`  
- North/South fencing: `0x028E`

Gate opening = 4 missing fence pieces at chosen wall spot.

**Temporary Structures**  
- Use signposts + server-side zones as placeholder buildings

---

## 3) Domain Data Models

## 3.1 Town

```text
Town
- TownId (Guid/int)
- Name
- OwnerType (Player/Guild/Faction/NPC)
- OwnerId
- BoundaryRegionId + TileBounds
- Buildings[]
- Zones[] (stockpile, agriculture, patrol, no-build)
- Treasury (gold + ledgers)
- Resources (itemized + abstract)
- PopulationStats
- Policies
- Meters: Prosperity, Security, Health, Morale (0..100)
- Alerts[]
- ThreatState (normal/warning/raid/siege)
- SimProfile (HiFi/LoFi)
- Version
```

## 3.2 Town NPC Extension

```text
TownNpcProfile
- NpcSerial
- TownId
- Profession/Archetype
- Skills[] (base UO + derived tags)
- Traits[]
- Needs: Hunger/Thirst/Rest/Safety/Comfort
- MoodScore + MoodModifiers[]
- ScheduleBlocks[]
- PersonalGoals[]
- InventoryAccessProfile
- CurrentTaskId
- AutonomyMode (Auto/Directed/Possessed)
- PossessionCooldownUntil
```

## 3.3 Task Queue

```text
TownTask
- TaskId
- TownId
- Type (Gather/Haul/Craft/Build/Patrol/Train/Trade/Heal/Mission)
- Priority (0..n or Critical/High/Normal/Low)
- CreatedBy
- Status (Queued/Reserved/InProgress/Blocked/Done/Failed/Canceled)
- Dependencies[] (task ids or condition keys)
- ReservationTokens[]
- RequiredSkills[]
- RequiredTools[]
- RequiredMaterials[]
- TargetEntity/TargetLocation
- RetryPolicy
- Deadline/Window (optional)
- FailureReason
```

## 3.4 Construction Blueprint

```text
BlueprintDefinition
- BlueprintId
- Name
- Category
- MultiId OR PlaceholderPrefabId (requested from content team)
- Footprint + Rotation Rules
- StageDefinitions[]

BlueprintStage
- StageIndex
- Materials[]
- LaborUnits
- RequiredSkillThresholds
- ResultVisualState (scaffold/partial/complete)
```

> **Content rule:** For house/structure placement and multis, implementation must request and use approved `MultiId` and/or placeholder assets from content maintainers before enabling placement.

## 3.5 Stockpile + Zone Models

```text
Zone
- ZoneId
- TownId
- ZoneType (Stockpile/Patrol/Agriculture/Construction/Restricted)
- Polygon/Rect bounds
- Filters (item categories, allow/deny)
- Priority

StockpileEntry
- StockpileId
- ItemType
- Quantity
- ReservedQuantity
- MinTarget
- MaxTarget
```

## 3.6 Squad / Threat

```text
Squad
- SquadId
- TownId
- Role (Militia/Guard/Scout/Response)
- Members[]
- Order (Patrol/Hold/Intercept/Fallback)
- RouteWaypoints[]
- AlertLevelBinding
```

---

## 4) Required Systems (Implementation Design)

## 4.1 Persistent Task Queue (priority + dependencies + reservation)

- Central per-town priority queue with stable ordering.
- Dependency resolver supports:
  - explicit `TaskId` prerequisites,
  - condition gates (e.g., `Stockpile.Wood >= 100`),
  - stage gates from construction stages.
- Reservation service allocates material/tool/target locks with TTL heartbeats.
- Dead reservation recovery on disconnect/death.
- Duplicate-haul prevention via item/stack reservation token.

## 4.2 Blueprint Construction (staged materials/labor)

- Place `BlueprintGhost` after zoning + permission validation.
- Stage progression:
  1. Deliver required materials to site buffer.
  2. Consume labor units by assigned builders.
  3. Advance visual state and unlock next stage.
- Damage/raid can reduce stage completion (optional configurable).
- Building completion emits `BuildingOnline` event to unlock policies/tasks.

## 4.3 Stockpile Zoning + Hauling

- Item filters by category/tag (`Food`, `Ore`, `Logs`, `Medical`, etc.).
- Pull/push hauling jobs auto-generated by deficits and overflow.
- Haul path batching to reduce expensive path calls.
- Restricted zones enforce role-based access.

## 4.4 Squads / Patrols / Alerts

- Configurable patrol routes (waypoint loop or area roam).
- Alert levels:
  - Green: normal schedule
  - Yellow: suspicious sightings, reinforce gates
  - Red: active raid/siege, all guard squads to combat posture
- Civilian shelter behavior linked to alert level.

## 4.5 Raids / Siege-lite

- Threat generator based on prosperity, notoriety, nearby spawn pressure.
- Raid phases: warning -> contact -> assault -> retreat/resolution.
- Siege-lite interactables: gates, barricades, fire spread on tagged objects.
- Post-raid ledger: losses, loot, injuries, morale/security impacts.

## 4.6 UI: Gumps + Menus + Overlays

- **Town Ledger Gump:** resources, treasury, meters, policies, top alerts.
- **Task Board Gump:** queue list, drag/reorder priority, dependencies, failed task retry.
- **NPC Sheet Gump:** skills, traits, needs, inventory, schedule, goals, autonomy mode.
- **Map Overlays:** zone boundaries, patrol routes, danger pings, build ghosts.
- Permission-aware actions per role rank.

## 4.7 Performance Throttling

- Hi-fi sim for loaded/nearby towns (e.g., tick 1s).
- Lo-fi sim for distant towns (e.g., 10-30s aggregate tick).
- Event-priority wakeups (raid, fire, player enters region).
- Budget caps per tick: max task allocations, max path requests, max NPC AI updates.

## 4.8 Audit Logging + Anti-Abuse Possession Rules

- Append-only audit stream for:
  - governance edits (policy/tax/permissions),
  - treasury changes,
  - possession start/end,
  - punitive justice actions.
- Possession constraints:
  - only authorized roles,
  - cooldown per player + per NPC,
  - range/region restrictions,
  - disabled for jailed/protected NPC types,
  - immediate revoke on abuse flags/combat logging exploits.

---

## 5) Game Rules and Permissions

- Founding requirements: gold + charter + min reputation.
- Claim overlap checks + shard-configurable vulnerability windows.
- Governance roles (Governor/Treasurer/Captain/Quartermaster/Magistrate) with explicit ACL matrix.
- Offline protection and decay rules configurable per shard.
- Optional PvP diplomacy hooks (alliance/embargo/war posture).

---

## 6) Simulation Details

## 6.1 NPC Decision Stack

1. Emergency reaction (raid/fire/injury nearby).
2. Hard needs (hunger/rest/safety thresholds).
3. Reserved task execution.
4. Autonomous contribution (best-fit queued task by skill + distance).
5. Idle/social/training behavior.

## 6.2 Town Meter Drivers

- **Prosperity:** trade surplus, completed production, housing quality.
- **Security:** patrol coverage, wall integrity, unresolved crimes/raids.
- **Health:** food diversity, clinic coverage, disease/injury load.
- **Morale:** wages, festivals, victories, casualty trauma.

Meters are recalculated incrementally each tick and fully normalized during maintenance intervals.

---

## 7) Persistence & Migration

- Versioned serializers for all root aggregates (`Town`, `NpcProfile`, `Task`, `BlueprintSite`, `AuditLog`).
- Snapshot + journal approach:
  - periodic full snapshot for quick load,
  - append journal entries between snapshots.
- Migration registry maps old schema versions to new transforms.
- Corruption guardrails: validation on load, quarantine invalid records, admin repair command.

---

## 8) API / Service Contracts (ServUO-style)

## 8.1 Command Surface (proposed)

- `[Town]` opens town ledger for the player's governed town.
- `[TownTask]` opens task board with role-based permissions.
- `[TownNpc <serial>]` opens NPC sheet and control options.
- `[TownZone]` toggles zone-drawing mode (stockpile/patrol/construction).
- `[TownAudit]` displays filterable governance + possession audit entries.
- `[TownAdmin ...]` privileged maintenance commands (repair, migrate, reindex).

All command entry points should map to existing Ultima-Adventures command auth levels (`Player/Counselor/GM/Admin`) and log privileged usage.

---

```text
ITownService
- CreateTown(...)
- ClaimTown(...)
- GetTownLedger(...)
- SetPolicy(...)
- AssignRole(...)

ITaskService
- EnqueueTask(...)
- ReprioritizeTask(...)
- CancelTask(...)
- ReserveRequirements(...)
- ResolveDependencies(...)

IConstructionService
- PlaceBlueprintGhost(...)
- AdvanceStage(...)
- ValidateMultiOrPlaceholder(...)

IPossessionService
- TryBeginPossession(player, npc)
- EndPossession(...)
- ValidateAntiAbuse(...)
```

---

## 9) Vertical Slice First (Milestones)

## Milestone 0 - Foundation (1-2 weeks)
- Create module boundaries and config files.
- Implement `Town`, `TownNpcProfile`, `TownTask` persistence.
- Add audit log backbone and admin inspection command.

## Milestone 1 - Vertical Slice: “Secure Hamlet” (2-4 weeks)
- Found/claim one town region.
- Town Ledger + Task Board + basic NPC Sheet gumps.
- Task queue with priority + reservation + dependency (minimum feature set).
- One staged blueprint (e.g., `Palisade Gate` using approved multi/placeholder).
- Stockpile zone + hauling loop.
- Single patrol squad + basic alert escalation + one raid event type.
- Possession flow with cooldown + audit entry.
- Hi-fi/lo-fi simulation toggle and budget caps.

**Definition of Done:** a guild can found a hamlet, build one defensive structure through staged construction, survive a raid using patrol + directed tasks, and review all actions in ledger/audit.

## Milestone 2 - Town Operations Expansion (3-6 weeks)
- Add production chains and treasury wages/upkeep.
- Add more blueprint categories and repair/decay.
- Expand NPC goals/schedules and morale impacts.

## Milestone 3 - Governance & Conflict (4-8 weeks)
- Justice workflow, diplomacy hooks, vulnerability windows.
- Multiple squad types and siege-lite interactables.

## Milestone 4 - Scale & Polish
- Performance profiling and throttling refinements.
- Migration hardening, admin tools, docs/examples for shard operators.

---

## 9.1 Vertical Slice Backlog (developer-ready)

1. Create `TownAggregate` save model + migration version `v1`.
2. Add `TownService.CreateTown` with boundary + overlap validation.
3. Add `TownTaskService.Enqueue/Reserve/Complete` with reservation TTL cleanup job.
4. Ship one stockpile zone type and one haul task generator.
5. Ship one construction blueprint (`Palisade Gate`) with 3 stages and approved Multi/placeholder request gate.
6. Add one `Guard` squad with two patrol routes + yellow/red alert responses.
7. Add one raid scenario and post-raid ledger summary entry.
8. Add possession cooldown and audit enforcement for direct NPC control.
9. Add lo-fi tick mode for unloaded towns and wake-on-player-enter behavior.

---

## 10) Testing & Observability

- Unit tests:
  - dependency graph correctness,
  - reservation contention,
  - policy permission matrix,
  - possession anti-abuse guards.
- Integration tests:
  - construction stage progression under concurrent hauling,
  - raid alert transitions and squad orders.
- Runtime metrics:
  - tick duration by town,
  - task queue depth,
  - reservation leak count,
  - audit events per minute.
- Debug commands for forced raid, forced meter recalculation, and sim mode inspection.

---

## 11) FOSS Delivery Notes

- License compatibility with upstream Ultima-Adventures/ServUO conventions.
- Keep systems data-driven (JSON/XML configs where shard admins expect them).
- Provide extension points for custom NPC archetypes, policy packs, and blueprint sets.
- Publish migration guides for shard upgrades.

---

_End of design spec._
