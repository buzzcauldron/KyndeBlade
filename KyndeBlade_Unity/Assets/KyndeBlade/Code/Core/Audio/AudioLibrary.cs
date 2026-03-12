using UnityEngine;

namespace KyndeBlade
{
    /// <summary>
    /// Centralized audio clip references. Assign real clips here to replace
    /// procedural placeholders. All combat/UI systems pull from this library.
    /// </summary>
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "KyndeBlade/Audio Library")]
    public class AudioLibrary : ScriptableObject
    {
        [Header("Combat")]
        public AudioClip Hit;
        public AudioClip ParrySuccess;
        public AudioClip DodgeSuccess;
        public AudioClip Fail;
        public AudioClip Heal;

        [Header("UI")]
        public AudioClip MenuClick;
        public AudioClip PageTurn;
        public AudioClip BlessingSelect;
        public AudioClip VictoryFanfare;
        public AudioClip DefeatSting;

        [Header("Music")]
        public AudioClip DefaultTheme;
        public AudioClip GreenKnightTheme;
        public AudioClip OrfeoTheme;
        public AudioClip MenuTheme;

        [Header("Status Effects")]
        public AudioClip BurnCrackle;
        public AudioClip FrostShatter;
        public AudioClip StunImpact;
        public AudioClip PoisonBubble;

        /// <summary>
        /// Loads the AudioLibrary from Resources, or returns null if none exists.
        /// Place the asset at Resources/AudioLibrary.
        /// </summary>
        public static AudioLibrary LoadFromResources()
        {
            return Resources.Load<AudioLibrary>("AudioLibrary");
        }
    }
}
