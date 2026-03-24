using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace KyndeBlade
{
    /// <summary>Centralized input actions for the game. Creates action maps programmatically
    /// so no .inputactions asset is required. Supports keyboard and gamepad.</summary>
    public class KyndeBladeInputActions : MonoBehaviour, IDisposable
    {
        public static KyndeBladeInputActions Instance { get; private set; }

        InputActionMap _combatMap;
        InputActionMap _uiMap;
        InputActionMap _menuMap;

        public InputAction Dodge { get; private set; }
        public InputAction Parry { get; private set; }
        public InputAction Counter { get; private set; }

        public InputAction Navigate { get; private set; }
        public InputAction Submit { get; private set; }
        public InputAction Cancel { get; private set; }

        public InputAction MenuPause { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildActions();
            EnableAll();
        }

        void OnDestroy()
        {
            if (Instance == this) Instance = null;
            Dispose();
        }

        void BuildActions()
        {
            _combatMap = new InputActionMap("Combat");
            Dodge = _combatMap.AddAction("Dodge", InputActionType.Button);
            Dodge.AddBinding("<Keyboard>/space");
            Dodge.AddBinding("<Keyboard>/leftAlt");
            Dodge.AddBinding("<Mouse>/rightButton");
            Dodge.AddBinding("<Gamepad>/buttonSouth");

            Parry = _combatMap.AddAction("Parry", InputActionType.Button);
            Parry.AddBinding("<Keyboard>/leftShift");
            Parry.AddBinding("<Keyboard>/f");
            Parry.AddBinding("<Mouse>/leftButton");
            Parry.AddBinding("<Gamepad>/buttonWest");

            Counter = _combatMap.AddAction("Counter", InputActionType.Button);
            Counter.AddBinding("<Keyboard>/e");
            Counter.AddBinding("<Keyboard>/r");
            Counter.AddBinding("<Gamepad>/buttonNorth");

            _uiMap = new InputActionMap("UI");
            Navigate = _uiMap.AddAction("Navigate", InputActionType.Value);
            Navigate.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/rightArrow");
            Navigate.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a")
                .With("Right", "<Keyboard>/d");
            Navigate.AddBinding("<Gamepad>/leftStick");
            Navigate.AddBinding("<Gamepad>/dpad");

            Submit = _uiMap.AddAction("Submit", InputActionType.Button);
            Submit.AddBinding("<Keyboard>/enter");
            Submit.AddBinding("<Keyboard>/space");
            Submit.AddBinding("<Gamepad>/buttonSouth");

            Cancel = _uiMap.AddAction("Cancel", InputActionType.Button);
            Cancel.AddBinding("<Keyboard>/escape");
            Cancel.AddBinding("<Gamepad>/buttonEast");

            _menuMap = new InputActionMap("Menu");
            MenuPause = _menuMap.AddAction("Pause", InputActionType.Button);
            MenuPause.AddBinding("<Keyboard>/escape");
            MenuPause.AddBinding("<Gamepad>/start");
        }

        public void EnableAll()
        {
            _combatMap?.Enable();
            _uiMap?.Enable();
            _menuMap?.Enable();
        }

        public void EnableCombatOnly()
        {
            _combatMap?.Enable();
            _uiMap?.Disable();
            _menuMap?.Enable();
        }

        public void EnableUIOnly()
        {
            _combatMap?.Disable();
            _uiMap?.Enable();
            _menuMap?.Enable();
        }

        /// <summary>Apply saved rebindings from PlayerPrefs. Call after building actions.</summary>
        public void LoadRebindings()
        {
            string json = PlayerPrefs.GetString("KyndeBlade_Rebindings", "");
            if (string.IsNullOrEmpty(json)) return;
            ApplyRebindingsJson(_combatMap, json);
            ApplyRebindingsJson(_uiMap, json);
            ApplyRebindingsJson(_menuMap, json);
        }

        public void SaveRebindings()
        {
            string json = "";
            json += SaveMapBindings(_combatMap);
            json += SaveMapBindings(_uiMap);
            json += SaveMapBindings(_menuMap);
            PlayerPrefs.SetString("KyndeBlade_Rebindings", json);
            PlayerPrefs.Save();
        }

        static string SaveMapBindings(InputActionMap map)
        {
            if (map == null) return "";
            string result = "";
            foreach (var action in map)
                result += action.SaveBindingOverridesAsJson();
            return result;
        }

        static void ApplyRebindingsJson(InputActionMap map, string json)
        {
            if (map == null || string.IsNullOrEmpty(json)) return;
            foreach (var action in map)
            {
                try { action.LoadBindingOverridesFromJson(json); }
                catch { /* ignore non-matching bindings */ }
            }
        }

        public void Dispose()
        {
            _combatMap?.Dispose();
            _uiMap?.Dispose();
            _menuMap?.Dispose();
        }
    }
}
