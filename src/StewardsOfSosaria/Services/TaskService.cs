using System;
using System.Collections.Generic;
using StewardsOfSosaria.Core;

namespace StewardsOfSosaria.Services
{
    public sealed class TaskService : ITaskService
    {
        private readonly Dictionary<Guid, List<TownTask>> _tasksByTown;

        public TaskService()
        {
            _tasksByTown = new Dictionary<Guid, List<TownTask>>();
        }

        public TownTask Enqueue(TownTask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            List<TownTask> list = EnsureList(task.TownId);
            list.Add(task);
            list.Sort(CompareTasks);
            return task;
        }

        public bool Reprioritize(Guid townId, Guid taskId, int priority)
        {
            List<TownTask> list;
            if (!_tasksByTown.TryGetValue(townId, out list))
            {
                return false;
            }

            TownTask task = FindTask(list, taskId);
            if (task == null)
            {
                return false;
            }

            task.Priority = priority;
            list.Sort(CompareTasks);
            return true;
        }

        public bool ResolveDependencies(TownTask task, IDictionary<Guid, TownTaskStatus> statusByTaskId)
        {
            if (task == null)
            {
                return false;
            }

            if (task.DependencyTaskIds.Count == 0)
            {
                return true;
            }

            int i;
            for (i = 0; i < task.DependencyTaskIds.Count; i++)
            {
                Guid dependencyId = task.DependencyTaskIds[i];
                TownTaskStatus status;

                if (!statusByTaskId.TryGetValue(dependencyId, out status))
                {
                    return false;
                }

                if (status != TownTaskStatus.Done)
                {
                    return false;
                }
            }

            return true;
        }

        public IList<ReservationToken> ExpireReservations(DateTime nowUtc)
        {
            List<ReservationToken> expired = new List<ReservationToken>();

            foreach (List<TownTask> tasks in _tasksByTown.Values)
            {
                int i;
                for (i = 0; i < tasks.Count; i++)
                {
                    TownTask task = tasks[i];
                    int r = task.Reservations.Count - 1;

                    for (; r >= 0; r--)
                    {
                        ReservationToken token = task.Reservations[r];
                        if (token.ExpiresAtUtc <= nowUtc)
                        {
                            expired.Add(token);
                            task.Reservations.RemoveAt(r);
                        }
                    }
                }
            }

            return expired;
        }

        private static int CompareTasks(TownTask a, TownTask b)
        {
            int byPriority = b.Priority.CompareTo(a.Priority);
            if (byPriority != 0)
            {
                return byPriority;
            }

            return a.TaskId.CompareTo(b.TaskId);
        }

        private static TownTask FindTask(List<TownTask> tasks, Guid taskId)
        {
            int i;
            for (i = 0; i < tasks.Count; i++)
            {
                if (tasks[i].TaskId == taskId)
                {
                    return tasks[i];
                }
            }

            return null;
        }

        private List<TownTask> EnsureList(Guid townId)
        {
            List<TownTask> list;
            if (!_tasksByTown.TryGetValue(townId, out list))
            {
                list = new List<TownTask>();
                _tasksByTown[townId] = list;
            }

            return list;
        }
    }
}
