#include "MageCharacter.h"
#include "MedievalCharacter.h"

AMageCharacter::AMageCharacter(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	CharacterClass = ECharacterClass::Mage;
	MaxMana = 100.0f;
	CurrentMana = 100.0f;
}

void AMageCharacter::BeginPlay()
{
	Super::BeginPlay();
	CharacterClass = ECharacterClass::Mage;
	CurrentMana = MaxMana;
}

void AMageCharacter::Fireball(AMedievalCharacter* Target)
{
	if (Target && CurrentMana >= FireballManaCost)
	{
		ConsumeMana(FireballManaCost);
		Target->ApplyCustomDamage(FireballDamage, this);
		// Could add burning effect
	}
}

void AMageCharacter::HealTarget(AMedievalCharacter* Target)
{
	if (Target && CurrentMana >= HealManaCost)
	{
		ConsumeMana(HealManaCost);
		Target->Heal(HealAmount);
	}
}

void AMageCharacter::LightningBolt(AMedievalCharacter* Target)
{
	if (Target && CurrentMana >= LightningBoltManaCost)
	{
		ConsumeMana(LightningBoltManaCost);
		Target->ApplyCustomDamage(LightningBoltDamage, this);
		// Could add stun effect
	}
}

void AMageCharacter::ConsumeMana(float Amount)
{
	CurrentMana = FMath::Max(0.0f, CurrentMana - Amount);
}
