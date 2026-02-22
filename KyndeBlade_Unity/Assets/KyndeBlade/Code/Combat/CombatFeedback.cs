using System;
using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Visual and audio feedback for combat (Hodent: Perception, clear feedback).</summary>
    public class CombatFeedback : MonoBehaviour
    {
        [Header("Visual")]
        public float SuccessFlashDuration = 0.2f;
        public Color SuccessColor = Color.green;
        public Color FailureColor = Color.red;
        public float ScreenShakeOnHit = 0.1f;

        [Header("Audio (assign in Inspector)")]
        public AudioClip DodgeSuccessClip;
        public AudioClip DodgeFailClip;
        public AudioClip ParrySuccessClip;
        public AudioClip ParryFailClip;
        public AudioClip HitClip;

        AudioSource _audio;
        Camera _mainCamera;
        WaitForSeconds _flashWait;

        void Awake()
        {
            _audio = GetComponent<AudioSource>();
            if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
            _flashWait = new WaitForSeconds(SuccessFlashDuration);
        }

        public void OnDodgeSuccess(MedievalCharacter character)
        {
            PlayClip(DodgeSuccessClip);
            FlashCharacter(character, SuccessColor);
        }

        public void OnDodgeFail(MedievalCharacter character)
        {
            PlayClip(DodgeFailClip);
            FlashCharacter(character, FailureColor);
        }

        public void OnParrySuccess(MedievalCharacter character)
        {
            PlayClip(ParrySuccessClip);
            FlashCharacter(character, SuccessColor);
        }

        public void OnParryFail(MedievalCharacter character)
        {
            PlayClip(ParryFailClip);
            FlashCharacter(character, FailureColor);
        }

        public void OnDamageDealt(MedievalCharacter target)
        {
            PlayClip(HitClip);
            FlashCharacter(target, FailureColor);
            if (ScreenShakeOnHit > 0f) ShakeScreen();
        }

        void PlayClip(AudioClip clip)
        {
            if (clip != null && _audio != null) _audio.PlayOneShot(clip);
        }

        void FlashCharacter(MedievalCharacter c, Color color)
        {
            if (c == null) return;
            var sr = c.GetComponent<SpriteRenderer>();
            if (sr != null)
                StartCoroutine(FlashRoutine(sr, color));
        }

        System.Collections.IEnumerator FlashRoutine(SpriteRenderer sr, Color color)
        {
            var orig = sr.color;
            sr.color = color;
            yield return _flashWait;
            sr.color = orig;
        }

        void ShakeScreen()
        {
            if (ScreenShakeOnHit <= 0f) return;
            if (_mainCamera == null) _mainCamera = Camera.main;
            _mainCamera?.transform.Translate(UnityEngine.Random.insideUnitSphere * ScreenShakeOnHit);
        }
    }
}
