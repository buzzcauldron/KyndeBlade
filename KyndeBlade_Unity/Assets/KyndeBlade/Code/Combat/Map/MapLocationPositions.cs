using System;
using System.Collections.Generic;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Maps location IDs to normalized (0-1) positions on the visual world map.
    /// ScriptableObject for easy editor re-layout.
    /// </summary>
    [CreateAssetMenu(fileName = "MapLocationPositions", menuName = "KyndeBlade/Map Location Positions")]
    public class MapLocationPositions : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string LocationId;
            [Tooltip("Normalized position on the map (0,0 = bottom-left, 1,1 = top-right).")]
            public Vector2 Position;
        }

        public Entry[] Positions = new Entry[0];

        public bool TryGetPosition(string locationId, out Vector2 pos)
        {
            if (!string.IsNullOrEmpty(locationId))
            {
                var lower = locationId.ToLowerInvariant();
                foreach (var e in Positions)
                    if (e != null && !string.IsNullOrEmpty(e.LocationId) &&
                        e.LocationId.ToLowerInvariant() == lower)
                    {
                        pos = e.Position;
                        return true;
                    }
            }
            pos = Vector2.zero;
            return false;
        }
    }
}
