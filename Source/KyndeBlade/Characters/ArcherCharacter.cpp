#include "ArcherCharacter.h"
#include "../Combat/CombatAction.h"
#include "../Combat/StatusEffect.h"

AArcherCharacter::AArcherCharacter(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	CharacterClass = ECharacterClass::Archer;
	CharacterName = TEXT("Archer");
}

void AArcherCharacter::BeginPlay()
{
	Super::BeginPlay();
	CharacterClass = ECharacterClass::Archer;
}

void AArcherCharacter::QuickShot(AMedievalCharacter* Target)
{
	if (Target && GetCurrentStamina() >= QuickShotStaminaCost)
	{
		ConsumeStamina(QuickShotStaminaCost);
		Target->ApplyCustomDamage(QuickShotDamage + Stats.AttackPower * 0.5f, this);
	}
}

void AArcherCharacter::AimedShot(AMedievalCharacter* Target)
{
	if (Target && GetCurrentStamina() >= AimedShotStaminaCost)
	{
		ConsumeStamina(AimedShotStaminaCost);
		Target->ApplyCustomDamage(AimedShotDamage + Stats.AttackPower * 1.2f, this);
	}
}

void AArcherCharacter::PoisonArrow(AMedievalCharacter* Target)
{
	if (Target && GetCurrentStamina() >= PoisonArrowStaminaCost)
	{
		ConsumeStamina(PoisonArrowStaminaCost);
		Target->ApplyCustomDamage(PoisonArrowDamage + Stats.AttackPower * 0.5f, this);
		UStatusEffect* Poison = UStatusEffect::CreatePoisonEffect(5.0f, 3.0f);
		if (Poison)
		{
			Target->ApplyStatusEffect(Poison);
		}
	}
}
