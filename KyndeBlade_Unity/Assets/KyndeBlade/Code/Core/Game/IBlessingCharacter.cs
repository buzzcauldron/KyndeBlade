namespace KyndeBlade
{
    /// <summary>
    /// Core-side contract for blessing application without depending on combat assembly types.
    /// </summary>
    public interface IBlessingCharacter
    {
        CharacterStats GetBlessingStats();
        string GetBlessingCharacterName();
        bool RemoveHungerStack();
    }
}
