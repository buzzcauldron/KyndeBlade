#include "Characters/Enemies/LadyMedeCharacter.h"
#include "Characters/MedievalCharacter.h"

ALadyMedeCharacter::ALadyMedeCharacter(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	CharacterClass = ECharacterClass::Mage;
	CharacterName = TEXT("Lady Mede");
	MaxCorruptionMana = 120.0f;
	CurrentCorruptionMana = 120.0f;
}

void ALadyMedeCharacter::BeginPlay()
{
	Super::BeginPlay();
	
	// Lady Mede is a corrupt mage - high magic, moderate health
	CharacterClass = ECharacterClass::Mage;
	Stats.MaxHealth = 95.0f;
	Stats.CurrentHealth = 95.0f;
	Stats.MaxStamina = 160.0f;
	Stats.CurrentStamina = 160.0f;
	Stats.AttackPower = 22.0f; // High magic power
	Stats.Defense = 4.0f; // Low physical defense
	Stats.Speed = 9.0f;
	CurrentCorruptionMana = MaxCorruptionMana;
}

void ALadyMedeCharacter::CorruptingGold(AMedievalCharacter* Target)
{
	if (Target && CurrentCorruptionMana >= CorruptingGoldManaCost)
	{
		ConsumeCorruptionMana(CorruptingGoldManaCost);
		Target->TakeDamage(CorruptingGoldDamage, this);
		// Could add corruption status effect that reduces stats
	}
}

void ALadyMedeCharacter::BriberyCharm(AMedievalCharacter* Target)
{
	if (Target && CurrentCorruptionMana >= BriberyCharmManaCost)
	{
		ConsumeCorruptionMana(BriberyCharmManaCost);
		// Charm could reduce target's attack or make them skip a turn
		// This would be handled by status effects in a full implementation
	}
}

void ALadyMedeCharacter::WealthBolt(AMedievalCharacter* Target)
{
	if (Target && CurrentCorruptionMana >= WealthBoltManaCost)
	{
		ConsumeCorruptionMana(WealthBoltManaCost);
		Target->TakeDamage(WealthBoltDamage, this);
		// Could add gold-themed visual effects
	}
}

void ALadyMedeCharacter::ConsumeCorruptionMana(float Amount)
{
	CurrentCorruptionMana = FMath::Max(0.0f, CurrentCorruptionMana - Amount);
}
