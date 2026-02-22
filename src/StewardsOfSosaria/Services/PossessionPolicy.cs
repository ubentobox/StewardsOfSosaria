using System;
using System.Collections.Generic;
using StewardsOfSosaria.Core;

namespace StewardsOfSosaria.Services;

public sealed class PossessionPolicy : IPossessionPolicy
{
    private static readonly HashSet<string> AllowedRoles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Governor",
        "Captain",
        "Quartermaster",
        "Magistrate"
    };

    public bool CanPossess(TownNpcProfile npc, string playerRole, bool inBlockedRegion, DateTime nowUtc, out string? reason)
    {
        if (!AllowedRoles.Contains(playerRole))
        {
            reason = "Role not authorized for possession.";
            return false;
        }

        if (inBlockedRegion)
        {
            reason = "Possession is disallowed in this region.";
            return false;
        }

        if (nowUtc < npc.LastCombatUtc.AddSeconds(30))
        {
            reason = "NPC or player was in combat too recently.";
            return false;
        }

        if (nowUtc < npc.PossessionCooldownUntilUtc)
        {
            reason = "NPC possession cooldown is still active.";
            return false;
        }

        reason = null;
        return true;
    }
}
