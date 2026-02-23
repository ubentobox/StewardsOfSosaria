using System;
using System.Collections.Generic;
using StewardsOfSosaria.Core;

namespace StewardsOfSosaria.Services
{
    public sealed class AuditService
    {
        private readonly List<AuditEntry> _entries;

        public AuditService()
        {
            _entries = new List<AuditEntry>();
        }

        public void Append(AuditEventType eventType, string actor, Guid townId, string details)
        {
            AuditEntry entry = new AuditEntry();
            entry.UtcTime = DateTime.UtcNow;
            entry.EventType = eventType;
            entry.Actor = actor == null ? string.Empty : actor;
            entry.TownId = townId;
            entry.Details = details == null ? string.Empty : details;

            _entries.Add(entry);
        }

        public IList<AuditEntry> GetRecent(int maxCount)
        {
            if (maxCount <= 0)
            {
                return new List<AuditEntry>();
            }

            int start = _entries.Count - maxCount;
            if (start < 0)
            {
                start = 0;
            }

            List<AuditEntry> result = new List<AuditEntry>();
            int i;
            for (i = _entries.Count - 1; i >= start; i--)
            {
                result.Add(_entries[i]);
            }

            return result;
        }

        public IList<AuditEntry> GetByTown(Guid townId, int maxCount)
        {
            List<AuditEntry> result = new List<AuditEntry>();
            int i;
            for (i = _entries.Count - 1; i >= 0; i--)
            {
                if (_entries[i].TownId == townId)
                {
                    result.Add(_entries[i]);
                    if (result.Count >= maxCount)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}
