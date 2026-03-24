using System;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Manages separated volume channels (Master, Music, SFX, UI).
    /// Provides multiplied volumes for AudioSource assignment.
    /// Persists to PlayerPrefs.</summary>
    public class VolumeManager : MonoBehaviour
    {
        public static VolumeManager Instance { get; private set; }

        [Range(0f, 1f)] public float MasterVolume = 1f;
        [Range(0f, 1f)] public float MusicVolume = 1f;
        [Range(0f, 1f)] public float SFXVolume = 1f;
        [Range(0f, 1f)] public float UIVolume = 1f;

        public event Action OnVolumeChanged;

        const string PrefMaster = "KyndeBlade_Vol_Master";
        const string PrefMusic = "KyndeBlade_Vol_Music";
        const string PrefSFX = "KyndeBlade_Vol_SFX";
        const string PrefUI = "KyndeBlade_Vol_UI";

        public float EffectiveMusicVolume => MasterVolume * MusicVolume;
        public float EffectiveSFXVolume => MasterVolume * SFXVolume;
        public float EffectiveUIVolume => MasterVolume * UIVolume;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Load()
        {
            MasterVolume = PlayerPrefs.GetFloat(PrefMaster, 1f);
            MusicVolume = PlayerPrefs.GetFloat(PrefMusic, 1f);
            SFXVolume = PlayerPrefs.GetFloat(PrefSFX, 1f);
            UIVolume = PlayerPrefs.GetFloat(PrefUI, 1f);
            Apply();
        }

        public void Save()
        {
            PlayerPrefs.SetFloat(PrefMaster, MasterVolume);
            PlayerPrefs.SetFloat(PrefMusic, MusicVolume);
            PlayerPrefs.SetFloat(PrefSFX, SFXVolume);
            PlayerPrefs.SetFloat(PrefUI, UIVolume);
            PlayerPrefs.Save();
        }

        public void SetMaster(float v) { MasterVolume = Mathf.Clamp01(v); Apply(); Save(); }
        public void SetMusic(float v) { MusicVolume = Mathf.Clamp01(v); Apply(); Save(); }
        public void SetSFX(float v) { SFXVolume = Mathf.Clamp01(v); Apply(); Save(); }
        public void SetUI(float v) { UIVolume = Mathf.Clamp01(v); Apply(); Save(); }

        void Apply()
        {
            AudioListener.volume = MasterVolume;
            OnVolumeChanged?.Invoke();
        }

        /// <summary>Apply the correct volume to an AudioSource based on its channel.</summary>
        public void ApplyToSource(AudioSource source, AudioChannel channel)
        {
            if (source == null) return;
            source.volume = channel switch
            {
                AudioChannel.Music => EffectiveMusicVolume,
                AudioChannel.SFX => EffectiveSFXVolume,
                AudioChannel.UI => EffectiveUIVolume,
                _ => MasterVolume
            };
        }
    }

    public enum AudioChannel { Master, Music, SFX, UI }
}
