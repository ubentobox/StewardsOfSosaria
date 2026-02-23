using StewardsOfSosaria.Services;

namespace StewardsOfSosaria.Runtime
{
    public static class StewardsRuntime
    {
        private static readonly TownService __townSvc = new TownService();
        private static readonly TaskService __taskSvc = new TaskService();
        private static readonly PossessionPolicy __possessionSvc = new PossessionPolicy();
        private static readonly AuditService __auditSvc = new AuditService();

        static StewardsRuntime()
        {
            __townSvc.AuditSink = __auditSvc;
            __taskSvc.AuditSink = __auditSvc;
        }

        public static TownService TownService
        {
            get { return __townSvc; }
        }

        public static TaskService TaskService
        {
            get { return __taskSvc; }
        }

        public static PossessionPolicy PossessionPolicy
        {
            get { return __possessionSvc; }
        }

        public static AuditService AuditService
        {
            get { return __auditSvc; }
        }
    }
}
