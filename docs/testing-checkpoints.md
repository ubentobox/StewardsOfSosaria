# Stewards of Sosaria - Testing Checkpoints (When to Test)

Use this as a practical sequence for shard-side testing as features become testable.

## Checkpoint 1: Script compile health (run every change)

**When:** after any script change under `Custom/StewardsOfSosaria/...`.

**Test:** shard script compile only.

**Pass if:** no compile errors/warnings from Stewards scripts.

---

## Checkpoint 2: Stewardship Deed behavior

**When:** once `StewardshipDeed` item script is present and compiles.

**Test flow:**
1. Add deed (`0x14EF`) to player backpack.
2. Double-click deed.
3. Target valid location.
4. Verify statics spawned:
   - center marker `0x144C` at target,
   - center marker `0x144B` at `x-1,y,z`.
5. Verify deed consumed.

**Pass if:** markers place correctly and user gets success/failure messages.

---

## Checkpoint 3: Boundary overlap rejection (60x61)

**When:** after town claim logic (`CanClaim`) and deed integration are both wired.

**Test flow:**
1. Found first settlement.
2. Attempt second settlement inside overlap range.
3. Attempt third settlement outside overlap range.

**Pass if:** overlapping claim is rejected; non-overlapping claim succeeds.

---

## Checkpoint 4: Task queue priority + dependencies

**When:** task enqueue/reprioritize/dependency logic is implemented.

**Test flow:**
1. Enqueue three tasks with mixed priorities.
2. Reprioritize one task to highest value.
3. Add dependency chain A -> B.
4. Mark A not done, verify B unresolved.
5. Mark A done, verify B resolves.

**Pass if:** order and dependency behavior match expected outcomes.

---

## Checkpoint 5: Reservation expiry sweeper

**When:** reservation tokens are added to tasks and sweeper is implemented.

**Test flow:**
1. Add a task reservation with near-future expiry.
2. Run expiry sweep after expiry time.
3. Verify token removed from task and reported in expired list.

**Pass if:** expired reservations are removed deterministically.

---

## Checkpoint 6: Possession gates

**When:** possession command/hook is connected to `PossessionPolicy`.

**Test matrix:**
- Allowed role in safe region, no cooldown, no recent combat -> allow.
- Disallowed role -> deny.
- In blocked region -> deny.
- Recent combat (<30s) -> deny.
- Cooldown active -> deny.

**Pass if:** every matrix row returns expected allow/deny + reason string.


---

## Checkpoint 7: Audit log events

**When:** after town founding and task operations are wired to `AuditService`.

**Test flow:**
1. Found a settlement with the deed.
2. Enqueue one task and reprioritize it.
3. Create one near-expiry reservation and run expiry sweep.
4. Query recent audit entries from service (or debug command once added).

**Pass if:** events appear in order with correct event type, town id, actor, and details.


---

## Checkpoint 8: TownAudit command output

**When:** after command scripts compile and audit events are being written.

**Test flow:**
1. Found a settlement and run at least one task operation that logs audit entries.
2. Run `[TownAudit]` and `[TownAudit 5]`.
3. Verify command prints the expected number of recent entries and includes event type, town id, actor, and details.

**Pass if:** command responds without errors and displays the newest events in reverse chronological order.


---

## Checkpoint 9: Task helper commands

**When:** after `TownTaskAdd` and `TownTaskList` command scripts compile.

**Test flow:**
1. Found a settlement with deed.
2. Run `[TownTaskAdd]` and `[TownTaskAdd 80]`.
3. Run `[TownTaskList]`.
4. Confirm both tasks appear, with expected priorities and `Haul` type.

**Pass if:** command outputs show task creation and list reflects queued entries for the latest founded town.
