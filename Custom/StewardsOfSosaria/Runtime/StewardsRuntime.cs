using StewardsOfSosaria.Services;

namespace StewardsOfSosaria.Runtime
{
    internal static class StewardsRuntimeState
    {
        internal static readonly TownService Town = new TownService();
        internal static readonly TaskService Task = new TaskService();
        internal static readonly PossessionPolicy Possession = new PossessionPolicy();
        internal static readonly AuditService Audit = new AuditService();

        static StewardsRuntimeState()
        {
            Town.AuditSink = Audit;
            Task.AuditSink = Audit;
        }
    }

    public static class StewardsRuntime
    {
        public static TownService TownService
        {
            get { return StewardsRuntimeState.Town; }
        }

        public static TaskService TaskService
        {
            get { return StewardsRuntimeState.Task; }
        }

        public static PossessionPolicy PossessionPolicy
        {
            get { return StewardsRuntimeState.Possession; }
        }

        public static AuditService AuditService
        {
            get { return StewardsRuntimeState.Audit; }
        }
    }
}
