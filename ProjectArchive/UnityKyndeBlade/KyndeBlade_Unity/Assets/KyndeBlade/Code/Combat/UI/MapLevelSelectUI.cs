using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>Hub/level select UI for map. Shows current location and next locations as buttons.</summary>
    [RequireComponent(typeof(Canvas))]
    public class MapLevelSelectUI : MonoBehaviour
    {
        [Header("References")]
        public WorldMapManager WorldMap;
        [Tooltip("Optional palette for generated map UI.")]
        public KyndeHudTheme HudTheme;
        public RectTransform LocationButtonsRoot;
        public GameObject LocationButtonPrefab;

        [Header("Labels")]
        public Text CurrentLocationText;
        public Text TitleText;

        void Start()
        {
            if (WorldMap == null) WorldMap = UnityEngine.Object.FindFirstObjectByType<WorldMapManager>();
            EnsureUIRoots();
            if (WorldMap != null)
            {
                WorldMap.OnLocationChanged += Refresh;
                Refresh(WorldMap.CurrentLocation);
            }
        }

        void EnsureUIRoots()
        {
            if (LocationButtonsRoot != null) return;
            var canvas = GetComponent<Canvas>();
            if (canvas == null) return;
            var root = new GameObject("MapPanel");
            root.transform.SetParent(transform, false);
            var rootRect = root.AddComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.2f, 0.2f);
            rootRect.anchorMax = new Vector2(0.8f, 0.8f);
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;
            var title = new GameObject("Title");
            title.transform.SetParent(root.transform, false);
            var titleRect = title.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 0.85f);
            titleRect.anchorMax = new Vector2(1, 1f);
            titleRect.offsetMin = Vector2.zero;
            titleRect.offsetMax = Vector2.zero;
            var titleText = title.AddComponent<Text>();
            titleText.text = "Kynde Blade — Select Location";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 20;
            titleText.alignment = TextAnchor.MiddleCenter;
            ManuscriptUITheme.ApplyToText(titleText, emphasis: true);
            if (HudTheme != null) titleText.color = HudTheme.AccentTextColor;
            var current = new GameObject("CurrentLocation");
            current.transform.SetParent(root.transform, false);
            var currentRect = current.AddComponent<RectTransform>();
            currentRect.anchorMin = new Vector2(0, 0.75f);
            currentRect.anchorMax = new Vector2(1, 0.85f);
            currentRect.offsetMin = Vector2.zero;
            currentRect.offsetMax = Vector2.zero;
            CurrentLocationText = current.AddComponent<Text>();
            CurrentLocationText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            CurrentLocationText.fontSize = 16;
            CurrentLocationText.alignment = TextAnchor.MiddleCenter;
            ManuscriptUITheme.ApplyToText(CurrentLocationText, lapis: true);
            var buttonsGo = new GameObject("LocationButtons");
            buttonsGo.transform.SetParent(root.transform, false);
            var buttonsRect = buttonsGo.AddComponent<RectTransform>();
            buttonsRect.anchorMin = new Vector2(0.1f, 0.1f);
            buttonsRect.anchorMax = new Vector2(0.9f, 0.75f);
            buttonsRect.offsetMin = Vector2.zero;
            buttonsRect.offsetMax = Vector2.zero;
            LocationButtonsRoot = buttonsRect;
        }

        void OnDestroy()
        {
            if (WorldMap != null)
                WorldMap.OnLocationChanged -= Refresh;
        }

        public void Refresh(LocationNode current)
        {
            var saveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            if (CurrentLocationText != null && current != null)
            {
                var label = current.DisplayName ?? current.LocationId;
                if (current.IsRealLife)
                    label += " — Malvern, England";
                if (current.IsInescapable)
                    label += " — Inescapable";
                if (saveManager?.CurrentProgress != null)
                {
                    if (string.Equals(current.LocationId, "green_chapel", System.StringComparison.OrdinalIgnoreCase) && saveManager.CurrentProgress.GreenChapelBodiesAccrued > 0)
                        label += $" — {saveManager.CurrentProgress.GreenChapelBodiesAccrued} bod{(saveManager.CurrentProgress.GreenChapelBodiesAccrued == 1 ? "y" : "ies")} lie here";
                    if (string.Equals(current.LocationId, "otherworld", System.StringComparison.OrdinalIgnoreCase))
                    {
                        var souls = saveManager.CurrentProgress.OtherworldLivingCharactersAccrued;
                        var bodies = saveManager.CurrentProgress.OtherworldBodiesFromDeath;
                        if (souls > 0 || bodies > 0)
                        {
                            var parts = new System.Collections.Generic.List<string>();
                            if (souls > 0) parts.Add($"{souls} soul{(souls == 1 ? "" : "s")}");
                            if (bodies > 0) parts.Add($"{bodies} form{(bodies == 1 ? "" : "s")} of the dead");
                            label += " — " + string.Join(", ", parts) + " abide here";
                        }
                    }
                }
                CurrentLocationText.text = label;
            }

            if (LocationButtonsRoot == null) return;

            foreach (Transform c in LocationButtonsRoot)
                Destroy(c.gameObject);

            if (current != null && current.IsInescapable)
            {
                var msg = new GameObject("InescapableMessage");
                msg.transform.SetParent(LocationButtonsRoot, false);
                var rect = msg.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(300, 60);
                var text = msg.AddComponent<Text>();
                text.text = "Alternate Ending — No Return";
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                text.fontSize = 14;
                text.alignment = TextAnchor.MiddleCenter;
                ManuscriptUITheme.ApplyToText(text, lapis: true);
                return;
            }

            if (current != null && current.IsWaitingForGrace)
            {
                var msg = new GameObject("WaitingForGraceMessage");
                msg.transform.SetParent(LocationButtonsRoot, false);
                var rect = msg.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(400, 80);
                var text = msg.AddComponent<Text>();
                text.text = "Thou waitest for Grace. She does not come. The game runs.";
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                text.fontSize = 16;
                text.alignment = TextAnchor.MiddleCenter;
                ManuscriptUITheme.ApplyToText(text, lapis: true);
                return;
            }

            var next = WorldMap != null ? WorldMap.GetNextLocations() : new List<LocationNode>();

            foreach (var loc in next)
            {
                var btn = CreateLocationButton(loc, saveManager);
                if (btn != null)
                    btn.transform.SetParent(LocationButtonsRoot, false);
            }
        }

        GameObject CreateLocationButton(LocationNode loc, SaveManager saveManager)
        {
            GameObject go;
            if (LocationButtonPrefab != null)
            {
                go = Instantiate(LocationButtonPrefab);
            }
            else
            {
                go = new GameObject("Btn_" + loc.LocationId);
                var rect = go.AddComponent<RectTransform>();
                rect.sizeDelta = new Vector2(200, 40);
                var btn = go.AddComponent<Button>();
                var img = go.AddComponent<Image>();
                img.color = ManuscriptUITheme.ParchmentAged;
                var textGo = new GameObject("Text");
                textGo.transform.SetParent(go.transform, false);
                var text = textGo.AddComponent<Text>();
                text.text = loc.DisplayName ?? loc.LocationId;
                text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                text.fontSize = 14;
                text.alignment = TextAnchor.MiddleCenter;
                var textRect = text.rectTransform;
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
                ManuscriptUITheme.ApplyToButton(btn);
            }

            var button = go.GetComponent<Button>();
            var textComp = go.GetComponentInChildren<Text>();
            if (textComp != null)
            {
                var label = loc.DisplayName ?? loc.LocationId;
                if (loc.IsRealLife)
                    label += " (Malvern)";
                var sm = saveManager != null ? saveManager : UnityEngine.Object.FindFirstObjectByType<SaveManager>();
                if (sm?.CurrentProgress != null)
                {
                    if (string.Equals(loc.LocationId, "green_chapel", System.StringComparison.OrdinalIgnoreCase) && sm.CurrentProgress.GreenChapelBodiesAccrued > 0)
                        label += $" ({sm.CurrentProgress.GreenChapelBodiesAccrued} bodies)";
                    if (string.Equals(loc.LocationId, "otherworld", System.StringComparison.OrdinalIgnoreCase) && sm.CurrentProgress.OtherworldLivingCharactersAccrued > 0)
                        label += $" ({sm.CurrentProgress.OtherworldLivingCharactersAccrued} souls)";
                }
                textComp.text = label;
            }

            var unlocked = saveManager == null || saveManager.IsUnlocked(loc.LocationId);
            if (button != null)
            {
                button.interactable = unlocked;
                var trans = go.GetComponent<SceneTransition>();
                if (trans == null) trans = go.AddComponent<SceneTransition>();
                trans.TargetLocation = loc;
                trans.WorldMap = WorldMap;
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(trans.Transition);
            }

            return go;
        }
    }
}
