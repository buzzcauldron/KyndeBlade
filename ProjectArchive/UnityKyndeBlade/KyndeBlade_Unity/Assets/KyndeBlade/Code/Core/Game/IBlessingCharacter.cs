namespace KyndeBlade
{
    /// <summary>
    /// Core-side contract for blessing application without depending on combat assembly types.
    /// </summary>
    public interface IBlessingCharacter
    {
        string GetBlessingCharacterName();
        bool RemoveHungerStack();

        float GetBlessingMaxHealth();
        void SetBlessingMaxHealth(float value);
        float GetBlessingMaxStamina();
        void SetBlessingMaxStamina(float value);
        float GetBlessingAttackPower();
        void SetBlessingAttackPower(float value);
        float GetBlessingDefense();
        void SetBlessingDefense(float value);
        float GetBlessingSpeed();
        void SetBlessingSpeed(float value);
        float GetBlessingMaxKynde();
        void SetBlessingMaxKynde(float value);
        float GetBlessingCurrentHealth();
        void SetBlessingCurrentHealth(float value);
        float GetBlessingCurrentStamina();
        void SetBlessingCurrentStamina(float value);
        int GetBlessingCount();
        void SetBlessingCount(int value);
        System.Collections.Generic.List<ActiveBlessing> GetActiveBlessings();
    }
}
