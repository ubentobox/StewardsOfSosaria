# Stewards of Sosaria - Testing Checkpoints (When to Test)

Use this as a practical sequence for shard-side testing as features become testable.

## Checkpoint 1: Script compile health (run every change)

**When:** after any script change under `Custom/StewardsOfSosaria/...`.

**Test:** shard script compile only.

If a local `obj/` or `bin/` folder exists from a separate MSBuild/dotnet compile, delete those folders first to avoid duplicate assembly attribute errors being pulled into script compilation.

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

**When:** possession command/hook is connected to `PossessionPolicy` (e.g., `[TownPossessCheck]`).

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


---

## Checkpoint 10: TownTaskReprio + TownInfo commands

**When:** after command scripts compile and task list output is working.

**Test flow:**
1. Found a settlement and run `[TownTaskAdd]` twice.
2. Run `[TownTaskList]` and copy one task guid.
3. Run `[TownTaskReprio <copiedGuid> 99]`.
4. Run `[TownTaskList]` again and verify updated priority.
5. Run `[TownInfo]` and verify town center/boundary/ids match the founded settlement.

**Pass if:** reprioritize command updates the expected task, and town info prints consistent metadata.


---

## Checkpoint 11: Dependency + reservation helper commands

**When:** after `TownTaskDepend`, `TownTaskResolve`, `TownTaskReserveTest`, and `TownTaskExpire` compile.

**Test flow:**
1. Found a settlement and add two tasks via `[TownTaskAdd]`.
2. Run `[TownTaskList]` and copy both task GUIDs.
3. Run `[TownTaskDepend <taskA> <taskB>]`.
4. Run `[TownTaskResolve <taskB>]` and verify unresolved while dependency is not `Done`.
5. Run `[TownTaskSetStatus <taskA> Done]`, then rerun `[TownTaskResolve <taskB>]` and verify resolved=true.
6. Run `[TownTaskReserveTest 2]`, wait 3 seconds, then run `[TownTaskExpire]`.

**Pass if:** dependency command links tasks correctly, resolve flips false->true after setting dependency to Done, and expiry sweep removes the test reservation token.


---

## Checkpoint 12: Merge-recovery sanity checks

**When:** after any manual conflict resolution on GitHub (especially “Accept both changes”).

**Test flow:**
1. Run checks from `docs/merge-recovery-checklist.md`.
2. Confirm single runtime definition and no conflict markers.
3. Recompile scripts.

**Pass if:** checks return expected results and compile completes cleanly.

---

## Checkpoint 13: Town/TownTask/TownNpc shell commands

**When:** after `[Town]`, `[TownTask]`, and `[TownNpc]` command scripts compile.

**Test flow:**
1. Found a settlement.
2. Run `[Town]` and confirm summary lines (name/id/owner/center/boundary/task count).
3. Run `[TownTask]` and verify it shows the current task count and up to five task summary rows.
4. Run `[TownNpc]` and verify it prints town context and the v1 stub notice.

**Pass if:** all three shell commands execute without errors and show stable read-model output for the latest settlement.

---

## Checkpoint 14: Persistence serializer round-trip

**When:** after `StewardsPersistenceSerializer` compiles.

**Test flow:**
1. Create one `TownAggregate`, one `TownTask` (with at least one dependency and reservation), and one `TownNpcProfile` in a local harness or temporary script.
2. Serialize each model using `StewardsPersistenceSerializer.Write...` methods.
3. Deserialize each model using matching `Read...` methods.
4. Compare key fields (ids, enum values, coordinates, status, reservation values, cooldown/combat timestamps).

**Pass if:** serialized models deserialize successfully and key fields match expected values for each model type.

---

## Checkpoint 15: TownPossessCheck command + audit

**When:** after `[TownPossessCheck]` compiles.

**Test flow:**
1. Found a settlement.
2. Run `[TownPossessCheck Governor 0 999 0]` and expect allow=true.
3. Run `[TownPossessCheck Peasant 0 999 0]` and expect deny by role.
4. Run `[TownPossessCheck Governor 1 999 0]` and expect deny by blocked region.
5. Run `[TownPossessCheck Governor 0 1 0]` and expect deny by recent combat.
6. Run `[TownPossessCheck Governor 0 999 30]` and expect deny by cooldown.
7. Run `[TownAudit 10]` and confirm `PossessionAttempt` entries are present with details.

**Pass if:** policy gates return expected allow/deny reasons and each check appends an audit entry.

---

## Checkpoint 16: TownTaskNext executable preview

**When:** after `[TownTaskNext]` compiles.

**Test flow:**
1. Found a settlement and add two tasks (`[TownTaskAdd]` twice).
2. Set one task to depend on the other with `[TownTaskDepend <taskA> <taskB>]`.
3. Run `[TownTaskNext]` and verify it selects the executable queued task (dependency-satisfied one).
4. Set prerequisite to `Done` with `[TownTaskSetStatus <taskA> Done]`.
5. Run `[TownTaskNext]` again and verify the dependent task can now be selected.

**Pass if:** command consistently returns the highest-priority queued task whose dependencies are satisfied.

---

## Checkpoint 17: Task start/complete flow commands

**When:** after `[TownTaskStartNext]` and `[TownTaskDone]` compile.

**Test flow:**
1. Found a settlement and add two tasks with different priorities.
2. Run `[TownTaskStartNext]` and verify the highest-priority executable queued task moves to `InProgress`.
3. Run `[TownTaskList]` and copy the started task GUID.
4. Run `[TownTaskDone <startedTaskGuid>]` and verify it becomes `Done`.
5. Run `[TownAudit 10]` and confirm `TaskStarted` and `TaskCompleted` entries are present.

**Pass if:** start/complete transitions occur as expected and both transitions are recorded in audit history.

---

## Checkpoint 18: Task failure path command

**When:** after `[TownTaskFail]` compiles.

**Test flow:**
1. Found a settlement and add a task with `[TownTaskAdd]`.
2. Run `[TownTaskList]` and copy the task GUID.
3. Run `[TownTaskFail <taskGuid> Missing resources]`.
4. Run `[TownTaskList]` and verify status is `Failed`.
5. Run `[TownAudit 10]` and verify a `TaskFailed` entry includes the reason text.

**Pass if:** task is marked failed, reason is captured on the task, and an audit event is recorded.

