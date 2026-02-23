using System;
using StewardsOfSosaria.Core;
using Server;

namespace StewardsOfSosaria.Persistence
{
    /// <summary>
    /// Shard-compatible serializers for v1 stewardship models.
    /// Keeps persistence logic centralized and versioned.
    /// </summary>
    public static class StewardsPersistenceSerializer
    {
        public static void WriteTownAggregate(GenericWriter writer, TownAggregate town)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (town == null)
            {
                throw new ArgumentNullException("town");
            }

            writer.Write((int)0); // version
            WriteGuid(writer, town.TownId);
            writer.Write(town.Name);
            writer.Write((int)town.OwnerType);
            WriteGuid(writer, town.OwnerId);
            writer.Write(town.BoundaryRegionId);
            writer.Write(town.CenterX);
            writer.Write(town.CenterY);
            writer.Write(town.CenterZ);
            writer.Write(town.Width);
            writer.Write(town.Height);
            writer.Write((int)town.SimProfile);
            writer.Write((int)town.ThreatState);
            writer.Write(town.Meters.Prosperity);
            writer.Write(town.Meters.Security);
            writer.Write(town.Meters.Health);
            writer.Write(town.Meters.Morale);
            writer.Write(town.Version);
        }

        public static TownAggregate ReadTownAggregate(GenericReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            int version = reader.ReadInt();
            if (version != 0)
            {
                throw new InvalidOperationException("Unsupported TownAggregate version: " + version.ToString());
            }

            TownAggregate town = new TownAggregate();
            town.TownId = ReadGuid(reader);
            town.Name = reader.ReadString();
            town.OwnerType = (TownOwnerType)reader.ReadInt();
            town.OwnerId = ReadGuid(reader);
            town.BoundaryRegionId = reader.ReadString();
            town.CenterX = reader.ReadInt();
            town.CenterY = reader.ReadInt();
            town.CenterZ = reader.ReadInt();
            town.Width = reader.ReadInt();
            town.Height = reader.ReadInt();
            town.SimProfile = (SimProfile)reader.ReadInt();
            town.ThreatState = (ThreatState)reader.ReadInt();
            town.Meters.Prosperity = reader.ReadInt();
            town.Meters.Security = reader.ReadInt();
            town.Meters.Health = reader.ReadInt();
            town.Meters.Morale = reader.ReadInt();
            town.Version = reader.ReadInt();
            return town;
        }

        public static void WriteTownTask(GenericWriter writer, TownTask task)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (task == null)
            {
                throw new ArgumentNullException("task");
            }

            writer.Write((int)0); // version
            WriteGuid(writer, task.TaskId);
            WriteGuid(writer, task.TownId);
            writer.Write((int)task.Type);
            writer.Write(task.Priority);
            writer.Write((int)task.Status);

            writer.Write(task.DependencyTaskIds.Count);
            int i;
            for (i = 0; i < task.DependencyTaskIds.Count; i++)
            {
                WriteGuid(writer, task.DependencyTaskIds[i]);
            }

            writer.Write(task.Reservations.Count);
            for (i = 0; i < task.Reservations.Count; i++)
            {
                WriteReservationToken(writer, task.Reservations[i]);
            }

            writer.Write(task.FailureReason);
        }

        public static TownTask ReadTownTask(GenericReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            int version = reader.ReadInt();
            if (version != 0)
            {
                throw new InvalidOperationException("Unsupported TownTask version: " + version.ToString());
            }

            TownTask task = new TownTask();
            task.TaskId = ReadGuid(reader);
            task.TownId = ReadGuid(reader);
            task.Type = (TownTaskType)reader.ReadInt();
            task.Priority = reader.ReadInt();
            task.Status = (TownTaskStatus)reader.ReadInt();

            int dependencyCount = reader.ReadInt();
            int i;
            for (i = 0; i < dependencyCount; i++)
            {
                task.DependencyTaskIds.Add(ReadGuid(reader));
            }

            int reservationCount = reader.ReadInt();
            for (i = 0; i < reservationCount; i++)
            {
                task.Reservations.Add(ReadReservationToken(reader));
            }

            task.FailureReason = reader.ReadString();
            return task;
        }

        public static void WriteTownNpcProfile(GenericWriter writer, TownNpcProfile npc)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            if (npc == null)
            {
                throw new ArgumentNullException("npc");
            }

            writer.Write((int)0); // version
            writer.Write(npc.NpcSerial);
            WriteGuid(writer, npc.TownId);
            writer.Write((int)npc.AutonomyMode);
            writer.Write(npc.PossessionCooldownUntilUtc);
            writer.Write(npc.LastCombatUtc);
        }

        public static TownNpcProfile ReadTownNpcProfile(GenericReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            int version = reader.ReadInt();
            if (version != 0)
            {
                throw new InvalidOperationException("Unsupported TownNpcProfile version: " + version.ToString());
            }

            TownNpcProfile npc = new TownNpcProfile();
            npc.NpcSerial = reader.ReadInt();
            npc.TownId = ReadGuid(reader);
            npc.AutonomyMode = (NpcAutonomyMode)reader.ReadInt();
            npc.PossessionCooldownUntilUtc = reader.ReadDateTime();
            npc.LastCombatUtc = reader.ReadDateTime();
            return npc;
        }


        private static void WriteGuid(GenericWriter writer, Guid value)
        {
            writer.Write(value.ToString());
        }

        private static Guid ReadGuid(GenericReader reader)
        {
            string raw = reader.ReadString();
            if (string.IsNullOrEmpty(raw))
            {
                return Guid.Empty;
            }

            return new Guid(raw);
        }

        private static void WriteReservationToken(GenericWriter writer, ReservationToken token)
        {
            WriteGuid(writer, token.TokenId);
            WriteGuid(writer, token.TaskId);
            writer.Write(token.ItemSerial);
            writer.Write(token.Amount);
            writer.Write(token.ExpiresAtUtc);
        }

        private static ReservationToken ReadReservationToken(GenericReader reader)
        {
            ReservationToken token = new ReservationToken();
            token.TokenId = ReadGuid(reader);
            token.TaskId = ReadGuid(reader);
            token.ItemSerial = reader.ReadInt();
            token.Amount = reader.ReadInt();
            token.ExpiresAtUtc = reader.ReadDateTime();
            return token;
        }
    }
}
