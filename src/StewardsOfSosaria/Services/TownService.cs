using System;
using System.Collections.Generic;
using StewardsOfSosaria.Core;

namespace StewardsOfSosaria.Services
{
    public sealed class TownService : ITownService
    {
        private readonly Dictionary<Guid, TownAggregate> _towns;

        public TownService()
        {
            _towns = new Dictionary<Guid, TownAggregate>();
        }

        public TownAggregate CreateTown(Guid ownerId, TownOwnerType ownerType, string name, int centerX, int centerY, int centerZ)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Town name cannot be empty.", "name");
            }

            if (!CanClaim(centerX, centerY, 60, 61))
            {
                throw new InvalidOperationException("Town claim overlaps an existing settlement boundary.");
            }

            TownAggregate town = new TownAggregate();
            town.TownId = Guid.NewGuid();
            town.OwnerId = ownerId;
            town.OwnerType = ownerType;
            town.Name = name;
            town.CenterX = centerX;
            town.CenterY = centerY;
            town.CenterZ = centerZ;

            _towns[town.TownId] = town;
            return town;
        }

        public bool CanClaim(int centerX, int centerY, int width, int height)
        {
            int candidateLeft = centerX - (width / 2);
            int candidateTop = centerY - (height / 2);
            int candidateRight = candidateLeft + width - 1;
            int candidateBottom = candidateTop + height - 1;

            foreach (TownAggregate existing in _towns.Values)
            {
                int left = existing.CenterX - (existing.Width / 2);
                int top = existing.CenterY - (existing.Height / 2);
                int right = left + existing.Width - 1;
                int bottom = top + existing.Height - 1;

                bool overlapX = !(candidateRight < left || candidateLeft > right);
                bool overlapY = !(candidateBottom < top || candidateTop > bottom);

                if (overlapX && overlapY)
                {
                    return false;
                }
            }

            return true;
        }

        public TownAggregate GetTown(Guid townId)
        {
            TownAggregate town;

            if (_towns.TryGetValue(townId, out town))
            {
                return town;
            }

            return null;
        }
    }
}
