using UnityEngine;

namespace KyndeBlade
{
    /// <summary>Listens for the Wode-Wo passcode (W O D E keys in sequence). On entry: unlocks the Wode-Wo line, shows "a wode-wo has awakened", and starts the arc.</summary>
    public class SecretCodeListener : MonoBehaviour
    {
        static readonly KeyCode[] WodeWoCode = { KeyCode.W, KeyCode.O, KeyCode.D, KeyCode.E };
        const float TimeoutSeconds = 3f;

        int _index;
        float _lastKeyTime;

        void Update()
        {
            var saveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            if (saveManager != null && saveManager.IsWodeWoUnlocked) return;

            if (Input.anyKeyDown)
            {
                if (Time.unscaledTime - _lastKeyTime > TimeoutSeconds)
                    _index = 0;

                if (Input.GetKeyDown(WodeWoCode[_index]))
                {
                    _index++;
                    _lastKeyTime = Time.unscaledTime;
                    if (_index >= WodeWoCode.Length)
                    {
                        OnPasscodeEntered();
                        _index = 0;
                    }
                }
                else
                    _index = 0;
            }
        }

        void OnPasscodeEntered()
        {
            var saveManager = UnityEngine.Object.FindFirstObjectByType<SaveManager>();
            if (saveManager == null) return;

            saveManager.SetWodeWoUnlocked(true);
            saveManager.SetWodeWoArcStage(1); // start the line (Baby)

            // Show "a wode-wo has awakened" — try asset first, else runtime beat
            var nm = UnityEngine.Object.FindFirstObjectByType<NarrativeManager>();
            var beat = nm != null ? Resources.Load<StoryBeat>("Data/Vision1/WodeWoAwakened") : null;
            if (beat == null && nm != null)
            {
                beat = ScriptableObject.CreateInstance<StoryBeat>();
                beat.Text = "A wode-wo has awakened.";
                beat.BeatId = "wode_wo_awakened";
                beat.WaitForInput = true;
            }
            if (nm != null && beat != null)
                nm.ShowStoryBeat(beat, null);
        }
    }
}
