using System;
using System.Collections;
using Server;
using Server.Commands;
using StewardsOfSosaria.Core;
using StewardsOfSosaria.Runtime;

namespace StewardsOfSosaria.Commands
{
    public static class StewardsCommands
    {
        public static void Initialize()
        {
            CommandSystem.Register("TownAudit", AccessLevel.Player, new CommandEventHandler(OnTownAudit));
        }

        private static void OnTownAudit(CommandEventArgs e)
        {
            int max = 10;
            if (e.Length > 0)
            {
                int parsed = e.GetInt32(0);
                if (parsed > 0)
                {
                    max = parsed;
                }
            }

            if (StewardsRuntime.TownService.AuditSink == null)
            {
                e.Mobile.SendMessage("Stewards audit sink is not available.");
                return;
            }

            IList entries = StewardsRuntime.TownService.AuditSink.GetRecent(max);
            e.Mobile.SendMessage("Stewards Audit: showing {0} most recent events.", entries.Count);

            int i;
            for (i = 0; i < entries.Count; i++)
            {
                AuditEntry entry = (AuditEntry)entries[i];
                e.Mobile.SendMessage(
                    "[{0}] {1} Town={2} Actor={3} :: {4}",
                    entry.UtcTime,
                    entry.EventType,
                    entry.TownId,
                    entry.Actor,
                    entry.Details);
            }
        }
    }
}
