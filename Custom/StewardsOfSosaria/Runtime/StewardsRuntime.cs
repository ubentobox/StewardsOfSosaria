using StewardsOfSosaria.Services;

namespace StewardsOfSosaria.Runtime
{
    public static class StewardsRuntime
    {
        // Keeping instances in a nested type helps avoid duplicate field definitions
        // if the script compiler merges partial/stale sources.
        private static class State
        {
            internal static readonly TownService Town = new TownService();
            internal static readonly TaskService Task = new TaskService();
            internal static readonly PossessionPolicy Possession = new PossessionPolicy();
            internal static readonly AuditService Audit = new AuditService();

            static State()
            {
                Town.AuditSink = Audit;
                Task.AuditSink = Audit;
            }
        }

        public static TownService TownService
        {
            get { return State.Town; }
        }

        public static TaskService TaskService
        {
            get { return State.Task; }
        }

        public static PossessionPolicy PossessionPolicy
        {
            get { return State.Possession; }
        }

        public static AuditService AuditService
        {
            get { return State.Audit; }
        }
    }
}
