using UnityEngine;

namespace KyndeBlade.Combat
{
    /// <summary>Maps KyndeElementType to StatusEffect application with probability-based proc.</summary>
    public static class StatusEffectPipeline
    {
        public static void TryApplyElement(KyndeElementType element, MedievalCharacter target)
        {
            if (target == null || element == KyndeElementType.None) return;

            switch (element)
            {
                case KyndeElementType.Flamme:
                    if (Random.value < 0.30f)
                        target.ApplyStatusEffect(StatusEffect.CreateBurningEffect(3f, 3f));
                    break;

                case KyndeElementType.Frost:
                    if (Random.value < 0.30f)
                        target.ApplyStatusEffect(StatusEffect.CreateFrostEffect(3f));
                    break;

                case KyndeElementType.Thunder:
                    if (Random.value < 0.20f)
                        target.ApplyStatusEffect(StatusEffect.CreateStunEffect(1f));
                    break;

                case KyndeElementType.Fals:
                    if (Random.value < 0.25f)
                        target.ApplyStatusEffect(StatusEffect.CreateWeakEffect(2f));
                    break;

                case KyndeElementType.Trewthe:
                    if (Random.value < 0.25f)
                    {
                        var tm = Object.FindFirstObjectByType<TurnManager>();
                        bool isEnemy = tm != null && tm.EnemyCharacters != null && tm.EnemyCharacters.Contains(target);
                        if (isEnemy)
                            target.ApplyStatusEffect(StatusEffect.CreateVulnerableEffect(2f));
                        else
                            target.ApplyStatusEffect(StatusEffect.CreateBlessedEffect(2f));
                    }
                    break;

                case KyndeElementType.Kynde:
                    if (Random.value < 0.40f)
                        target.ApplyStatusEffect(StatusEffect.CreateKyndeBoostEffect(2f, 2f));
                    break;
            }
        }
    }
}
