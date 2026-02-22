using System;
using System.Collections;
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
