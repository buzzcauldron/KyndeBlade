using UnityEngine;

namespace KyndeBlade
{
    /// <summary>State that persists across the entire game installation. Not cleared by NewGame.
    /// Used for Wode-Wo's death—once he dies, he stays dead forever for this install.</summary>
    public static class InstallState
    {
        const string WodeWoDeadKey = "KyndeBlade_Install_WodeWo_IsDead";

        /// <summary>Wode-Wo has died. Permanent to this install. Never resets. NewGame does not clear this.</summary>
        public static bool WodeWoIsDead
        {
            get => PlayerPrefs.GetInt(WodeWoDeadKey, 0) == 1;
            set
            {
                PlayerPrefs.SetInt(WodeWoDeadKey, value ? 1 : 0);
                PlayerPrefs.Save();
            }
        }

        /// <summary>Mark Wode-Wo as dead. Permanent. Call when death cutscene plays.</summary>
        public static void MarkWodeWoDead()
        {
            WodeWoIsDead = true;
        }

        /// <summary>Reset for testing only. Does NOT clear WodeWoIsDead by default.</summary>
        public static void ResetForTesting(bool includeWodeWo = false)
        {
            if (includeWodeWo)
                WodeWoIsDead = false;
        }
    }
}
