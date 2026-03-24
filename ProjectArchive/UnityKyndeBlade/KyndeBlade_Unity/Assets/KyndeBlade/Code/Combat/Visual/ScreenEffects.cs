using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Full-screen post-processing effects via UI overlay:
    /// low-health vignette, status tints, boss phase flash, aging desaturation.</summary>
    public class ScreenEffects : MonoBehaviour
    {
        [Header("Vignette")]
        public float LowHealthThreshold = 0.3f;
        public Color VignetteColor = new Color(0.6f, 0.1f, 0.1f, 0.3f);
        public float VignettePulseSpeed = 2f;

        [Header("Status Tints")]
        public Color FrostTint = new Color(0.5f, 0.7f, 1f, 0.15f);
        public Color BurnTint = new Color(1f, 0.5f, 0.2f, 0.12f);
        public Color PoisonTint = new Color(0.3f, 0.8f, 0.3f, 0.1f);

        [Header("Boss Phase")]
        public Color BossFlashColor = new Color(1f, 1f, 1f, 0.5f);
        public float BossFlashDuration = 0.3f;

        [Header("Aging")]
        [Tooltip("Desaturation amount at Field of Grace (0=none, 1=full grayscale).")]
        public float AgingDesaturation = 0f;

        Image _overlay;
        Canvas _canvas;
        Color _currentTint = Color.clear;
        float _vignetteAlpha;

        void Start()
        {
            EnsureOverlay();
        }

        void EnsureOverlay()
        {
            if (_overlay != null) return;
            var go = new GameObject("ScreenEffectOverlay");
            go.transform.SetParent(transform, false);

            _canvas = go.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 999;
            go.AddComponent<CanvasScaler>();

            var overlayGo = new GameObject("Overlay");
            overlayGo.transform.SetParent(go.transform, false);
            var rect = overlayGo.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            _overlay = overlayGo.AddComponent<Image>();
            _overlay.color = Color.clear;
            _overlay.raycastTarget = false;
        }

        void Update()
        {
            if (_overlay == null) return;
            UpdateVignette();
            _overlay.color = Color.Lerp(_overlay.color, _currentTint, Time.deltaTime * 5f);
        }

        void UpdateVignette()
        {
            var tm = GameRuntime.TurnManager;
            if (tm == null || tm.PlayerCharacters == null) { _currentTint = Color.clear; return; }

            float minHealthRatio = 1f;
            bool hasFrost = false, hasBurn = false, hasPoison = false;

            foreach (var c in tm.PlayerCharacters)
            {
                if (c == null || !c.IsAlive()) continue;
                float ratio = c.Stats.CurrentHealth / Mathf.Max(c.Stats.MaxHealth, 1f);
                if (ratio < minHealthRatio) minHealthRatio = ratio;
                if (c.HasStatusEffect(StatusEffectType.Frost)) hasFrost = true;
                if (c.HasStatusEffect(StatusEffectType.Burning)) hasBurn = true;
                if (c.HasStatusEffect(StatusEffectType.Poison)) hasPoison = true;
            }

            _currentTint = Color.clear;

            if (minHealthRatio < LowHealthThreshold)
            {
                float pulse = (Mathf.Sin(Time.time * VignettePulseSpeed * Mathf.PI) + 1f) * 0.5f;
                float intensity = (1f - minHealthRatio / LowHealthThreshold) * pulse;
                _currentTint = new Color(VignetteColor.r, VignetteColor.g, VignetteColor.b, VignetteColor.a * intensity);
            }

            if (hasFrost) _currentTint = AddTint(_currentTint, FrostTint);
            if (hasBurn) _currentTint = AddTint(_currentTint, BurnTint);
            if (hasPoison) _currentTint = AddTint(_currentTint, PoisonTint);
        }

        Color AddTint(Color current, Color tint)
        {
            return new Color(
                Mathf.Clamp01(current.r + tint.r * tint.a),
                Mathf.Clamp01(current.g + tint.g * tint.a),
                Mathf.Clamp01(current.b + tint.b * tint.a),
                Mathf.Clamp01(current.a + tint.a * 0.5f)
            );
        }

        /// <summary>Trigger a white flash for boss phase transitions.</summary>
        public void TriggerBossPhaseFlash()
        {
            StartCoroutine(FlashRoutine(BossFlashColor, BossFlashDuration));
        }

        /// <summary>Trigger a custom color flash.</summary>
        public void TriggerFlash(Color color, float duration)
        {
            StartCoroutine(FlashRoutine(color, duration));
        }

        IEnumerator FlashRoutine(Color color, float duration)
        {
            if (_overlay == null) yield break;
            _overlay.color = color;
            float t = 0f;
            while (t < duration)
            {
                t += Time.deltaTime;
                float alpha = Mathf.Lerp(color.a, 0f, t / duration);
                _overlay.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }
        }

        /// <summary>Set aging desaturation for Field of Grace (0-1).</summary>
        public void SetAgingDesaturation(float amount)
        {
            AgingDesaturation = Mathf.Clamp01(amount);
            var cam = Camera.main;
            if (cam == null) return;
            float gray = AgingDesaturation * 0.3f;
            cam.backgroundColor = Color.Lerp(cam.backgroundColor,
                new Color(0.5f, 0.5f, 0.5f), gray);
        }
    }
}
