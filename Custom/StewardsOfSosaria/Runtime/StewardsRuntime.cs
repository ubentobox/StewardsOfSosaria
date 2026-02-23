using StewardsOfSosaria.Services;

namespace StewardsOfSosaria.Runtime
{
    public static class StewardsRuntime
    {
        private static readonly TownService __townSvc = new TownService();
        private static readonly TaskService __taskSvc = new TaskService();
        private static readonly PossessionPolicy __possessionSvc = new PossessionPolicy();
        private static readonly AuditService __auditSvc = new AuditService();
        private static bool __auditWiringApplied = false;

        private static void ApplyAuditWiringIfNeeded()
        {
            if (__auditWiringApplied)
            {
                return;
            }

            __townSvc.AuditSink = __auditSvc;
            __taskSvc.AuditSink = __auditSvc;
            __auditWiringApplied = true;
        }

        public static TownService GetTownService()
        {
            ApplyAuditWiringIfNeeded();
            return __townSvc;
        }

        public static TaskService GetTaskService()
        {
            ApplyAuditWiringIfNeeded();
            return __taskSvc;
        }

        public static PossessionPolicy GetPossessionPolicy()
        {
            ApplyAuditWiringIfNeeded();
            return __possessionSvc;
        }

        public static AuditService GetAuditService()
        {
            ApplyAuditWiringIfNeeded();
            return __auditSvc;
        }
    }
}
