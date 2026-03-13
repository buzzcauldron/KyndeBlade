using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using KyndeBlade.Combat;

namespace KyndeBlade
{
    /// <summary>Polls for dodge/parry input and fires events. Keyboard primary, controller add-in. Escapade = dodge, Ward = parry. Uses the new Input System.</summary>
    public class ParryDodgeInputHandler : MonoBehaviour
    {
        [Header("References")]
        public TurnManager TurnManager;
        [Tooltip("Accept dodge/parry input slightly before the defense window opens.")]
        public float EarlyInputGraceSeconds = 0.08f;

        public event Action<float> OnDodgePressed;
        public event Action<float> OnParryPressed;

        [Header("Keyboard (Primary)")]
        public Key DodgeKey = Key.Space;
        public Key ParryKey = Key.LeftShift;
        public Key CounterKey = Key.E;

        [Header("Gamepad (Add-in)")]
        public GamepadButton DodgeButton = GamepadButton.South;
        public GamepadButton ParryButton = GamepadButton.West;
        public GamepadButton CounterButton = GamepadButton.North;
        float _lastDodgePressedTime = -10f;
        float _lastParryPressedTime = -10f;
        bool _wasInWindow;

        bool GetDodgePressed()
        {
            var ia = KyndeBladeInputActions.Instance;
            if (ia != null && ia.Dodge != null && ia.Dodge.WasPressedThisFrame()) return true;
            if (Keyboard.current != null && Keyboard.current[DodgeKey].wasPressedThisFrame) return true;
            if (Gamepad.current != null && Gamepad.current[DodgeButton].wasPressedThisFrame) return true;
            return false;
        }

        bool GetParryPressed()
        {
            var ia = KyndeBladeInputActions.Instance;
            if (ia != null && ia.Parry != null && ia.Parry.WasPressedThisFrame()) return true;
            if (Keyboard.current != null && Keyboard.current[ParryKey].wasPressedThisFrame) return true;
            if (Gamepad.current != null && Gamepad.current[ParryButton].wasPressedThisFrame) return true;
            return false;
        }

        bool GetCounterPressed()
        {
            var ia = KyndeBladeInputActions.Instance;
            if (ia != null && ia.Counter != null && ia.Counter.WasPressedThisFrame()) return true;
            if (Keyboard.current != null && Keyboard.current[CounterKey].wasPressedThisFrame) return true;
            if (Gamepad.current != null && Gamepad.current[CounterButton].wasPressedThisFrame) return true;
            return false;
        }

        void Update()
        {
            if (TurnManager == null) return;

            bool dodgePressed = GetDodgePressed();
            bool parryPressed = GetParryPressed();
            if (dodgePressed) _lastDodgePressedTime = Time.time;
            if (parryPressed) _lastParryPressedTime = Time.time;

            if (TurnManager.IsCounterWindowActive)
            {
                if (GetCounterPressed())
                    TurnManager.ExecuteCounter();
                return;
            }
            if (TurnManager.State != CombatState.RealTimeWindow)
            {
                _wasInWindow = false;
                return;
            }

            float remaining = TurnManager.RealTimeWindowRemaining;
            var actionType = TurnManager.ActionTypeDuringWindow;
            bool enteredWindow = !_wasInWindow;
            _wasInWindow = true;
            float graceSeconds = EarlyInputGraceSeconds;
            if (TurnManager.Settings != null)
                graceSeconds = Mathf.Max(graceSeconds, TurnManager.Settings.DefenseCoyoteSeconds);

            if (actionType == CombatActionType.Escapade)
            {
                bool buffered = enteredWindow && (Time.time - _lastDodgePressedTime) <= graceSeconds;
                if (dodgePressed || buffered)
                    OnDodgePressed?.Invoke(remaining);
            }
            else if (actionType == CombatActionType.Ward)
            {
                bool buffered = enteredWindow && (Time.time - _lastParryPressedTime) <= graceSeconds;
                if (parryPressed || buffered)
                    OnParryPressed?.Invoke(remaining);
            }
        }
    }
}
