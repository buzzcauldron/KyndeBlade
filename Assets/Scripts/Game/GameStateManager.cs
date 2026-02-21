using System;
using UnityEngine;
using UnityEngine.UI;

namespace KyndeBlade
{
    /// <summary>Goals, rewards, victory/defeat (Hodent: Motivation; Beginner: Goals and Rewards).</summary>
    public class GameStateManager : MonoBehaviour
    {
        [Header("References")]
        public TurnManager TurnManager;
        public KyndeBladeGameManager GameManager;
        public IlluminationManager IlluminationManager;

        [Header("UI")]
        public GameObject VictoryPanel;
        public GameObject DefeatPanel;
        public Text VictoryText;
        public Text DefeatText;

        [Header("Rewards")]
        public int BaseXPPerEnemy = 50;
        public int BonusXPForVictory = 100;

        public event Action OnVictory;
        public event Action OnDefeat;

        void Start()
        {
            if (TurnManager == null) TurnManager = FindObjectOfType<TurnManager>();
            if (IlluminationManager == null) IlluminationManager = FindObjectOfType<IlluminationManager>();
            if (TurnManager != null)
                TurnManager.OnCombatEnded += OnCombatEnded;

            if (VictoryPanel != null) VictoryPanel.SetActive(false);
            if (DefeatPanel != null) DefeatPanel.SetActive(false);
        }

        void OnDestroy()
        {
            if (TurnManager != null) TurnManager.OnCombatEnded -= OnCombatEnded;
        }

        void OnCombatEnded()
        {
            if (TurnManager == null) return;

            bool victory = AreAllDefeated(TurnManager.EnemyCharacters);
            if (victory)
            {
                Victory();
            }
            else
            {
                Defeat();
            }
        }

        bool AreAllDefeated(System.Collections.Generic.List<MedievalCharacter> list)
        {
            foreach (var c in list)
                if (c != null && c.IsAlive()) return false;
            return true;
        }

        void Victory()
        {
            int xp = CalculateVictoryXP();
            if (IlluminationManager != null)
                StartCoroutine(ShowVictoryAfterIllumination(xp));
            else
                ShowVictoryPanel(xp);
        }

        void Defeat()
        {
            if (IlluminationManager != null)
                StartCoroutine(ShowDefeatAfterIllumination());
            else
                ShowDefeatPanel();
        }

        System.Collections.IEnumerator ShowVictoryAfterIllumination(int xp)
        {
            IlluminationManager.TriggerVictoryIllumination();
            yield return new WaitForSeconds(2.6f);
            ShowVictoryPanel(xp);
        }

        System.Collections.IEnumerator ShowDefeatAfterIllumination()
        {
            IlluminationManager.TriggerDefeatIllumination();
            yield return new WaitForSeconds(2.6f);
            ShowDefeatPanel();
        }

        void ShowVictoryPanel(int xp)
        {
            if (VictoryPanel != null) VictoryPanel.SetActive(true);
            if (VictoryText != null) VictoryText.text = $"Victory! +{xp} XP";
            OnVictory?.Invoke();
        }

        void ShowDefeatPanel()
        {
            if (DefeatPanel != null) DefeatPanel.SetActive(true);
            if (DefeatText != null) DefeatText.text = "Defeat...";
            OnDefeat?.Invoke();
        }

        int CalculateVictoryXP()
        {
            int xp = BonusXPForVictory;
            if (TurnManager != null)
            {
                foreach (var e in TurnManager.EnemyCharacters)
                    if (e != null && !e.IsAlive()) xp += BaseXPPerEnemy;
            }
            return xp;
        }
    }
}
