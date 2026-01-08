#include "Characters/Enemies/WrathCharacter.h"
#include "Characters/MedievalCharacter.h"

AWrathCharacter::AWrathCharacter(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	CharacterClass = ECharacterClass::Knight;
	CharacterName = TEXT("Wrath");
	RageLevel = 0.0f;
	bIsBerserk = false;
}

void AWrathCharacter::BeginPlay()
{
	Super::BeginPlay();
	
	// Wrath is an enraged warrior - high health, high attack when enraged
	CharacterClass = ECharacterClass::Knight;
	Stats.MaxHealth = 180.0f;
	Stats.CurrentHealth = 180.0f;
	Stats.MaxStamina = 110.0f;
	Stats.CurrentStamina = 110.0f;
	Stats.AttackPower = 18.0f;
	Stats.Defense = 8.0f;
	Stats.Speed = 7.0f; // Slower but powerful
	RageLevel = 0.0f;
}

void AWrathCharacter::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	UpdateRage();
}

void AWrathCharacter::RageStrike(AMedievalCharacter* Target)
{
	if (Target && Stats.CurrentStamina >= RageStrikeStaminaCost)
	{
		ConsumeStamina(RageStrikeStaminaCost);
		// Damage increases with rage level
		float RageBonus = 1.0f + (RageLevel / MaxRageLevel);
		float RageDamage = Stats.AttackPower * RageBonus * 1.5f;
		Target->TakeDamage(RageDamage, this);
	}
}

void AWrathCharacter::BerserkerRage()
{
	if (Stats.CurrentStamina >= BerserkerRageStaminaCost)
	{
		ConsumeStamina(BerserkerRageStaminaCost);
		bIsBerserk = true;
		RageLevel = MaxRageLevel;
		// Increase attack power significantly
		Stats.AttackPower *= 1.5f;
		// Reduce defense (berserkers are reckless)
		Stats.Defense *= 0.7f;
	}
}

void AWrathCharacter::FuriousCharge(AMedievalCharacter* Target)
{
	if (Target && Stats.CurrentStamina >= 40.0f)
	{
		ConsumeStamina(40.0f);
		float ChargeDamage = Stats.AttackPower * 2.0f;
		if (bIsBerserk)
		{
			ChargeDamage *= 1.5f;
		}
		Target->TakeDamage(ChargeDamage, this);
	}
}

void AWrathCharacter::UpdateRage()
{
	// Rage increases as health decreases
	if (Stats.MaxHealth > 0.0f && MaxRageLevel > 0.0f)
	{
		float HealthPercentage = Stats.CurrentHealth / Stats.MaxHealth;
		RageLevel = FMath::Clamp((1.0f - HealthPercentage) * MaxRageLevel, 0.0f, MaxRageLevel);
		
		// Update rage damage multiplier
		RageDamageMultiplier = 1.0f + (RageLevel / MaxRageLevel) * 0.5f;
	}
}
