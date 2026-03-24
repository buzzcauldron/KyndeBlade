using System;
using UnityEngine;
using KyndeBlade.Combat;

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
        public AudioClip RangedHitClip;
        public AudioClip CounterHitClip;
        public AudioClip SpecialClip;
        public AudioClip HealClip;

        AudioSource _audio;
        Camera _mainCamera;
        WaitForSeconds _flashWait;

        AudioClip _proceduralHit;
        AudioClip _proceduralDodge;
        AudioClip _proceduralParry;
        AudioClip _proceduralFail;
        CombatActionType _pendingDamageAudioType = CombatActionType.Rest;
        bool _hasPendingDamageAudioType;

        void Awake()
        {
            _audio = GetComponent<AudioSource>();
            if (_audio == null) _audio = gameObject.AddComponent<AudioSource>();
            _flashWait = new WaitForSeconds(SuccessFlashDuration);
            EnsureProceduralClips();
        }

        void EnsureProceduralClips()
        {
            var lib = AudioLibrary.LoadFromResources();
            if (HitClip == null) HitClip = lib != null ? lib.Hit : null;
            if (RangedHitClip == null) RangedHitClip = lib != null ? lib.Hit : null;
            if (CounterHitClip == null) CounterHitClip = lib != null ? lib.Hit : null;
            if (SpecialClip == null) SpecialClip = lib != null ? lib.ParrySuccess : null;
            if (HealClip == null) HealClip = lib != null ? lib.Heal : null;
            if (DodgeSuccessClip == null) DodgeSuccessClip = lib != null ? lib.DodgeSuccess : null;
            if (ParrySuccessClip == null) ParrySuccessClip = lib != null ? lib.ParrySuccess : null;
            if (DodgeFailClip == null) DodgeFailClip = lib != null ? lib.Fail : null;
            if (ParryFailClip == null) ParryFailClip = lib != null ? lib.Fail : null;

            if (HitClip == null) { _proceduralHit = ProceduralAudioFactory.Hit(); HitClip = _proceduralHit; }
            if (RangedHitClip == null) RangedHitClip = ProceduralAudioFactory.DodgeSuccess();
            if (CounterHitClip == null) CounterHitClip = _proceduralHit != null ? _proceduralHit : ProceduralAudioFactory.Hit();
            if (SpecialClip == null) SpecialClip = _proceduralParry != null ? _proceduralParry : ProceduralAudioFactory.ParrySuccess();
            if (HealClip == null) HealClip = ProceduralAudioFactory.Heal();
            if (DodgeSuccessClip == null) { _proceduralDodge = ProceduralAudioFactory.DodgeSuccess(); DodgeSuccessClip = _proceduralDodge; }
            if (ParrySuccessClip == null) { _proceduralParry = ProceduralAudioFactory.ParrySuccess(); ParrySuccessClip = _proceduralParry; }
            if (DodgeFailClip == null) { _proceduralFail = ProceduralAudioFactory.Fail(); DodgeFailClip = _proceduralFail; }
            if (ParryFailClip == null) ParryFailClip = _proceduralFail != null ? _proceduralFail : ProceduralAudioFactory.Fail();
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
            PlayClip(GetDamageClip());
            FlashCharacter(target, FailureColor);
            if (ScreenShakeOnHit > 0f) ShakeScreen();
        }

        public void OnActionCommitted(CombatAction action)
        {
            if (action == null || action.ActionData == null) return;
            var actionType = action.ActionData.ActionType;
            switch (actionType)
            {
                case CombatActionType.Strike:
                case CombatActionType.RangedStrike:
                case CombatActionType.Counter:
                case CombatActionType.Special:
                    _pendingDamageAudioType = actionType;
                    _hasPendingDamageAudioType = true;
                    break;
                case CombatActionType.Heal:
                    PlayClip(HealClip);
                    break;
            }
        }

        AudioClip GetDamageClip()
        {
            if (!_hasPendingDamageAudioType)
                return HitClip;

            _hasPendingDamageAudioType = false;
            switch (_pendingDamageAudioType)
            {
                case CombatActionType.RangedStrike: return RangedHitClip != null ? RangedHitClip : HitClip;
                case CombatActionType.Counter: return CounterHitClip != null ? CounterHitClip : HitClip;
                case CombatActionType.Special: return SpecialClip != null ? SpecialClip : HitClip;
                default: return HitClip;
            }
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
