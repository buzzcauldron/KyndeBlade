using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>
    /// Manuscript-themed visual world map with positioned location nodes,
    /// paths between them, and a pulsing current-location indicator.
    /// </summary>
    public class VisualWorldMap : MonoBehaviour
    {
        [Header("Data")]
        public MapLocationPositions Positions;
        public WorldMapManager WorldMapManager;

        [Header("Visual")]
        public float NodeSize = 30f;
        public float PathWidth = 2f;
        public float PulseSpeed = 2f;

        RectTransform _root;
        readonly Dictionary<string, RectTransform> _nodeRects = new Dictionary<string, RectTransform>();
        readonly List<GameObject> _pathObjects = new List<GameObject>();
        string _currentLocationId;
        Image _currentPulse;
        float _pulsePhase;

        void Start()
        {
            if (WorldMapManager == null) WorldMapManager = Object.FindFirstObjectByType<WorldMapManager>();
            _root = GetComponent<RectTransform>();
            if (_root == null)
            {
                _root = gameObject.AddComponent<RectTransform>();
                _root.anchorMin = Vector2.zero;
                _root.anchorMax = Vector2.one;
                _root.offsetMin = _root.offsetMax = Vector2.zero;
            }
            EnsureBackground();
        }

        void Update()
        {
            if (_currentPulse != null)
            {
                _pulsePhase += Time.deltaTime * PulseSpeed;
                float alpha = 0.5f + 0.5f * Mathf.Sin(_pulsePhase);
                var c = ManuscriptUITheme.Gold;
                c.a = alpha;
                _currentPulse.color = c;
            }
        }

        /// <summary>Rebuild the full map from current data.</summary>
        public void Build(List<LocationNode> allLocations, LocationNode current, SaveManager saveManager)
        {
            if (_root == null)
            {
                _root = GetComponent<RectTransform>();
                if (_root == null)
                {
                    _root = gameObject.AddComponent<RectTransform>();
                    _root.anchorMin = Vector2.zero;
                    _root.anchorMax = Vector2.one;
                    _root.offsetMin = _root.offsetMax = Vector2.zero;
                }
            }
            Clear();
            if (Positions == null || allLocations == null) return;

            foreach (var loc in allLocations)
            {
                if (loc == null) continue;
                Vector2 pos;
                if (!Positions.TryGetPosition(loc.LocationId, out pos)) continue;

                bool visited = saveManager != null && saveManager.HasVisited(loc.LocationId);
                bool unlocked = saveManager == null || saveManager.IsUnlocked(loc.LocationId);
                bool isCurrent = current != null && loc.LocationId == current.LocationId;

                var nodeGo = CreateNode(loc, pos, visited, unlocked, isCurrent);
                _nodeRects[loc.LocationId] = nodeGo.GetComponent<RectTransform>();

                if (isCurrent)
                {
                    _currentLocationId = loc.LocationId;
                    _currentPulse = nodeGo.GetComponent<Image>();
                }
            }

            foreach (var loc in allLocations)
            {
                if (loc?.NextLocationIds == null) continue;
                foreach (var nextId in loc.NextLocationIds)
                {
                    RectTransform fromRect, toRect;
                    if (_nodeRects.TryGetValue(loc.LocationId, out fromRect) &&
                        _nodeRects.TryGetValue(nextId, out toRect))
                        CreatePath(fromRect, toRect);
                }
            }
        }

        void Clear()
        {
            foreach (var kv in _nodeRects)
                if (kv.Value != null) Destroy(kv.Value.gameObject);
            _nodeRects.Clear();
            foreach (var p in _pathObjects)
                if (p != null) Destroy(p);
            _pathObjects.Clear();
            _currentPulse = null;
        }

        GameObject CreateNode(LocationNode loc, Vector2 normalizedPos, bool visited, bool unlocked, bool isCurrent)
        {
            var go = new GameObject("Node_" + loc.LocationId);
            go.transform.SetParent(_root, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = normalizedPos;
            rect.anchorMax = normalizedPos;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(NodeSize, NodeSize);

            var img = go.AddComponent<Image>();
            if (isCurrent) img.color = ManuscriptUITheme.Gold;
            else if (visited) img.color = ManuscriptUITheme.GoldDark;
            else if (unlocked) img.color = ManuscriptUITheme.ParchmentAged;
            else img.color = new Color(ManuscriptUITheme.ParchmentAged.r, ManuscriptUITheme.ParchmentAged.g, ManuscriptUITheme.ParchmentAged.b, 0.3f);

            if (unlocked)
            {
                var btn = go.AddComponent<Button>();
                ManuscriptUITheme.ApplyToButton(btn, emphasis: isCurrent);
                var trans = go.AddComponent<SceneTransition>();
                trans.TargetLocation = loc;
                trans.WorldMap = WorldMapManager;
                btn.onClick.AddListener(trans.Transition);
                if (!unlocked) btn.interactable = false;
            }

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = new Vector2(0, -1.5f);
            labelRect.anchorMax = new Vector2(1, -0.2f);
            labelRect.offsetMin = new Vector2(-30, 0);
            labelRect.offsetMax = new Vector2(30, 0);
            var t = labelGo.AddComponent<Text>();
            t.text = loc.DisplayName ?? loc.LocationId;
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = 10;
            t.alignment = TextAnchor.UpperCenter;
            ManuscriptUITheme.ApplyToText(t, emphasis: isCurrent);

            return go;
        }

        void CreatePath(RectTransform from, RectTransform to)
        {
            var go = new GameObject("Path");
            go.transform.SetParent(_root, false);
            go.transform.SetAsFirstSibling();
            var img = go.AddComponent<Image>();
            img.color = ManuscriptUITheme.BorderSepia;

            var rect = img.rectTransform;
            var fromPos = from.anchorMin;
            var toPos = to.anchorMin;
            var mid = (fromPos + toPos) * 0.5f;
            rect.anchorMin = mid;
            rect.anchorMax = mid;
            rect.pivot = new Vector2(0.5f, 0.5f);

            float dist = Vector2.Distance(
                new Vector2(fromPos.x * _root.rect.width, fromPos.y * _root.rect.height),
                new Vector2(toPos.x * _root.rect.width, toPos.y * _root.rect.height));
            rect.sizeDelta = new Vector2(Mathf.Max(dist, 1f), PathWidth);

            float angle = Mathf.Atan2(
                (toPos.y - fromPos.y) * _root.rect.height,
                (toPos.x - fromPos.x) * _root.rect.width) * Mathf.Rad2Deg;
            rect.localRotation = Quaternion.Euler(0, 0, angle);

            _pathObjects.Add(go);
        }

        void EnsureBackground()
        {
            var bg = GetComponent<Image>();
            if (bg == null)
            {
                bg = gameObject.AddComponent<Image>();
                bg.color = ManuscriptUITheme.Parchment;
                bg.raycastTarget = false;
            }
        }
    }
}
