using System;
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
            CommandSystem.Register("TownAudit", AccessLevel.Player, OnTownAudit);
            CommandSystem.Register("TownTaskAdd", AccessLevel.Player, OnTownTaskAdd);
            CommandSystem.Register("TownTaskList", AccessLevel.Player, OnTownTaskList);
            CommandSystem.Register("TownTaskReprio", AccessLevel.Player, OnTownTaskReprio);
            CommandSystem.Register("TownInfo", AccessLevel.Player, OnTownInfo);
            CommandSystem.Register("TownTaskDepend", AccessLevel.Player, OnTownTaskDepend);
            CommandSystem.Register("TownTaskResolve", AccessLevel.Player, OnTownTaskResolve);
            CommandSystem.Register("TownTaskReserveTest", AccessLevel.Player, OnTownTaskReserveTest);
            CommandSystem.Register("TownTaskExpire", AccessLevel.Player, OnTownTaskExpire);
        }

        private static TownAggregate GetTown(CommandEventArgs e)
        {
            TownAggregate town = StewardsRuntime.TownService.GetLastCreatedTown();
            if (town == null)
            {
                e.Mobile.SendMessage("No settlement has been founded yet.");
            }
            return town;
        }

        private static void OnTownTaskAdd(CommandEventArgs e)
        {
            TownAggregate town = GetTown(e);
            if (town == null) return;

            int priority = e.Length > 0 ? e.GetInt32(0) : 50;

            TownTask task = new TownTask();
            task.TaskId = Guid.NewGuid();
            task.TownId = town.TownId;
            task.Type = TownTaskType.Haul;
            task.Priority = priority;

            StewardsRuntime.TaskService.Enqueue(task);

            e.Mobile.SendMessage(
                "Added task {0} to town {1} with priority {2}.",
                task.TaskId, town.Name, priority
            );
        }

        private static void OnTownTaskList(CommandEventArgs e)
        {
            TownAggregate town = GetTown(e);
            if (town == null) return;

            System.Collections.IList tasks =
                StewardsRuntime.TaskService.GetTasksForTown(town.TownId);

            e.Mobile.SendMessage("Tasks for {0}:", town.Name);

            for (int i = 0; i < tasks.Count; i++)
            {
                TownTask t = (TownTask)tasks[i];
                e.Mobile.SendMessage(
                    "{0} | {1} | Priority={2} | Status={3}",
                    t.TaskId, t.Type, t.Priority, t.Status
                );
            }
        }

        private static void OnTownTaskReprio(CommandEventArgs e)
        {
            TownAggregate town = GetTown(e);
            if (town == null) return;
            if (e.Length < 2) return;

            Guid taskId = new Guid(e.GetString(0));
            int priority = e.GetInt32(1);

            bool ok = StewardsRuntime.TaskService.Reprioritize(
                town.TownId, taskId, priority
            );

            e.Mobile.SendMessage(
                ok ? "Task reprioritized." : "Task not found."
            );
        }

        private static void OnTownInfo(CommandEventArgs e)
        {
            TownAggregate town = GetTown(e);
            if (town == null) return;

            e.Mobile.SendMessage("Town: {0}", town.Name);
            e.Mobile.SendMessage("OwnerType: {0}", town.OwnerType);
            e.Mobile.SendMessage("Center: {0},{1},{2}",
                town.CenterX, town.CenterY, town.CenterZ);
        }

        private static void OnTownTaskDepend(CommandEventArgs e)
        {
            TownAggregate town = GetTown(e);
            if (town == null || e.Length < 2) return;

            Guid dependencyId = new Guid(e.GetString(0));
            Guid dependentId = new Guid(e.GetString(1));

            TownTask dep = FindTask(town.TownId, dependencyId);
            TownTask task = FindTask(town.TownId, dependentId);

            if (dep == null || task == null) return;

            if (!task.DependencyTaskIds.Contains(dependencyId))
            {
                task.DependencyTaskIds.Add(dependencyId);
            }

            e.Mobile.SendMessage("Dependency added.");
        }

        private static void OnTownTaskResolve(CommandEventArgs e)
        {
            TownAggregate town = GetTown(e);
            if (town == null || e.Length < 1) return;

            Guid taskId = new Guid(e.GetString(0));
            TownTask task = FindTask(town.TownId, taskId);
            if (task == null) return;

            IDictionary<Guid, TownTaskStatus> status =
                BuildStatusMap(town.TownId);

            bool resolved =
                StewardsRuntime.TaskService.ResolveDependencies(task, status);

            e.Mobile.SendMessage("Resolved={0}", resolved);
        }

        private static void OnTownTaskReserveTest(CommandEventArgs e)
        {
            TownAggregate town = GetTown(e);
            if (town == null) return;

            System.Collections.IList tasks =
                StewardsRuntime.TaskService.GetTasksForTown(town.TownId);

            if (tasks.Count == 0) return;

            TownTask task = (TownTask)tasks[0];

            ReservationToken token = new ReservationToken();
            token.TokenId = Guid.NewGuid();
            token.TaskId = task.TaskId;
            token.ExpiresAtUtc = DateTime.UtcNow.AddSeconds(5);

            task.Reservations.Add(token);

            e.Mobile.SendMessage("Reservation added.");
        }

        private static void OnTownTaskExpire(CommandEventArgs e)
        {
            IList<ReservationToken> expired =
                StewardsRuntime.TaskService.ExpireReservations(DateTime.UtcNow);

            e.Mobile.SendMessage(
                "Expired {0} reservations.", expired.Count
            );
        }

        private static IDictionary<Guid, TownTaskStatus> BuildStatusMap(Guid townId)
        {
            System.Collections.IList tasks =
                StewardsRuntime.TaskService.GetTasksForTown(townId);

            Dictionary<Guid, TownTaskStatus> map =
                new Dictionary<Guid, TownTaskStatus>();

            for (int i = 0; i < tasks.Count; i++)
            {
                TownTask t = (TownTask)tasks[i];
                map[t.TaskId] = t.Status;
            }

            return map;
        }

        private static TownTask FindTask(Guid townId, Guid taskId)
        {
            System.Collections.IList tasks =
                StewardsRuntime.TaskService.GetTasksForTown(townId);

            for (int i = 0; i < tasks.Count; i++)
            {
                if (((TownTask)tasks[i]).TaskId == taskId)
                {
                    return (TownTask)tasks[i];
                }
            }

            return null;
        }

        private static void OnTownAudit(CommandEventArgs e)
        {
            int max = e.Length > 0 ? e.GetInt32(0) : 10;

            IList<AuditEntry> entries =
                StewardsRuntime.TownService.AuditSink.GetRecent(max);

            for (int i = 0; i < entries.Count; i++)
            {
                AuditEntry a = entries[i];
                e.Mobile.SendMessage(
                    "[{0}] {1} :: {2}",
                    a.UtcTime, a.EventType, a.Details
                );
            }
        }
    }
}
