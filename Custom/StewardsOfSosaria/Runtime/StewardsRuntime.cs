using StewardsOfSosaria.Services;

namespace StewardsOfSosaria.Runtime
{
    // NOTE: Runtime instances live in StewardsRuntimeState to avoid duplicate field definitions
    // on StewardsRuntime if script compilers merge stale/partial sources.
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
    public static class StewardsRuntime
    {
        static StewardsRuntime()
        {
            _townService.AuditSink = _auditService;
            _taskService.AuditSink = _auditService;
        }

        private static readonly TownService _townService = new TownService();
        private static readonly TaskService _taskService = new TaskService();
        private static readonly PossessionPolicy _possessionPolicy = new PossessionPolicy();
        private static readonly AuditService _auditService = new AuditService();
        private static readonly TownService _townService = new TownService();
        private static readonly TaskService _taskService = new TaskService();
        private static readonly PossessionPolicy _possessionPolicy = new PossessionPolicy();

        public static TownService TownService
        {
            get { return _townService; }
        }

        public static TaskService TaskService
        {
            get { return StewardsRuntimeState.Task; }
            get { return _taskService; }
        }

        public static PossessionPolicy PossessionPolicy
        {
            get { return StewardsRuntimeState.Possession; }
        }

        public static AuditService AuditService
        {
            get { return StewardsRuntimeState.Audit; }
            get { return _possessionPolicy; }
        }
    }
}
