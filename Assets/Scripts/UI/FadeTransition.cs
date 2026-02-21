using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>
    /// Manuscript-style fade overlay. Adapted from GameDev.tv 2D RPG Combat course (UIFade).
    /// Uses parchment/ink colors for map↔combat transitions. See docs/GAMEDEVTV_2D_RPG_COMBAT_LEARNING_PLAN.md.
    /// </summary>
    public class FadeTransition : MonoBehaviour
    {
        [Header("Fade")]
        [Tooltip("Manuscript dark (ink) for fade-to-black. Parchment for fade-to-clear.")]
        public Color FadeColor = ManuscriptUITheme.InkPrimary;
        public float FadeSpeed = 2f;

        Image _overlay;
        Canvas _canvas;
        Coroutine _routine;

        void Awake()
        {
            EnsureOverlay();
        }

        void EnsureOverlay()
        {
            if (_overlay != null) return;

            var canvasGo = new GameObject("FadeOverlay");
            canvasGo.transform.SetParent(transform);

            _canvas = canvasGo.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 32767;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            var imgGo = new GameObject("Image");
            imgGo.transform.SetParent(canvasGo.transform, false);
            var rect = imgGo.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;

            _overlay = imgGo.AddComponent<Image>();
            _overlay.color = new Color(FadeColor.r, FadeColor.g, FadeColor.b, 0f);
            _overlay.raycastTarget = false;
        }

        /// <summary>Fade to opaque (dark). Call before combat or scene change.</summary>
        public void FadeToDark(Action onComplete = null)
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(FadeRoutine(1f, onComplete));
        }

        /// <summary>Fade to clear. Call when returning to map or after transition.</summary>
        public void FadeToClear(Action onComplete = null)
        {
            if (_routine != null) StopCoroutine(_routine);
            _routine = StartCoroutine(FadeRoutine(0f, onComplete));
        }

        IEnumerator FadeRoutine(float targetAlpha, Action onComplete)
        {
            EnsureOverlay();
            var c = _overlay.color;
            c.r = FadeColor.r;
            c.g = FadeColor.g;
            c.b = FadeColor.b;

            while (!Mathf.Approximately(c.a, targetAlpha))
            {
                c.a = Mathf.MoveTowards(c.a, targetAlpha, FadeSpeed * Time.deltaTime);
                _overlay.color = c;
                yield return null;
            }

            _routine = null;
            onComplete?.Invoke();
        }
    }
}
