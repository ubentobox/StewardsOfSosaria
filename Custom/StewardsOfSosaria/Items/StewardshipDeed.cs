using System;
using Server;
using Server.Items;
using Server.Targeting;
using StewardsOfSosaria.Core;
using StewardsOfSosaria.Runtime;

namespace StewardsOfSosaria.Items
{
    public sealed class StewardshipDeed : Item
    {
        public const int DeedItemId = 0x14EF;
        public const int CenterMarkerPrimary = 0x144C;
        public const int CenterMarkerSecondary = 0x144B;

        [Constructable]
        public StewardshipDeed() : base(DeedItemId)
        {
            Name = "a stewardship deed";
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public StewardshipDeed(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null)
            {
                return;
            }

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("The deed must be in your backpack to use it.");
                return;
            }

            from.SendMessage("Target where you wish to found a settlement.");
            from.Target = new StewardshipTarget(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        private sealed class StewardshipTarget : Target
        {
            private readonly StewardshipDeed _deed;

            public StewardshipTarget(StewardshipDeed deed) : base(12, true, TargetFlags.None)
            {
                _deed = deed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                IPoint3D p = targeted as IPoint3D;
                if (p == null)
                {
                    from.SendMessage("That is not a valid location.");
                    return;
                }

                Point3D point = new Point3D(p);
                Map map = from.Map;

                if (map == null || map == Map.Internal)
                {
                    from.SendMessage("You cannot found a settlement there.");
                    return;
                }

                bool canClaim = StewardsRuntime.TownService.CanClaim(point.X, point.Y, 60, 61);
                if (!canClaim)
                {
                    from.SendMessage("This location overlaps another settlement boundary.");
                    return;
                }

                Item markerA = new Static(CenterMarkerPrimary);
                markerA.MoveToWorld(point, map);

                Item markerB = new Static(CenterMarkerSecondary);
                markerB.MoveToWorld(new Point3D(point.X - 1, point.Y, point.Z), map);

                TownAggregate town = StewardsRuntime.TownService.CreateTown(
                    Guid.NewGuid(),
                    TownOwnerType.Player,
                    BuildTownName(from),
                    point.X,
                    point.Y,
                    point.Z);

                if (StewardsRuntime.TownService.AuditSink != null)
                {
                    StewardsRuntime.TownService.AuditSink.Append(
                        AuditEventType.TownFounded,
                        from.Name,
                        town.TownId,
                        "Stewardship deed founded settlement at (" + point.X + "," + point.Y + "," + point.Z + ")");
                }

                from.SendMessage("Settlement founded: {0}.", town.Name);
                _deed.Delete();
            }

            private static string BuildTownName(Mobile from)
            {
                string owner = from.Name;
                if (string.IsNullOrEmpty(owner))
                {
                    owner = "Governor";
                }

                return owner + "'s Settlement";
            }
        }
    }
}
