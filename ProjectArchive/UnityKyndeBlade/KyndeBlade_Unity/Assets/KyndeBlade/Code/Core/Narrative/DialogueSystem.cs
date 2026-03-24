using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>Displays story beats and dialogue in manuscript-style UI.</summary>
    [RequireComponent(typeof(Canvas))]
    public class DialogueSystem : MonoBehaviour
    {
        [Header("UI")]
        public RectTransform PanelRoot;
        public Text DialogueText;
        public Text SpeakerText;
        public Image PortraitImage;
        public Image PanelBackground;
        public Image BorderImage;
        public Button ContinueButton;
        [Tooltip("Full-screen vista backdrop; auto-created if null.")]
        public RawImage StoryBackdropImage;

        [Header("Layout")]
        public float PanelWidth = 600f;
        public float PanelHeight = 200f;

        Action _onComplete;
        Coroutine _displayCoroutine;

        void Awake()
        {
            if (PanelRoot == null)
                EnsurePanel();
        }

        void EnsureStoryBackdropLayer()
        {
            if (StoryBackdropImage != null) return;
            var go = new GameObject("StoryBackdrop");
            go.transform.SetParent(transform, false);
            go.transform.SetAsFirstSibling();
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            var raw = go.AddComponent<RawImage>();
            raw.raycastTarget = false;
            raw.color = Color.white;
            go.SetActive(false);
            StoryBackdropImage = raw;
        }

        void EnsurePanel()
        {
            EnsureStoryBackdropLayer();
            var go = new GameObject("DialoguePanel");
            go.transform.SetParent(transform, false);

            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.2f);
            rect.anchorMax = new Vector2(0.5f, 0.2f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(PanelWidth, PanelHeight);
            rect.anchoredPosition = Vector2.zero;
            PanelRoot = rect;

            var bg = new GameObject("Background").AddComponent<Image>();
            bg.transform.SetParent(rect, false);
            bg.rectTransform.anchorMin = Vector2.zero;
            bg.rectTransform.anchorMax = Vector2.one;
            bg.rectTransform.offsetMin = Vector2.zero;
            bg.rectTransform.offsetMax = Vector2.zero;
            ManuscriptUITheme.ApplyToPanel(bg);
            PanelBackground = bg;

            var border = new GameObject("Border").AddComponent<Image>();
            border.transform.SetParent(rect, false);
            border.rectTransform.anchorMin = Vector2.zero;
            border.rectTransform.anchorMax = Vector2.one;
            border.rectTransform.offsetMin = new Vector2(-4, -4);
            border.rectTransform.offsetMax = new Vector2(4, 4);
            ManuscriptUITheme.ApplyToBorder(border);
            BorderImage = border;

            var textGo = new GameObject("DialogueText");
            textGo.transform.SetParent(rect, false);
            var text = textGo.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 18;
            text.alignment = TextAnchor.UpperLeft;
            text.supportRichText = true;
            var textRect = text.rectTransform;
            textRect.anchorMin = new Vector2(0.05f, 0.15f);
            textRect.anchorMax = new Vector2(0.95f, 0.9f);
            textRect.offsetMin = new Vector2(20, 20);
            textRect.offsetMax = new Vector2(-20, -20);
            ManuscriptUITheme.ApplyToText(text);
            DialogueText = text;

            var speakerGo = new GameObject("SpeakerText");
            speakerGo.transform.SetParent(rect, false);
            var speaker = speakerGo.AddComponent<Text>();
            speaker.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            speaker.fontSize = 14;
            speaker.alignment = TextAnchor.UpperLeft;
            var speakerRect = speaker.rectTransform;
            speakerRect.anchorMin = new Vector2(0.05f, 0.92f);
            speakerRect.anchorMax = new Vector2(0.5f, 1f);
            speakerRect.offsetMin = new Vector2(20, 0);
            speakerRect.offsetMax = new Vector2(-20, -5);
            ManuscriptUITheme.ApplyToText(speaker, lapis: true);
            SpeakerText = speaker;

            var btnGo = new GameObject("ContinueButton");
            btnGo.transform.SetParent(rect, false);
            var btnImgForRect = btnGo.AddComponent<Image>();
            var btn = btnGo.AddComponent<Button>();
            var btnRect = btnGo.GetComponent<RectTransform>();
            btnRect.anchorMin = new Vector2(0.8f, 0.02f);
            btnRect.anchorMax = new Vector2(0.98f, 0.12f);
            btnRect.offsetMin = Vector2.zero;
            btnRect.offsetMax = Vector2.zero;
            btnImgForRect.color = ManuscriptUITheme.ParchmentAged;
            var btnTextGo = new GameObject("Text");
            btnTextGo.transform.SetParent(btnGo.transform, false);
            var btnText = btnTextGo.AddComponent<Text>();
            btnText.text = "Continue";
            btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            btnText.fontSize = 14;
            btnText.alignment = TextAnchor.MiddleCenter;
            var btnTextRect = btnText.rectTransform;
            btnTextRect.anchorMin = Vector2.zero;
            btnTextRect.anchorMax = Vector2.one;
            btnTextRect.offsetMin = Vector2.zero;
            btnTextRect.offsetMax = Vector2.zero;
            ManuscriptUITheme.ApplyToButton(btn);
            ContinueButton = btn;
        }

        public void ShowStoryBeat(StoryBeat beat, Action onComplete = null)
        {
            if (beat == null)
            {
                onComplete?.Invoke();
                return;
            }
            _onComplete = onComplete;
            if (_displayCoroutine != null)
                StopCoroutine(_displayCoroutine);

            if (PanelRoot == null) EnsurePanel();
            EnsureStoryBackdropLayer();
            if (StoryBackdropImage != null)
            {
                if (beat.StoryBackdrop != null)
                {
                    StoryBackdropImage.texture = beat.StoryBackdrop;
                    StoryBackdropImage.gameObject.SetActive(true);
                }
                else
                    StoryBackdropImage.gameObject.SetActive(false);
            }
            PanelRoot.gameObject.SetActive(true);

            if (DialogueText != null) DialogueText.text = beat.Text ?? "";
            if (SpeakerText != null) SpeakerText.text = beat.SpeakerName ?? "";
            if (PortraitImage != null)
            {
                PortraitImage.gameObject.SetActive(beat.Portrait != null);
                if (beat.Portrait != null) PortraitImage.sprite = beat.Portrait;
            }
            if (ContinueButton != null)
            {
                ContinueButton.onClick.RemoveAllListeners();
                ContinueButton.onClick.AddListener(OnContinueClicked);
                ContinueButton.gameObject.SetActive(beat.WaitForInput);
            }

            if (beat.WaitForInput)
                return;

            _displayCoroutine = StartCoroutine(AutoCloseAfter(beat.DisplayDuration));
        }

        IEnumerator AutoCloseAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            _displayCoroutine = null;
            Complete();
        }

        void OnContinueClicked()
        {
            if (_displayCoroutine != null)
            {
                StopCoroutine(_displayCoroutine);
                _displayCoroutine = null;
            }
            Complete();
        }

        void Complete()
        {
            if (StoryBackdropImage != null)
                StoryBackdropImage.gameObject.SetActive(false);
            if (PanelRoot != null)
                PanelRoot.gameObject.SetActive(false);
            var cb = _onComplete;
            _onComplete = null;
            cb?.Invoke();
        }

        public void Hide()
        {
            if (_displayCoroutine != null)
            {
                StopCoroutine(_displayCoroutine);
                _displayCoroutine = null;
            }
            if (StoryBackdropImage != null)
                StoryBackdropImage.gameObject.SetActive(false);
            if (PanelRoot != null)
                PanelRoot.gameObject.SetActive(false);
            _onComplete = null;
        }

        /// <summary>Show a single line (e.g. boss bark or phase line). Auto-closes after displayDuration. For manuscript-style boss dialogue.</summary>
        public void ShowLine(string text, string speakerName, float displayDuration = 3f)
        {
            if (string.IsNullOrEmpty(text)) return;
            _onComplete = null;
            if (_displayCoroutine != null) StopCoroutine(_displayCoroutine);
            if (PanelRoot == null) EnsurePanel();
            PanelRoot.gameObject.SetActive(true);
            if (DialogueText != null) DialogueText.text = text ?? "";
            if (SpeakerText != null) SpeakerText.text = speakerName ?? "";
            if (PortraitImage != null) PortraitImage.gameObject.SetActive(false);
            if (ContinueButton != null) ContinueButton.gameObject.SetActive(false);
            _displayCoroutine = StartCoroutine(AutoCloseAfter(displayDuration));
        }

        /// <summary>Show dialogue with choices. onChoiceSelected(index, isCorrectChoice, transitionToLocationId, associatedSin).</summary>
        public void ShowChoiceBeat(DialogueChoiceBeat beat, Action<int, bool, string, SinType> onChoiceSelected)
        {
            if (beat == null || beat.Choices == null || beat.Choices.Length == 0)
            {
                onChoiceSelected?.Invoke(-1, false, null, SinType.None);
                return;
            }
            _onComplete = null;
            if (_displayCoroutine != null) StopCoroutine(_displayCoroutine);
            _displayCoroutine = null;

            if (PanelRoot == null) EnsurePanel();
            PanelRoot.gameObject.SetActive(true);

            if (DialogueText != null) DialogueText.text = beat.Text ?? "";
            if (SpeakerText != null) SpeakerText.text = beat.SpeakerName ?? "";
            if (PortraitImage != null)
            {
                PortraitImage.gameObject.SetActive(beat.Portrait != null);
                if (beat.Portrait != null) PortraitImage.sprite = beat.Portrait;
            }
            if (ContinueButton != null) ContinueButton.gameObject.SetActive(false);

            EnsureChoiceButtonsRoot();
            _choiceButtonsRoot.gameObject.SetActive(true);
            foreach (Transform c in _choiceButtonsRoot) c.gameObject.SetActive(false);

            for (int i = 0; i < beat.Choices.Length; i++)
            {
                var choice = beat.Choices[i];
                var btn = GetOrCreateChoiceButton(i);
                btn.gameObject.SetActive(true);
                var textComp = btn.GetComponentInChildren<Text>();
                if (textComp != null) textComp.text = choice.Text ?? $"Choice {i + 1}";
                btn.onClick.RemoveAllListeners();
                int idx = i;
                bool correct = choice.IsCorrectChoice;
                string transitionTo = choice.TransitionToLocationId;
                var sin = choice.AssociatedSin;
                btn.onClick.AddListener(() =>
                {
                    _choiceButtonsRoot.gameObject.SetActive(false);
                    if (PanelRoot != null) PanelRoot.gameObject.SetActive(false);
                    onChoiceSelected?.Invoke(idx, correct, transitionTo, sin);
                });
            }
        }

        RectTransform _choiceButtonsRoot;

        void EnsureChoiceButtonsRoot()
        {
            if (_choiceButtonsRoot != null) return;
            var go = new GameObject("ChoiceButtons");
            go.transform.SetParent(PanelRoot, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.05f, 0.02f);
            rect.anchorMax = new Vector2(0.95f, 0.25f);
            rect.offsetMin = new Vector2(20, 10);
            rect.offsetMax = new Vector2(-20, -10);
            var vlg = go.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 8;
            vlg.childControlHeight = true;
            vlg.childForceExpandHeight = false;
            _choiceButtonsRoot = rect;
        }

        Button GetOrCreateChoiceButton(int index)
        {
            var existing = _choiceButtonsRoot.childCount > index ? _choiceButtonsRoot.GetChild(index) : null;
            if (existing != null)
            {
                var btn = existing.GetComponent<Button>();
                if (btn != null) return btn;
            }
            var go = new GameObject("Choice_" + index);
            go.transform.SetParent(_choiceButtonsRoot, false);
            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0, 36);
            var btn2 = go.AddComponent<Button>();
            var img = go.AddComponent<Image>();
            img.color = ManuscriptUITheme.ParchmentAged;
            var textGo = new GameObject("Text");
            textGo.transform.SetParent(go.transform, false);
            var text = textGo.AddComponent<Text>();
            text.text = "Choice";
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 14;
            text.alignment = TextAnchor.MiddleLeft;
            var textRect = text.rectTransform;
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(15, 2);
            textRect.offsetMax = new Vector2(-15, -2);
            ManuscriptUITheme.ApplyToButton(btn2);
            return btn2;
        }
    }
}
