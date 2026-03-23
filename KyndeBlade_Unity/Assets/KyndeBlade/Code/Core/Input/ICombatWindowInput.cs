namespace KyndeBlade
{
    /// <summary>
    /// Combat dodge / parry / counter presses during real-time windows.
    /// Default implementation is keyboard+gamepad inside ParryDodgeInputHandler (KyndeBlade.Combat).
    /// Tests can assign an instance to ParryDodgeInputHandler.InputSourceOverride.
    /// Future: wire to a Unity Input Actions asset behind a thin adapter implementing this interface.
    /// </summary>
    public interface ICombatWindowInput
    {
        bool WasDodgePressedThisFrame();
        bool WasParryPressedThisFrame();
        bool WasCounterPressedThisFrame();
    }
}
