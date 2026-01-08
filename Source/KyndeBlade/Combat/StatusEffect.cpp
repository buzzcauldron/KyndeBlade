#include "Combat/StatusEffect.h"
#include "Characters/MedievalCharacter.h"

UStatusEffect::UStatusEffect()
{
	EffectData = FStatusEffectData();
}

void UStatusEffect::UpdateEffect(float DeltaTime)
{
	if (EffectData.Duration > 0.0f)
	{
		EffectData.RemainingTime -= DeltaTime;
		if (EffectData.RemainingTime <= 0.0f)
		{
			EffectData.RemainingTime = 0.0f;
		}
	}
}

bool UStatusEffect::IsExpired() const
{
	if (EffectData.Duration <= 0.0f)
	{
		return false; // Permanent effect
	}
	return EffectData.RemainingTime <= 0.0f;
}

void UStatusEffect::ApplyEffect(AMedievalCharacter* Target)
{
	if (!Target)
	{
		return;
	}

	ApplyStatModifiers(Target, true);
}

void UStatusEffect::RemoveEffect(AMedievalCharacter* Target)
{
	if (!Target)
	{
		return;
	}

	ApplyStatModifiers(Target, false);
}

void UStatusEffect::StackEffect(int32 AdditionalStacks)
{
	EffectData.StackCount += AdditionalStacks;
	
	// For Hunger, stacking makes it worse
	if (EffectData.EffectType == EStatusEffectType::Hunger)
	{
		// Each stack increases severity
		float StackMultiplier = 1.0f + (EffectData.StackCount - 1) * 0.2f; // 20% worse per stack
		EffectData.AttackPowerModifier = FMath::Max(0.1f, 1.0f - (EffectData.StackCount * 0.15f)); // -15% per stack
		EffectData.DefenseModifier = FMath::Max(0.1f, 1.0f - (EffectData.StackCount * 0.15f));
		EffectData.SpeedModifier = FMath::Max(0.1f, 1.0f - (EffectData.StackCount * 0.15f));
		EffectData.StaminaRegenModifier = FMath::Max(0.0f, 1.0f - (EffectData.StackCount * 0.25f)); // -25% stamina regen per stack
		EffectData.DamagePerSecond = 2.0f * EffectData.StackCount; // 2 DPS per stack
		EffectData.KyndeGenerationModifier = FMath::Max(0.0f, 1.0f - (EffectData.StackCount * 0.3f)); // -30% Kynde generation per stack
	}
}

void UStatusEffect::ApplyStatModifiers(AMedievalCharacter* Target, bool bApply)
{
	if (!Target)
	{
		return;
	}

	// Apply or remove stat modifiers
	// Note: In a full implementation, characters would track base stats separately
	// For now, we apply multipliers directly (effects should be applied in order)
	if (bApply)
	{
		// Apply modifiers
		Target->Stats.AttackPower *= EffectData.AttackPowerModifier;
		Target->Stats.Defense *= EffectData.DefenseModifier;
		Target->Stats.Speed *= EffectData.SpeedModifier;
	}
	else
	{
		// Remove modifiers (divide by the modifier to reverse)
		if (EffectData.AttackPowerModifier > 0.0f)
		{
			Target->Stats.AttackPower /= EffectData.AttackPowerModifier;
		}
		if (EffectData.DefenseModifier > 0.0f)
		{
			Target->Stats.Defense /= EffectData.DefenseModifier;
		}
		if (EffectData.SpeedModifier > 0.0f)
		{
			Target->Stats.Speed /= EffectData.SpeedModifier;
		}
	}
}

void UStatusEffect::ApplyDamageOverTime(AMedievalCharacter* Target, float DeltaTime)
{
	if (!Target || EffectData.DamagePerSecond <= 0.0f)
	{
		return;
	}

	float Damage = EffectData.DamagePerSecond * DeltaTime;
	Target->TakeDamage(Damage, nullptr);
}

// Static factory methods
UStatusEffect* UStatusEffect::CreateHungerEffect(float Duration, int32 Stacks)
{
	UStatusEffect* Effect = NewObject<UStatusEffect>();
	if (Effect)
	{
		Effect->EffectData.EffectType = EStatusEffectType::Hunger;
		Effect->EffectData.Duration = Duration; // 0 = permanent until removed
		Effect->EffectData.RemainingTime = Duration;
		Effect->EffectData.StackCount = Stacks;
		Effect->EffectData.EffectName = TEXT("Hunger");
		
		// Hunger is the WORST status effect - severe penalties
		Effect->EffectData.AttackPowerModifier = FMath::Max(0.1f, 1.0f - (Stacks * 0.15f)); // -15% attack per stack (min 10%)
		Effect->EffectData.DefenseModifier = FMath::Max(0.1f, 1.0f - (Stacks * 0.15f)); // -15% defense per stack (min 10%)
		Effect->EffectData.SpeedModifier = FMath::Max(0.1f, 1.0f - (Stacks * 0.15f)); // -15% speed per stack (min 10%)
		Effect->EffectData.StaminaRegenModifier = FMath::Max(0.0f, 1.0f - (Stacks * 0.25f)); // -25% stamina regen per stack
		Effect->EffectData.DamagePerSecond = 2.0f * Stacks; // 2 DPS per stack
		Effect->EffectData.KyndeGenerationModifier = FMath::Max(0.0f, 1.0f - (Stacks * 0.3f)); // -30% Kynde generation per stack
		Effect->EffectData.StatModifier = FMath::Max(0.1f, 1.0f - (Stacks * 0.2f)); // Overall -20% stats per stack
	}
	return Effect;
}

UStatusEffect* UStatusEffect::CreateFrostEffect(float Duration, float SlowAmount)
{
	UStatusEffect* Effect = NewObject<UStatusEffect>();
	if (Effect)
	{
		Effect->EffectData.EffectType = EStatusEffectType::Frost;
		Effect->EffectData.Duration = Duration;
		Effect->EffectData.RemainingTime = Duration;
		Effect->EffectData.SpeedModifier = SlowAmount;
		Effect->EffectData.EffectName = TEXT("Frost");
	}
	return Effect;
}

UStatusEffect* UStatusEffect::CreateBurningEffect(float Duration, float DamagePerSecond)
{
	UStatusEffect* Effect = NewObject<UStatusEffect>();
	if (Effect)
	{
		Effect->EffectData.EffectType = EStatusEffectType::Burning;
		Effect->EffectData.Duration = Duration;
		Effect->EffectData.RemainingTime = Duration;
		Effect->EffectData.DamagePerSecond = DamagePerSecond;
		Effect->EffectData.EffectName = TEXT("Burning");
	}
	return Effect;
}

UStatusEffect* UStatusEffect::CreatePoisonEffect(float Duration, float DamagePerSecond)
{
	UStatusEffect* Effect = NewObject<UStatusEffect>();
	if (Effect)
	{
		Effect->EffectData.EffectType = EStatusEffectType::Poison;
		Effect->EffectData.Duration = Duration;
		Effect->EffectData.RemainingTime = Duration;
		Effect->EffectData.DamagePerSecond = DamagePerSecond;
		Effect->EffectData.EffectName = TEXT("Poison");
	}
	return Effect;
}

UStatusEffect* UStatusEffect::CreateStunEffect(float Duration)
{
	UStatusEffect* Effect = NewObject<UStatusEffect>();
	if (Effect)
	{
		Effect->EffectData.EffectType = EStatusEffectType::Stun;
		Effect->EffectData.Duration = Duration;
		Effect->EffectData.RemainingTime = Duration;
		Effect->EffectData.SpeedModifier = 0.0f; // Cannot act
		Effect->EffectData.EffectName = TEXT("Stun");
	}
	return Effect;
}
