using StewardsOfSosaria.Services;

namespace StewardsOfSosaria.Runtime
{
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
            get { return _taskService; }
        }

        public static PossessionPolicy PossessionPolicy
        {
            get { return _possessionPolicy; }
        }
    }
}
