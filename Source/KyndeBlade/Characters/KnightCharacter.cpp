#include "KnightCharacter.h"
#include "MedievalCharacter.h"

AKnightCharacter::AKnightCharacter(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	CharacterClass = ECharacterClass::Knight;
}

void AKnightCharacter::BeginPlay()
{
	Super::BeginPlay();
	// Knight-specific initialization
	CharacterClass = ECharacterClass::Knight;
}

void AKnightCharacter::ShieldBash(AMedievalCharacter* Target)
{
	if (Target && Stats.CurrentStamina >= 25.0f)
	{
		ConsumeStamina(25.0f);
		Target->ApplyCustomDamage(ShieldBashDamage, this);
		// Could add stun effect here
	}
}

void AKnightCharacter::DefensiveStance()
{
	if (Stats.CurrentStamina >= 30.0f)
	{
		ConsumeStamina(30.0f);
		bInDefensiveStance = true;
		Stats.Defense += DefensiveStanceDefenseBonus;
		// Could add visual effect
	}
}

void AKnightCharacter::ChargeAttack(AMedievalCharacter* Target)
{
	if (Target && Stats.CurrentStamina >= 40.0f)
	{
		ConsumeStamina(40.0f);
		float ChargeDamage = Stats.AttackPower * 2.0f;
		Target->ApplyCustomDamage(ChargeDamage, this);
	}
}
