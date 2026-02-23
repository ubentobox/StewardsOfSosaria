using StewardsOfSosaria.Services;

namespace StewardsOfSosaria.Runtime
{
    /// <summary>
    /// Single runtime access point for Steward services.
    /// Keeps instances centralized and avoids duplicate field definitions.
    /// </summary>
    public static class StewardsRuntime
    {
        // NOTE:
        // Some script compilers will merge/retain partial stale sources and can end up duplicating
        // fields on the same type. Putting instances in a nested type helps avoid that.
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

        // Legacy-safe property syntax (avoid expression-bodied members for older script compilers)
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

        // Backward-compatible method accessors used by existing scripts in this repo.
        public static TownService GetTownService() { return State.Town; }
        public static TaskService GetTaskService() { return State.Task; }
        public static PossessionPolicy GetPossessionPolicy() { return State.Possession; }
        public static AuditService GetAuditService() { return State.Audit; }
    }
}
