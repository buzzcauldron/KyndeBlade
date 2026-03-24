#include "FalseCharacter.h"
#include "../MedievalCharacter.h"
#include "../../Game/CombatAIController.h"

AFalseCharacter::AFalseCharacter(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	CharacterClass = ECharacterClass::Rogue;
	CharacterName = TEXT("False");
	AIControllerClass = ACombatAIController::StaticClass();
}

void AFalseCharacter::BeginPlay()
{
	Super::BeginPlay();
	
	// False is a deceptive rogue - high speed, moderate attack
	CharacterClass = ECharacterClass::Rogue;
	Stats.MaxHealth = 110.0f;
	Stats.CurrentHealth = 110.0f;
	Stats.MaxStamina = 150.0f;
	Stats.CurrentStamina = 150.0f;
	Stats.AttackPower = 16.0f;
	Stats.Defense = 6.0f;
	Stats.Speed = 16.0f; // Very fast, deceptive
}

void AFalseCharacter::DeceptiveStrike(AMedievalCharacter* Target)
{
	if (Target && Stats.CurrentStamina >= 25.0f)
	{
		ConsumeStamina(25.0f);
		float DeceptiveDamage = Stats.AttackPower * DeceptionDamageMultiplier;
		Target->ApplyCustomDamage(DeceptiveDamage, this);
		// Could add confusion or debuff effect
	}
}

void AFalseCharacter::FeintAttack(AMedievalCharacter* Target)
{
	if (Target && Stats.CurrentStamina >= FeintStaminaCost)
	{
		ConsumeStamina(FeintStaminaCost);
		bIsFeinting = true;
		// Feint reduces target's defense on next attack
		// This would be handled by status effects in a full implementation
	}
}

void AFalseCharacter::ShadowStep(AMedievalCharacter* Target)
{
	if (Target && Stats.CurrentStamina >= 35.0f)
	{
		ConsumeStamina(35.0f);
		// Teleport behind target and attack
		float BackstabDamage = Stats.AttackPower * 2.0f;
		Target->ApplyCustomDamage(BackstabDamage, this);
	}
}
