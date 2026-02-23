using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using StewardsOfSosaria.Core;
using StewardsOfSosaria.Runtime;

namespace StewardsOfSosaria.Commands
{
    public static class StewardsCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("TownAudit", AccessLevel.Player, new CommandEventHandler(OnTownAudit));
            CommandSystem.Register("TownTaskAdd", AccessLevel.Player, new CommandEventHandler(OnTownTaskAdd));
            CommandSystem.Register("TownTaskList", AccessLevel.Player, new CommandEventHandler(OnTownTaskList));
            CommandSystem.Register("TownTaskReprio", AccessLevel.Player, new CommandEventHandler(OnTownTaskReprio));
            CommandSystem.Register("TownInfo", AccessLevel.Player, new CommandEventHandler(OnTownInfo));
            CommandSystem.Register("TownTaskDepend", AccessLevel.Player, new CommandEventHandler(OnTownTaskDepend));
            CommandSystem.Register("TownTaskResolve", AccessLevel.Player, new CommandEventHandler(OnTownTaskResolve));
            CommandSystem.Register("TownTaskReserveTest", AccessLevel.Player, new CommandEventHandler(OnTownTaskReserveTest));
            CommandSystem.Register("TownTaskExpire", AccessLevel.Player, new CommandEventHandler(OnTownTaskExpire));
            CommandSystem.Register("TownTaskSetStatus", AccessLevel.Player, new CommandEventHandler(OnTownTaskSetStatus));
            CommandSystem.Register("TownTaskNext", AccessLevel.Player, new CommandEventHandler(OnTownTaskNext));
            CommandSystem.Register("Town", AccessLevel.Player, new CommandEventHandler(OnTown));
            CommandSystem.Register("TownTask", AccessLevel.Player, new CommandEventHandler(OnTownTask));
            CommandSystem.Register("TownNpc", AccessLevel.Player, new CommandEventHandler(OnTownNpc));
            CommandSystem.Register("TownPossessCheck", AccessLevel.Player, new CommandEventHandler(OnTownPossessCheck));
        }

        private static void OnTown(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            IList tasks = StewardsRuntime.TaskService.GetTasksForTown(town.TownId);

            e.Mobile.SendMessage("Town shell (v1 read model stub)");
            e.Mobile.SendMessage("Name={0}  Id={1}", town.Name, town.TownId);
            e.Mobile.SendMessage("Owner={0}  Center={1},{2},{3}", town.OwnerType, town.CenterX, town.CenterY, town.CenterZ);
            e.Mobile.SendMessage("Boundary={0}x{1}  Threat={2}  Sim={3}", town.Width, town.Height, town.ThreatState, town.SimProfile);
            e.Mobile.SendMessage("TaskCount={0}", tasks.Count);
        }

        private static void OnTownTask(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            IList tasks = StewardsRuntime.TaskService.GetTasksForTown(town.TownId);
            e.Mobile.SendMessage("TownTask shell (v1 read model stub) - {0} task(s).", tasks.Count);

            int limit = tasks.Count;
            if (limit > 5)
            {
                limit = 5;
            }

            int i;
            for (i = 0; i < limit; i++)
            {
                TownTask task = (TownTask)tasks[i];
                e.Mobile.SendMessage("{0} {1} prio={2} status={3}", task.TaskId, task.Type, task.Priority, task.Status);
            }
        }

        private static void OnTownNpc(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            e.Mobile.SendMessage("TownNpc shell (v1 read model stub)");
            e.Mobile.SendMessage("Town={0}  Threat={1}  Sim={2}", town.Name, town.ThreatState, town.SimProfile);
            e.Mobile.SendMessage("Npc profile list not wired yet in v1 scaffold.");
        }

        private static void OnTownPossessCheck(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            if (e.Length < 4)
            {
                e.Mobile.SendMessage("Usage: [TownPossessCheck <role> <blockedRegion:0|1> <recentCombatSeconds> <cooldownSeconds>]");
                return;
            }

            string role = e.GetString(0);
            bool inBlockedRegion = e.GetInt32(1) != 0;
            int recentCombatSeconds = e.GetInt32(2);
            int cooldownSeconds = e.GetInt32(3);

            TownNpcProfile npc = new TownNpcProfile();
            npc.TownId = town.TownId;
            npc.NpcSerial = 0;
            npc.LastCombatUtc = DateTime.UtcNow.AddSeconds(-recentCombatSeconds);
            npc.PossessionCooldownUntilUtc = DateTime.UtcNow.AddSeconds(cooldownSeconds);

            string reason;
            bool allowed = StewardsRuntime.PossessionPolicy.CanPossess(npc, role, inBlockedRegion, DateTime.UtcNow, out reason);

            if (StewardsRuntime.TownService.AuditSink != null)
            {
                StewardsRuntime.TownService.AuditSink.Append(
                    AuditEventType.PossessionAttempt,
                    e.Mobile.Name,
                    town.TownId,
                    "role=" + role + " blocked=" + inBlockedRegion + " recentCombatSeconds=" + recentCombatSeconds + " cooldownSeconds=" + cooldownSeconds + " allowed=" + allowed + " reason=" + reason);
            }

            e.Mobile.SendMessage("Possession check: allowed={0} reason={1}", allowed, reason);
        }


        private static void OnTownTaskAdd(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            int priority = 50;
            if (e.Length > 0)
            {
                int parsed = e.GetInt32(0);
                if (parsed >= 0)
                {
                    priority = parsed;
                }
            }

            TownTask task = new TownTask();
            task.TaskId = Guid.NewGuid();
            task.TownId = town.TownId;
            task.Type = TownTaskType.Haul;
            task.Priority = priority;

            StewardsRuntime.TaskService.Enqueue(task);
            e.Mobile.SendMessage("Added task {0} to town {1} with priority {2}.", task.TaskId, town.Name, task.Priority);
        }

        private static void OnTownTaskList(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            IList tasks = StewardsRuntime.TaskService.GetTasksForTown(town.TownId);
            e.Mobile.SendMessage("Town tasks for {0}: {1} entries.", town.Name, tasks.Count);

            int i;
            for (i = 0; i < tasks.Count; i++)
            {
                TownTask task = (TownTask)tasks[i];
                e.Mobile.SendMessage("{0}: type={1} priority={2} status={3}", task.TaskId, task.Type, task.Priority, task.Status);
            }
        }

        private static void OnTownTaskReprio(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            if (e.Length < 2)
            {
                e.Mobile.SendMessage("Usage: [TownTaskReprio <taskGuid> <priority>]");
                return;
            }

            Guid taskId;
            try
            {
                taskId = new Guid(e.GetString(0));
            }
            catch
            {
                e.Mobile.SendMessage("Invalid task guid format.");
                return;
            }

            int priority = e.GetInt32(1);
            bool ok = StewardsRuntime.TaskService.Reprioritize(town.TownId, taskId, priority);
            if (!ok)
            {
                e.Mobile.SendMessage("Task not found in latest settlement queue.");
                return;
            }

            e.Mobile.SendMessage("Task {0} reprioritized to {1}.", taskId, priority);
        }

        private static void OnTownInfo(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            e.Mobile.SendMessage("Town: {0}", town.Name);
            e.Mobile.SendMessage("TownId: {0}", town.TownId);
            e.Mobile.SendMessage("OwnerType: {0}", town.OwnerType);
            e.Mobile.SendMessage("Center: {0},{1},{2}", town.CenterX, town.CenterY, town.CenterZ);
            e.Mobile.SendMessage("Boundary: {0}x{1}", town.Width, town.Height);
            e.Mobile.SendMessage("Threat/Sim: {0}/{1}", town.ThreatState, town.SimProfile);
        }

        private static void OnTownTaskDepend(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            if (e.Length < 2)
            {
                e.Mobile.SendMessage("Usage: [TownTaskDepend <dependencyGuid> <dependentGuid>]");
                return;
            }

            Guid dependencyId;
            Guid dependentId;

            try
            {
                dependencyId = new Guid(e.GetString(0));
                dependentId = new Guid(e.GetString(1));
            }
            catch
            {
                e.Mobile.SendMessage("Invalid guid format.");
                return;
            }

            TownTask dependencyTask = FindTaskById(town.TownId, dependencyId);
            TownTask dependentTask = FindTaskById(town.TownId, dependentId);

            if (dependencyTask == null || dependentTask == null)
            {
                e.Mobile.SendMessage("Could not locate one or both tasks in latest settlement.");
                return;
            }

            if (!dependentTask.DependencyTaskIds.Contains(dependencyId))
            {
                dependentTask.DependencyTaskIds.Add(dependencyId);
            }

            e.Mobile.SendMessage("Task dependency added: {0} -> {1}", dependencyId, dependentId);
        }

        private static void OnTownTaskResolve(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            if (e.Length < 1)
            {
                e.Mobile.SendMessage("Usage: [TownTaskResolve <taskGuid>]");
                return;
            }

            Guid taskId;
            try
            {
                taskId = new Guid(e.GetString(0));
            }
            catch
            {
                e.Mobile.SendMessage("Invalid task guid format.");
                return;
            }

            TownTask task = FindTaskById(town.TownId, taskId);
            if (task == null)
            {
                e.Mobile.SendMessage("Task not found in latest settlement queue.");
                return;
            }

            Dictionary<Guid, TownTaskStatus> status = BuildStatusMap(town.TownId);
            bool resolved = StewardsRuntime.TaskService.ResolveDependencies(task, status);
            e.Mobile.SendMessage("Task {0} dependency status resolved={1}", taskId, resolved);
        }

        private static void OnTownTaskReserveTest(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            int seconds = 5;
            if (e.Length > 0)
            {
                int parsed = e.GetInt32(0);
                if (parsed > 0)
                {
                    seconds = parsed;
                }
            }

            IList tasks = StewardsRuntime.TaskService.GetTasksForTown(town.TownId);
            if (tasks.Count == 0)
            {
                e.Mobile.SendMessage("No tasks available for reservation test.");
                return;
            }

            TownTask task = (TownTask)tasks[0];
            ReservationToken token = new ReservationToken();
            token.TokenId = Guid.NewGuid();
            token.TaskId = task.TaskId;
            token.ItemSerial = 0;
            token.Amount = 1;
            token.ExpiresAtUtc = DateTime.UtcNow.AddSeconds(seconds);
            task.Reservations.Add(token);

            e.Mobile.SendMessage("Added reservation token {0} on task {1}, expires in {2}s.", token.TokenId, task.TaskId, seconds);
        }

        private static void OnTownTaskSetStatus(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            if (e.Length < 2)
            {
                e.Mobile.SendMessage("Usage: [TownTaskSetStatus <taskGuid> <Queued|Reserved|InProgress|Blocked|Done|Failed|Canceled>]");
                return;
            }

            Guid taskId;
            try
            {
                taskId = new Guid(e.GetString(0));
            }
            catch
            {
                e.Mobile.SendMessage("Invalid task guid format.");
                return;
            }

            TownTask task = FindTaskById(town.TownId, taskId);
            if (task == null)
            {
                e.Mobile.SendMessage("Task not found in latest settlement queue.");
                return;
            }

            TownTaskStatus status;
            if (!TryParseTaskStatus(e.GetString(1), out status))
            {
                e.Mobile.SendMessage("Invalid status value.");
                return;
            }

            task.Status = status;
            e.Mobile.SendMessage("Task {0} status set to {1}.", task.TaskId, task.Status);
        }

        private static bool TryParseTaskStatus(string value, out TownTaskStatus status)
        {
            if (string.Equals(value, "Queued", StringComparison.OrdinalIgnoreCase)) { status = TownTaskStatus.Queued; return true; }
            if (string.Equals(value, "Reserved", StringComparison.OrdinalIgnoreCase)) { status = TownTaskStatus.Reserved; return true; }
            if (string.Equals(value, "InProgress", StringComparison.OrdinalIgnoreCase)) { status = TownTaskStatus.InProgress; return true; }
            if (string.Equals(value, "Blocked", StringComparison.OrdinalIgnoreCase)) { status = TownTaskStatus.Blocked; return true; }
            if (string.Equals(value, "Done", StringComparison.OrdinalIgnoreCase)) { status = TownTaskStatus.Done; return true; }
            if (string.Equals(value, "Failed", StringComparison.OrdinalIgnoreCase)) { status = TownTaskStatus.Failed; return true; }
            if (string.Equals(value, "Canceled", StringComparison.OrdinalIgnoreCase)) { status = TownTaskStatus.Canceled; return true; }

            status = TownTaskStatus.Queued;
            return false;
        }

        private static void OnTownTaskNext(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
                return;
            }

            IList tasks = StewardsRuntime.TaskService.GetTasksForTown(town.TownId);
            if (tasks.Count == 0)
            {
                e.Mobile.SendMessage("No tasks are queued for the latest settlement.");
                return;
            }

            Dictionary<Guid, TownTaskStatus> status = BuildStatusMap(town.TownId);

            int i;
            for (i = 0; i < tasks.Count; i++)
            {
                TownTask task = (TownTask)tasks[i];

                if (task.Status != TownTaskStatus.Queued)
                {
                    continue;
                }

                if (!StewardsRuntime.TaskService.ResolveDependencies(task, status))
                {
                    continue;
                }

                e.Mobile.SendMessage("Next executable task: {0} type={1} priority={2} status={3}", task.TaskId, task.Type, task.Priority, task.Status);
                return;
            }

            e.Mobile.SendMessage("No executable queued task found (all queued tasks are dependency-blocked or non-queued).");
        }

        private static void OnTownTaskExpire(CommandEventArgs e)
        {
            IList<ReservationToken> expired = StewardsRuntime.TaskService.ExpireReservations(DateTime.UtcNow);
            e.Mobile.SendMessage("Expired reservation sweep removed {0} token(s).", expired.Count);
        }

        private static Dictionary<Guid, TownTaskStatus> BuildStatusMap(Guid townId)
        {
            IList tasks = StewardsRuntime.TaskService.GetTasksForTown(townId);
            Dictionary<Guid, TownTaskStatus> status = new Dictionary<Guid, TownTaskStatus>();

            int i;
            for (i = 0; i < tasks.Count; i++)
            {
                TownTask task = (TownTask)tasks[i];
                status[task.TaskId] = task.Status;
            }

            return status;
        }

        private static TownTask FindTaskById(Guid townId, Guid taskId)
        {
            IList tasks = StewardsRuntime.TaskService.GetTasksForTown(townId);
            int i;
            for (i = 0; i < tasks.Count; i++)
            {
                TownTask task = (TownTask)tasks[i];
                if (task.TaskId == taskId)
                {
                    return task;
                }
            }

            return null;
        }

        private static void OnTownAudit(CommandEventArgs e)
        {
            int max = 10;
            if (e.Length > 0)
            {
                int parsed = e.GetInt32(0);
                if (parsed > 0)
                {
                    max = parsed;
                }
            }

            if (StewardsRuntime.TownService.AuditSink == null)
            {
                e.Mobile.SendMessage("Stewards audit sink is not available.");
                return;
            }

            IList entries = StewardsRuntime.TownService.AuditSink.GetRecent(max);
            e.Mobile.SendMessage("Stewards Audit: showing {0} most recent events.", entries.Count);

            int i;
            for (i = 0; i < entries.Count; i++)
            {
                AuditEntry entry = (AuditEntry)entries[i];
                e.Mobile.SendMessage(
                    "[{0}] {1} Town={2} Actor={3} :: {4}",
                    entry.UtcTime,
                    entry.EventType,
                    entry.TownId,
                    entry.Actor,
                    entry.Details);
            }
        }
    }
}
