using System;
using System.Collections.Generic;

namespace StewardsOfSosaria.Core
{
    public interface ITownService
    {
        TownAggregate CreateTown(Guid ownerId, TownOwnerType ownerType, string name, int centerX, int centerY, int centerZ);
        bool CanClaim(int centerX, int centerY, int width, int height);
        TownAggregate GetTown(Guid townId);
    }

    public interface ITaskService
    {
        TownTask Enqueue(TownTask task);
        bool Reprioritize(Guid townId, Guid taskId, int priority);
        bool ResolveDependencies(TownTask task, IDictionary<Guid, TownTaskStatus> statusByTaskId);
        IList<ReservationToken> ExpireReservations(DateTime nowUtc);
    }

    public interface IPossessionPolicy
    {
        bool CanPossess(TownNpcProfile npc, string playerRole, bool inBlockedRegion, DateTime nowUtc, out string reason);
    }
}
