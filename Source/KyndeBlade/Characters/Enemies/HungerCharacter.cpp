#include "Characters/Enemies/HungerCharacter.h"
#include "Characters/MedievalCharacter.h"
#include "Combat/StatusEffect.h"

AHungerCharacter::AHungerCharacter(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	CharacterClass = ECharacterClass::Knight; // Tank-type, but represents the weight of hunger
	CharacterName = TEXT("Hunger");
	HungerPower = 1.0f;
	bBarrenFieldActive = false;
	BarrenFieldDuration = 0.0f;
}

void AHungerCharacter::BeginPlay()
{
	Super::BeginPlay();
	
	// Hunger is a powerful boss - high health, moderate attack, but devastating status effects
	CharacterClass = ECharacterClass::Knight;
	Stats.MaxHealth = 250.0f;
	Stats.CurrentHealth = 250.0f;
	Stats.MaxStamina = 150.0f;
	Stats.CurrentStamina = 150.0f;
	Stats.AttackPower = 16.0f; // Moderate attack, but hunger effects are the real threat
	Stats.Defense = 12.0f;
	Stats.Speed = 6.0f; // Slow but inexorable, like hunger itself
	HungerPower = 1.0f;
}

void AHungerCharacter::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	UpdateHungerPower();
	
	if (bBarrenFieldActive)
	{
		UpdateBarrenField(DeltaTime);
	}
}

void AHungerCharacter::UpdateHungerPower()
{
	// Hunger's power grows as more enemies become hungry
	// Note: In full implementation, would get enemies from combat system
	// For now, HungerPower updates based on combat state
	// Hunger becomes stronger as more enemies suffer
	Stats.AttackPower = 16.0f * HungerPower;
}

void AHungerCharacter::UpdateBarrenField(float DeltaTime)
{
	if (!bBarrenFieldActive)
	{
		return;
	}

	BarrenFieldDuration -= DeltaTime;
	if (BarrenFieldDuration <= 0.0f)
	{
		bBarrenFieldActive = false;
		BarrenFieldDuration = 0.0f;
	}
}

void AHungerCharacter::HungersGrip(const TArray<AMedievalCharacter*>& Targets)
{
	if (Stats.CurrentStamina < HungersGripStaminaCost)
	{
		return;
	}

	ConsumeStamina(HungersGripStaminaCost);

	// From Piers Plowman: Hunger forces people to work
	// Apply hunger status effect to all targets
	for (AMedievalCharacter* Target : Targets)
	{
		if (Target && Target->IsAlive())
		{
			// Apply hunger (1 stack, permanent until removed)
			Target->ApplyHunger(1, 0.0f);
		}
	}
}

void AHungerCharacter::TheEmptyBelly(const TArray<AMedievalCharacter*>& Targets)
{
	if (Stats.CurrentStamina < TheEmptyBellyStaminaCost)
	{
		return;
	}

	ConsumeStamina(TheEmptyBellyStaminaCost);

	// From Piers Plowman: The physical reality of hunger
	// Drains stamina and Kynde from all enemies
	for (AMedievalCharacter* Target : Targets)
	{
		if (Target && Target->IsAlive())
		{
			// Drain stamina
			float StaminaDrain = 30.0f * HungerPower;
			Target->ConsumeStamina(StaminaDrain);
			
			// Drain Kynde
			float KyndeDrain = 3.0f * HungerPower;
			if (Target->GetCurrentKynde() >= KyndeDrain)
			{
				Target->ConsumeKynde(KyndeDrain);
			}
			
			// Apply hunger if not already hungry
			if (!Target->IsHungry())
			{
				Target->ApplyHunger(1, 0.0f);
			}
		}
	}
}

void AHungerCharacter::TheBarrenField()
{
	if (Stats.CurrentStamina < TheBarrenFieldStaminaCost)
	{
		return;
	}

	ConsumeStamina(TheBarrenFieldStaminaCost);

	// From Piers Plowman: Hunger comes from lack of food
	// Creates an area effect that applies hunger over time
	bBarrenFieldActive = true;
	BarrenFieldDuration = BarrenFieldMaxDuration;
	
	// This would be handled by a level/area effect system
	// For now, it's a timed ability that will affect enemies in range
}

void AHungerCharacter::TheWorkThatNeverEnds(AMedievalCharacter* Target)
{
	if (!Target || Stats.CurrentStamina < TheWorkThatNeverEndsStaminaCost)
	{
		return;
	}

	ConsumeStamina(TheWorkThatNeverEndsStaminaCost);

	// From Piers Plowman: Hunger forces endless labor
	// Applies hunger and forces target to consume more stamina
	Target->ApplyHunger(1, 0.0f);
	
	// Force stamina consumption
	float ForcedStaminaCost = 40.0f * HungerPower;
	Target->ConsumeStamina(ForcedStaminaCost);
	
	// Deal damage representing the exhaustion of endless work
	float ExhaustionDamage = 15.0f * HungerPower;
	Target->TakeDamage(ExhaustionDamage, this);
}

void AHungerCharacter::TheFeastOfWant()
{
	if (Stats.CurrentStamina < TheFeastOfWantStaminaCost)
	{
		return;
	}

	ConsumeStamina(TheFeastOfWantStaminaCost);

	// From Piers Plowman: Hunger feeds on the hungry
	// Heals Hunger based on how many enemies are hungry
	// Note: In full implementation, would get enemies from combat system
	// For now, uses current HungerPower to determine healing
	float HealAmount = 20.0f * HungerPower;
	Heal(HealAmount);
	
	// Increase hunger power
	HungerPower = FMath::Min(MaxHungerPower, HungerPower + 0.1f);
}

void AHungerCharacter::TheUnendingNeed(const TArray<AMedievalCharacter*>& Targets)
{
	if (Stats.CurrentStamina < TheUnendingNeedStaminaCost)
	{
		return;
	}

	ConsumeStamina(TheUnendingNeedStaminaCost);

	// From Piers Plowman: Hunger is never satisfied
	// Stacks hunger on all enemies, making it worse
	for (AMedievalCharacter* Target : Targets)
	{
		if (Target && Target->IsAlive())
		{
			if (Target->IsHungry())
			{
				// Stack more hunger
				UStatusEffect* HungerEffect = Target->GetStatusEffect(EStatusEffectType::Hunger);
				if (HungerEffect)
				{
					HungerEffect->StackEffect(1);
					HungerEffect->ApplyEffect(Target);
				}
			}
			else
			{
				// Apply initial hunger
				Target->ApplyHunger(1, 0.0f);
			}
		}
	}
}

int32 AHungerCharacter::GetHungryEnemyCount(const TArray<AMedievalCharacter*>& Enemies) const
{
	// Count how many enemies are hungry
	int32 HungryCount = 0;
	for (AMedievalCharacter* Enemy : Enemies)
	{
		if (Enemy && Enemy->IsAlive() && Enemy->IsHungry())
		{
			HungryCount++;
		}
	}
	return HungryCount;
}
