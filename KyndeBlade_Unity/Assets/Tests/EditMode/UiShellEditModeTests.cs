using NUnit.Framework;
using UnityEngine;
using KyndeBlade;

namespace KyndeBlade.Tests
{
    /// <summary>EditMode checks for menu/settings save keys and SaveManager.HasSavedGame.</summary>
    public class UiShellEditModeTests
    {
        [Test]
        public void KyndeBladeSettingsStore_MasterVolume_Persists()
        {
            float backup = KyndeBladeSettingsStore.MasterVolume;
            try
            {
                KyndeBladeSettingsStore.MasterVolume = 0.37f;
                Assert.AreEqual(0.37f, PlayerPrefs.GetFloat(KyndeBladeSettingsStore.PrefMasterVolume), 0.02f);
            }
            finally
            {
                KyndeBladeSettingsStore.MasterVolume = backup;
            }
        }

        [Test]
        public void SaveManager_HasSavedGame_TrueWhenPrefsContainSave()
        {
            const string k = "KyndeBlade_Save";
            var hadKey = PlayerPrefs.HasKey(k);
            var prev = hadKey ? PlayerPrefs.GetString(k) : null;
            var json = GameProgress.CreateNew(GameWorldConstants.DefaultStartLocationId).ToJson();
            PlayerPrefs.SetString(k, json);
            PlayerPrefs.Save();
            var go = new GameObject("SaveMgrTest");
            var sm = go.AddComponent<SaveManager>();
            Assert.IsTrue(sm.HasSavedGame);
            Object.DestroyImmediate(go);
            if (hadKey) PlayerPrefs.SetString(k, prev);
            else PlayerPrefs.DeleteKey(k);
            PlayerPrefs.Save();
        }
    }
}
