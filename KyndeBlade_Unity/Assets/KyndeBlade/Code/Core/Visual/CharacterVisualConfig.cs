using System;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Maps a character key to visual overrides. Assign real sprites here
    /// to replace placeholders without touching code. One asset per character
    /// or one shared asset with an array of entries.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterVisualConfig", menuName = "KyndeBlade/Character Visual Config")]
    public class CharacterVisualConfig : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string CharacterKey;
            [Tooltip("Override sprite. Leave null to use placeholder.")]
            public Sprite Sprite;
            [Tooltip("Override idle color tint. Alpha 0 = use default.")]
            public Color ColorOverride = new Color(0, 0, 0, 0);
            [Tooltip("Override scale. Zero = use default.")]
            public Vector3 ScaleOverride = Vector3.zero;
            [Tooltip("Idle bob speed multiplier (1 = default).")]
            public float AnimationSpeed = 1f;
        }

        public Entry[] Entries = new Entry[0];

        /// <summary>Find entry by character key (case-insensitive). Returns null if not found.</summary>
        public Entry Find(string characterKey)
        {
            if (string.IsNullOrEmpty(characterKey)) return null;
            var lower = characterKey.ToLowerInvariant();
            foreach (var e in Entries)
                if (e != null && !string.IsNullOrEmpty(e.CharacterKey) &&
                    e.CharacterKey.ToLowerInvariant() == lower)
                    return e;
            return null;
        }
    }
}
