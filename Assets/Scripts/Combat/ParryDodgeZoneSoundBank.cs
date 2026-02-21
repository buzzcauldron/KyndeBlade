using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Maps CharacterSoundTheme to AudioClips for parry/dodge zone strike warning.</summary>
    public class ParryDodgeZoneSoundBank : MonoBehaviour
    {
        [Header("Strike Warning Clips (assign in Inspector)")]
        public AudioClip DefaultClip;
        public AudioClip ChurchBellClip;
        public AudioClip CreakingClip;
        public AudioClip SloppyClip;
        public AudioClip WrathClip;
        public AudioClip GreedClip;
        public AudioClip SlothClip;
        public AudioClip PrideClip;
        public AudioClip EnvyClip;

        AudioSource _audio;

        void Awake()
        {
            _audio = GetComponent<AudioSource>();
            if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
        }

        public void PlayStrikeWarning(MedievalCharacter attacker)
        {
            var clip = GetClipForCharacter(attacker);
            if (clip != null && _audio != null)
                _audio.PlayOneShot(clip);
        }

        public AudioClip GetClipForCharacter(MedievalCharacter c)
        {
            var theme = c != null ? c.SoundTheme : CharacterSoundTheme.Default;
            return GetClipForTheme(theme);
        }

        public AudioClip GetClipForTheme(CharacterSoundTheme theme)
        {
            switch (theme)
            {
                case CharacterSoundTheme.ChurchBell: return ChurchBellClip;
                case CharacterSoundTheme.Creaking: return CreakingClip;
                case CharacterSoundTheme.Sloppy: return SloppyClip;
                case CharacterSoundTheme.Wrath: return WrathClip;
                case CharacterSoundTheme.Greed: return GreedClip;
                case CharacterSoundTheme.Sloth: return SlothClip;
                case CharacterSoundTheme.Pride: return PrideClip;
                case CharacterSoundTheme.Envy: return EnvyClip;
                default: return DefaultClip;
            }
        }
    }
}
