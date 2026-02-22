#include "RogueCharacter.h"
#include "Components/CapsuleComponent.h"

ARogueCharacter::ARogueCharacter(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	CharacterClass = ECharacterClass::Rogue;
	CharacterName = TEXT("Rogue");
}

void ARogueCharacter::BeginPlay()
{
	Super::BeginPlay();
	CharacterClass = ECharacterClass::Rogue;
}

void ARogueCharacter::Backstab(AMedievalCharacter* Target)
{
	if (Target && GetCurrentStamina() >= BackstabStaminaCost)
	{
		ConsumeStamina(BackstabStaminaCost);
		float Damage = Stats.AttackPower * BackstabDamageMultiplier;
		Target->ApplyCustomDamage(Damage, this);
	}
}

void ARogueCharacter::Evade()
{
	if (GetCurrentStamina() >= EvadeStaminaCost)
	{
		ConsumeStamina(EvadeStaminaCost);
		StartDodge(EvadeWindowDuration);
	}
}

void ARogueCharacter::CriticalStrike(AMedievalCharacter* Target)
{
	if (Target && GetCurrentStamina() >= CriticalStrikeStaminaCost)
	{
		ConsumeStamina(CriticalStrikeStaminaCost);
		float Damage = Stats.AttackPower * 1.5f;
		if (FMath::FRand() < CriticalStrikeChance)
		{
			Damage *= CriticalStrikeMultiplier;
		}
		Target->ApplyCustomDamage(Damage, this);
	}
}
