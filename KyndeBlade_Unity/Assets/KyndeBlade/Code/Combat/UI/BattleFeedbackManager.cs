using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>
    /// VFX/SFX bridge: triggers camera shake and floating text when actions occur.
    /// Subscribe to TurnManager / TurnSequenceController / OnDamageDealt; call TriggerCameraShake and ShowFloatingText.
    /// AI → Bridge (this) → Camera/UI feedback; TurnManager still handles math.
    /// </summary>
    public class BattleFeedbackManager : MonoBehaviour
    {
        [Header("References")]
        public TurnManager TurnManager;
        public TurnSequenceController SequenceController;
        [Tooltip("Optional. Main camera for shake; uses Camera.main if null.")]
        public Camera ShakeCamera;

        [Header("Hit-Stop")]
        [Tooltip("When damage is dealt, freeze time briefly (real seconds). 0 = no hit-stop.")]
        [Range(0f, 0.15f)]
        public float HitStopDurationOnDamage = 0.05f;

        [Header("Camera Shake")]
        [Min(0f)]
        public float ShakeIntensityOnAction = 0.04f;
        public float ShakeDurationOnAction = 0.15f;
        [Min(0f)]
        public float ShakeIntensityOnDamage = 0.08f;
        public float ShakeDurationOnDamage = 0.2f;

        [Header("Floating Text")]
        [Tooltip("Show action name above target during telegraph. Off by default; use for 'intent' popup.")]
        public bool ShowActionNameOnTelegraph;
        [Tooltip("World-space offset above character (e.g. 1.5).")]
        public float FloatTextHeightOffset = 1.5f;
        public float FloatTextDuration = 1.2f;
        public float FloatTextRiseSpeed = 1.2f;
        public int FloatTextFontSize = 32;
        public Color FloatTextDamageColor = new Color(1f, 0.3f, 0.2f);
        public Color FloatTextHealColor = new Color(0.2f, 1f, 0.4f);
        public Color FloatTextActionColor = Color.white;

        Dictionary<MedievalCharacter, System.Action<MedievalCharacter, MedievalCharacter, float>> _damageHandlers = new Dictionary<MedievalCharacter, System.Action<MedievalCharacter, MedievalCharacter, float>>();
        bool _subscribed;
        Coroutine _shakeRoutine;

        void Start()
        {
            if (TurnManager == null) TurnManager = GameRuntime.TurnManager ?? UnityEngine.Object.FindFirstObjectByType<TurnManager>();
            if (SequenceController == null) SequenceController = UnityEngine.Object.FindFirstObjectByType<TurnSequenceController>();
            if (ShakeCamera == null) ShakeCamera = Camera.main;
            ShowActionNameOnTelegraph = true;

            var settings = GameRuntime.GameManager != null ? GameRuntime.GameManager.Settings : null;
            if (settings != null)
            {
                // Keep readability high by limiting excessive shake on lower performance.
                ShakeIntensityOnAction = Mathf.Min(ShakeIntensityOnAction, 0.04f);
                ShakeIntensityOnDamage = Mathf.Min(ShakeIntensityOnDamage, 0.08f);
            }

            Subscribe();
        }

        void OnDestroy()
        {
            Unsubscribe();
        }

        void Subscribe()
        {
            if (_subscribed) return;
            _subscribed = true;

            if (TurnManager != null)
            {
                TurnManager.OnTurnChanged += OnTurnChanged;
                TurnManager.OnActionExecuted += OnActionExecuted;
                TurnManager.OnCombatEnded += UnsubscribeAll;
            }

            if (SequenceController != null)
            {
                SequenceController.OnTelegraphStarted += OnTelegraphStarted;
                SequenceController.OnAnimationStarting += OnAnimationStarting;
            }

            SubscribeToAllCharacters();
        }

        void Unsubscribe()
        {
            if (!_subscribed) return;
            _subscribed = false;

            if (TurnManager != null)
            {
                TurnManager.OnTurnChanged -= OnTurnChanged;
                TurnManager.OnActionExecuted -= OnActionExecuted;
                TurnManager.OnCombatEnded -= UnsubscribeAll;
            }

            if (SequenceController != null)
            {
                SequenceController.OnTelegraphStarted -= OnTelegraphStarted;
                SequenceController.OnAnimationStarting -= OnAnimationStarting;
            }

            UnsubscribeAll();
        }

        void OnTurnChanged(MedievalCharacter _)
        {
            SubscribeToAllCharacters();
        }

        void SubscribeToAllCharacters()
        {
            UnsubscribeAll();
            if (TurnManager == null) return;

            foreach (var c in TurnManager.PlayerCharacters)
                if (c != null) SubscribeToCharacter(c);
            foreach (var c in TurnManager.EnemyCharacters)
                if (c != null) SubscribeToCharacter(c);
        }

        void SubscribeToCharacter(MedievalCharacter c)
        {
            if (_damageHandlers.ContainsKey(c)) return;
            System.Action<MedievalCharacter, MedievalCharacter, float> dmg = (attacker, target, amount) =>
            {
                OnDamageDealt(attacker, target, amount);
            };
            c.OnDamageDealt += dmg;
            _damageHandlers[c] = dmg;
        }

        void UnsubscribeAll()
        {
            foreach (var kv in _damageHandlers)
            {
                if (kv.Key == null) continue;
                kv.Key.OnDamageDealt -= kv.Value;
            }
            _damageHandlers.Clear();
        }

        void OnTelegraphStarted(MedievalCharacter actor, CombatAction action, MedievalCharacter target)
        {
            if (ShowActionNameOnTelegraph && target != null)
                ShowFloatingText(target.transform.position + Vector3.up * FloatTextHeightOffset, action?.ActionData.ActionName ?? "?", FloatTextActionColor);
        }

        void OnAnimationStarting(MedievalCharacter actor, CombatAction action, MedievalCharacter target)
        {
            TriggerCameraShake(ShakeIntensityOnAction, ShakeDurationOnAction);
        }

        void OnActionExecuted(MedievalCharacter executor, MedievalCharacter target, CombatAction action)
        {
            // Light shake when action is committed (may precede damage if deferred to animation)
            TriggerCameraShake(ShakeIntensityOnAction, ShakeDurationOnAction);
        }

        void OnDamageDealt(MedievalCharacter attacker, MedievalCharacter target, float amount)
        {
            if (HitStopDurationOnDamage > 0f)
                HitStop.Trigger(HitStopDurationOnDamage);
            TriggerCameraShake(ShakeIntensityOnDamage, ShakeDurationOnDamage);
            Vector3 pos = target != null ? target.transform.position + Vector3.up * FloatTextHeightOffset : Vector3.zero;
            string text = Mathf.Approximately(amount, Mathf.Floor(amount)) ? ((int)amount).ToString() : amount.ToString("F0");
            ShowFloatingText(pos, "-" + text, FloatTextDamageColor);
        }

        /// <summary>Call from anywhere to trigger a screen shake.</summary>
        public void TriggerCameraShake(float intensity, float duration)
        {
            if (intensity <= 0f || duration <= 0f) return;
            Camera cam = ShakeCamera != null ? ShakeCamera : Camera.main;
            if (cam == null) return;
            if (_shakeRoutine != null)
                StopCoroutine(_shakeRoutine);
            _shakeRoutine = StartCoroutine(ShakeRoutine(cam, intensity, duration));
        }

        /// <summary>Show floating text at world position (e.g. damage number or action name).</summary>
        public void ShowFloatingText(Vector3 worldPosition, string text, Color color)
        {
            if (string.IsNullOrEmpty(text)) return;
            StartCoroutine(FloatingTextRoutine(worldPosition, text, color));
        }

        /// <summary>Convenience: show a heal number in heal color.</summary>
        public void ShowHealText(Vector3 worldPosition, float amount)
        {
            string t = amount >= 0f ? "+" + ((int)amount).ToString() : ((int)amount).ToString();
            ShowFloatingText(worldPosition, t, FloatTextHealColor);
        }

        IEnumerator ShakeRoutine(Camera cam, float intensity, float duration)
        {
            Transform t = cam.transform;
            Vector3 origPos = t.localPosition;
            Quaternion origRot = t.localRotation;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float decay = 1f - (elapsed / duration) * (elapsed / duration); // ease out
                float amp = intensity * decay;
                t.localPosition = origPos + UnityEngine.Random.insideUnitSphere * amp;
                t.localRotation = origRot * Quaternion.Euler(UnityEngine.Random.insideUnitSphere * amp * 10f);
                yield return null;
            }

            t.localPosition = origPos;
            t.localRotation = origRot;
            _shakeRoutine = null;
        }

        IEnumerator FloatingTextRoutine(Vector3 worldPos, string text, Color color)
        {
            var go = new GameObject("FloatingText");
            go.transform.position = worldPos;

            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.fontSize = FloatTextFontSize;
            tm.characterSize = 0.15f;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = color;

            float elapsed = 0f;
            while (elapsed < FloatTextDuration)
            {
                elapsed += Time.deltaTime;
                go.transform.position += Vector3.up * (FloatTextRiseSpeed * Time.deltaTime);
                float a = 1f - (elapsed / FloatTextDuration);
                tm.color = new Color(color.r, color.g, color.b, a);
                yield return null;
            }

            Destroy(go);
        }
    }
}
