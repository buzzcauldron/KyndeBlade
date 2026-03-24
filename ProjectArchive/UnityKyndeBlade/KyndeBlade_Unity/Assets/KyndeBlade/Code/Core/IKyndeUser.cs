namespace KyndeBlade
{
    /// <summary>Any character (player or boss) that uses the Kynde resource must implement this.
    /// Decouples Kynde logic from a single character script and allows designers to plug in Kynde behaviour.</summary>
    public interface IKyndeUser
    {
        void GainKynde(float amount);
        bool ConsumeKynde(float amount);
        float GetCurrentKynde();
        float GetMaxKynde();
    }
}
