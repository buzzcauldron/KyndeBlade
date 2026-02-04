#include "MedievalCharacter.h"
#include "Components/CapsuleComponent.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "../Combat/CombatAction.h"
#include "../Combat/StatusEffect.h"

AMedievalCharacter::AMedievalCharacter(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	PrimaryActorTick.bCanEverTick = true;

	CharacterClass = ECharacterClass::Knight;
	Stats = FCharacterStats();
	CharacterName = TEXT("Unknown");
	bIsDodging = false;
	bIsParrying = false;
}

void AMedievalCharacter::BeginPlay()
{
	Super::BeginPlay();
	
	// Initialize stats based on class
	switch (CharacterClass)
	{
	case ECharacterClass::Knight:
		Stats.MaxHealth = 150.0f;
		Stats.CurrentHealth = 150.0f;
		Stats.MaxStamina = 120.0f;
		Stats.CurrentStamina = 120.0f;
		Stats.AttackPower = 15.0f;
		Stats.Defense = 10.0f;
		Stats.Speed = 8.0f;
		break;
	case ECharacterClass::Mage:
		Stats.MaxHealth = 80.0f;
		Stats.CurrentHealth = 80.0f;
		Stats.MaxStamina = 150.0f;
		Stats.CurrentStamina = 150.0f;
		Stats.AttackPower = 20.0f;
		Stats.Defense = 3.0f;
		Stats.Speed = 10.0f;
		break;
	case ECharacterClass::Archer:
		Stats.MaxHealth = 100.0f;
		Stats.CurrentHealth = 100.0f;
		Stats.MaxStamina = 130.0f;
		Stats.CurrentStamina = 130.0f;
		Stats.AttackPower = 12.0f;
		Stats.Defense = 5.0f;
		Stats.Speed = 12.0f;
		break;
	case ECharacterClass::Rogue:
		Stats.MaxHealth = 90.0f;
		Stats.CurrentHealth = 90.0f;
		Stats.MaxStamina = 140.0f;
		Stats.CurrentStamina = 140.0f;
		Stats.AttackPower = 14.0f;
		Stats.Defense = 4.0f;
		Stats.Speed = 15.0f;
		break;
	}
}

void AMedievalCharacter::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	UpdateRealTimeCombat(DeltaTime);
	UpdateActionCooldowns(DeltaTime);
	
	// Expedition 33-inspired: Update broken state
	if (Stats.bIsBroken && Stats.BrokenStunRemaining > 0.0f)
	{
		Stats.BrokenStunRemaining -= DeltaTime;
		if (Stats.BrokenStunRemaining <= 0.0f)
		{
			RecoverFromBreak();
		}
	}

	// Update status effects (Hunger is the worst)
	UpdateStatusEffects(DeltaTime);
}

void AMedievalCharacter::ApplyCustomDamage(float Damage, AMedievalCharacter* Attacker)
{
	if (!IsAlive())
	{
		return;
	}

	// Apply defense reduction
	float ActualDamage = FMath::Max(0.0f, Damage - Stats.Defense);

	if (bIsGuarding)
	{
		ActualDamage *= (1.0f - GuardDamageReduction);
		SetGuarding(false);
	}

	// Hunger makes you take MORE damage (the worst status effect)
	if (IsHungry())
	{
		UStatusEffect* HungerEffect = GetStatusEffect(EStatusEffectType::Hunger);
		if (HungerEffect)
		{
			// Hunger reduces defense effectiveness - take 20% more damage per stack
			float HungerVulnerability = 1.0f + (HungerEffect->EffectData.StackCount * 0.2f);
			ActualDamage *= HungerVulnerability;
		}
	}
	
	// Expedition 33-inspired: Check for Escapade/Ward with Kynde rewards (Piers Plowman)
	if (bIsDodging && AttemptDodge())
	{
		// Successful Escapade (Kynde's Evasion) - no damage, gain Kynde
		// Perfect Escapade is in last 30% of window
		float PerfectWindow = DodgeWindowRemaining * 0.3f;
		bool bPerfectDodge = (DodgeWindowRemaining <= PerfectWindow);
		float KyndeReward = bPerfectDodge ? 2.0f : 1.0f; // Perfect Escapade gives more Kynde
		GainKynde(KyndeReward);
		return;
	}

	if (bIsParrying && AttemptParry())
	{
		// Successful Ward (Trewthe's Sheeld) - reduced damage, gain Kynde, counter opportunity
		// Perfect Ward is in last 25% of window
		float PerfectWindow = ParryWindowRemaining * 0.25f;
		bool bPerfectParry = (ParryWindowRemaining <= PerfectWindow);
		float KyndeReward = bPerfectParry ? 4.0f : 2.0f; // Perfect Ward gives more Kynde
		GainKynde(KyndeReward);
		ActualDamage *= 0.3f;
		// Could trigger counter attack here
	}

	Stats.CurrentHealth = FMath::Max(0.0f, Stats.CurrentHealth - ActualDamage);
	OnHealthChanged.Broadcast(Stats.CurrentHealth, Stats.MaxHealth);

	CheckDefeated();

	// Grant XP to attacker when this character is defeated
	if (!IsAlive() && Attacker)
	{
		Attacker->AddExperience(25);
	}
}

void AMedievalCharacter::Heal(float Amount)
{
	Stats.CurrentHealth = FMath::Min(Stats.MaxHealth, Stats.CurrentHealth + Amount);
	OnHealthChanged.Broadcast(Stats.CurrentHealth, Stats.MaxHealth);
}

void AMedievalCharacter::ConsumeStamina(float Amount)
{
	Stats.CurrentStamina = FMath::Max(0.0f, Stats.CurrentStamina - Amount);
	OnStaminaChanged.Broadcast(Stats.CurrentStamina, Stats.MaxStamina);
}

void AMedievalCharacter::RestoreStamina(float Amount)
{
	// Hunger reduces stamina restoration (the worst status effect)
	if (IsHungry())
	{
		UStatusEffect* HungerEffect = GetStatusEffect(EStatusEffectType::Hunger);
		if (HungerEffect)
		{
			Amount *= HungerEffect->EffectData.StaminaRegenModifier;
		}
	}
	
	Stats.CurrentStamina = FMath::Min(Stats.MaxStamina, Stats.CurrentStamina + Amount);
	OnStaminaChanged.Broadcast(Stats.CurrentStamina, Stats.MaxStamina);
}

bool AMedievalCharacter::ConsumeMana(float Amount)
{
	if (Stats.CurrentMana < Amount)
	{
		return false;
	}
	Stats.CurrentMana -= Amount;
	return true;
}

void AMedievalCharacter::RestoreMana(float Amount)
{
	Stats.CurrentMana = FMath::Min(Stats.MaxMana, Stats.CurrentMana + Amount);
}

// Expedition 33-inspired: Kynde management
void AMedievalCharacter::GainKynde(float Amount)
{
	// Hunger reduces Kynde generation (the worst status effect)
	if (IsHungry())
	{
		UStatusEffect* HungerEffect = GetStatusEffect(EStatusEffectType::Hunger);
		if (HungerEffect)
		{
			Amount *= HungerEffect->EffectData.KyndeGenerationModifier;
		}
	}
	
	Stats.CurrentKynde = FMath::Min(Stats.MaxKynde, Stats.CurrentKynde + Amount);
	OnKyndeChanged.Broadcast(Stats.CurrentKynde, Stats.MaxKynde);
}

bool AMedievalCharacter::ConsumeKynde(float Amount)
{
	if (Stats.CurrentKynde >= Amount)
	{
		Stats.CurrentKynde -= Amount;
		OnKyndeChanged.Broadcast(Stats.CurrentKynde, Stats.MaxKynde);
		return true;
	}
	return false;
}

// Expedition 33-inspired: Break system
void AMedievalCharacter::TakeBreakDamage(float BreakAmount)
{
	if (Stats.bIsBroken)
	{
		return; // Already broken, break gauge doesn't decrease further
	}

	Stats.CurrentBreakGauge = FMath::Max(0.0f, Stats.CurrentBreakGauge - BreakAmount);
	OnBreakGaugeChanged.Broadcast(Stats.CurrentBreakGauge, Stats.MaxBreakGauge);

	if (Stats.CurrentBreakGauge <= 0.0f)
	{
		BreakCharacter();
	}
}

void AMedievalCharacter::BreakCharacter()
{
	if (Stats.bIsBroken)
	{
		return; // Already broken
	}

	Stats.bIsBroken = true;
	Stats.BrokenStunRemaining = 2.0f; // Stunned for 2 turns (will be decremented in Tick)
	OnCharacterBroken.Broadcast(this);
}

void AMedievalCharacter::RecoverFromBreak()
{
	Stats.bIsBroken = false;
	Stats.BrokenStunRemaining = 0.0f;
	Stats.CurrentBreakGauge = Stats.MaxBreakGauge; // Reset break gauge
	OnBreakGaugeChanged.Broadcast(Stats.CurrentBreakGauge, Stats.MaxBreakGauge);
}

void AMedievalCharacter::StartDodge(float WindowDuration)
{
	bIsDodging = true;
	DodgeWindowRemaining = WindowDuration;
}

void AMedievalCharacter::StartParry(float WindowDuration)
{
	bIsParrying = true;
	ParryWindowRemaining = WindowDuration;
}

void AMedievalCharacter::EndDodge()
{
	bIsDodging = false;
	DodgeWindowRemaining = 0.0f;
}

void AMedievalCharacter::EndParry()
{
	bIsParrying = false;
	ParryWindowRemaining = 0.0f;
}

bool AMedievalCharacter::AttemptDodge()
{
	// Simple dodge check - in a real implementation, this would check timing
	return DodgeWindowRemaining > 0.0f;
}

bool AMedievalCharacter::AttemptParry()
{
	// Simple parry check - in a real implementation, this would check timing
	return ParryWindowRemaining > 0.0f;
}

void AMedievalCharacter::ExecuteCombatAction(UCombatAction* Action, AMedievalCharacter* Target)
{
	if (Action)
	{
		Action->ExecuteAction(this, Target);
	}
}

void AMedievalCharacter::ExecuteCombatActionWithDeferredDamage(UCombatAction* Action, AMedievalCharacter* Target)
{
	if (Action)
	{
		Action->ExecuteActionWithDeferredDamage(this, Target);
	}
}

void AMedievalCharacter::UpdateRealTimeCombat(float DeltaTime)
{
	// Update Escapade window (Kynde's Evasion)
	if (bIsDodging)
	{
		DodgeWindowRemaining -= DeltaTime;
		if (DodgeWindowRemaining <= 0.0f)
		{
			EndDodge();
		}
	}

	// Update Ward window (Trewthe's Sheeld)
	if (bIsParrying)
	{
		ParryWindowRemaining -= DeltaTime;
		if (ParryWindowRemaining <= 0.0f)
		{
			EndParry();
		}
	}
}

void AMedievalCharacter::CheckDefeated()
{
	if (!IsAlive())
	{
		OnCharacterDefeated.Broadcast(this);
	}
}

int32 AMedievalCharacter::GetExperienceForNextLevel() const
{
	return Stats.Level * 100;
}

void AMedievalCharacter::AddExperience(int32 Amount)
{
	Stats.ExperiencePoints += Amount;
	while (Stats.ExperiencePoints >= GetExperienceForNextLevel())
	{
		Stats.ExperiencePoints -= GetExperienceForNextLevel();
		Stats.Level++;
		// Stat increases on level up
		Stats.MaxHealth += 10.0f;
		Stats.CurrentHealth = Stats.MaxHealth;
		Stats.MaxStamina += 8.0f;
		Stats.CurrentStamina = FMath::Min(Stats.CurrentStamina + 8.0f, Stats.MaxStamina);
		Stats.AttackPower += 1.0f;
		Stats.Defense += 0.5f;
		Stats.Speed += 0.3f;
		if (Stats.MaxMana > 0.0f)
		{
			Stats.MaxMana += 5.0f;
			Stats.CurrentMana = FMath::Min(Stats.CurrentMana + 5.0f, Stats.MaxMana);
		}
	}
}

// Status Effect Functions
void AMedievalCharacter::ApplyStatusEffect(UStatusEffect* Effect)
{
	if (!Effect)
	{
		return;
	}

	// Check if effect already exists
	UStatusEffect* ExistingEffect = GetStatusEffect(Effect->EffectData.EffectType);
	if (ExistingEffect)
	{
		// Stack the effect
		ExistingEffect->StackEffect(Effect->EffectData.StackCount);
		ExistingEffect->ApplyEffect(this);
	}
	else
	{
		// Add new effect
		ActiveStatusEffects.Add(Effect);
		Effect->ApplyEffect(this);
	}
}

void AMedievalCharacter::RemoveStatusEffect(EStatusEffectType EffectType)
{
	for (int32 i = ActiveStatusEffects.Num() - 1; i >= 0; i--)
	{
		if (ActiveStatusEffects[i] && ActiveStatusEffects[i]->EffectData.EffectType == EffectType)
		{
			ActiveStatusEffects[i]->RemoveEffect(this);
			ActiveStatusEffects.RemoveAt(i);
			break;
		}
	}
}

bool AMedievalCharacter::HasStatusEffect(EStatusEffectType EffectType) const
{
	return GetStatusEffect(EffectType) != nullptr;
}

UStatusEffect* AMedievalCharacter::GetStatusEffect(EStatusEffectType EffectType) const
{
	for (UStatusEffect* Effect : ActiveStatusEffects)
	{
		if (Effect && Effect->EffectData.EffectType == EffectType)
		{
			return Effect;
		}
	}
	return nullptr;
}

void AMedievalCharacter::UpdateStatusEffects(float DeltaTime)
{
	for (int32 i = ActiveStatusEffects.Num() - 1; i >= 0; i--)
	{
		if (!ActiveStatusEffects[i])
		{
			ActiveStatusEffects.RemoveAt(i);
			continue;
		}

		UStatusEffect* Effect = ActiveStatusEffects[i];
		
		// Update effect timer
		Effect->UpdateEffect(DeltaTime);

		// Apply damage over time (Hunger causes constant damage)
		if (Effect->EffectData.DamagePerSecond > 0.0f)
		{
			Effect->ApplyDamageOverTime(this, DeltaTime);
		}

		// Remove expired effects
		if (Effect->IsExpired())
		{
			Effect->RemoveEffect(this);
			ActiveStatusEffects.RemoveAt(i);
		}
	}
}

void AMedievalCharacter::ApplyHunger(int32 Stacks, float Duration)
{
	// Hunger is the WORST status effect - apply it
	UStatusEffect* HungerEffect = UStatusEffect::CreateHungerEffect(Duration, Stacks);
	if (HungerEffect)
	{
		ApplyStatusEffect(HungerEffect);
	}
}

void AMedievalCharacter::RemoveHunger()
{
	RemoveStatusEffect(EStatusEffectType::Hunger);
}

bool AMedievalCharacter::IsHungry() const
{
	return HasStatusEffect(EStatusEffectType::Hunger);
}
void AMedievalCharacter::UpdateActionCooldowns(float DeltaTime)
{
	// TODO: Implement action cooldown logic
	// For now, this is a stub to satisfy the build
}