using StewardsOfSosaria.Services;

namespace StewardsOfSosaria.Runtime
{
    public static class StewardsRuntime
    {
        private static readonly TownService __townSvc = new TownService();
        private static readonly TaskService __taskSvc = new TaskService();
        private static readonly PossessionPolicy __possessionSvc = new PossessionPolicy();
        private static readonly AuditService __auditSvc = new AuditService();
        private static bool __wired;

        private static void EnsureWired()
        {
            if (__wired)
            {
                return;
            }

            __townSvc.AuditSink = __auditSvc;
            __taskSvc.AuditSink = __auditSvc;
            __wired = true;
        }

        public static TownService GetTownService()
        {
            EnsureWired();
            return __townSvc;
        }

        public static TaskService GetTaskService()
        {
            EnsureWired();
            return __taskSvc;
        }

        public static PossessionPolicy GetPossessionPolicy()
        {
            EnsureWired();
            return __possessionSvc;
        }

        public static AuditService GetAuditService()
        {
            EnsureWired();
            return __auditSvc;
        }
    }
}
