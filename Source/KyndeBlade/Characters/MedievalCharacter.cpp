#include "Characters/MedievalCharacter.h"
#include "Components/CapsuleComponent.h"
#include "GameFramework/CharacterMovementComponent.h"
#include "Combat/CombatAction.h"

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
}

void AMedievalCharacter::TakeDamage(float Damage, AMedievalCharacter* Attacker)
{
	if (!IsAlive())
	{
		return;
	}

	// Apply defense reduction
	float ActualDamage = FMath::Max(0.0f, Damage - Stats.Defense);
	
	// Check for dodge/parry
	if (bIsDodging && AttemptDodge())
	{
		// Successful dodge - no damage
		return;
	}

	if (bIsParrying && AttemptParry())
	{
		// Successful parry - reduced damage and counter opportunity
		ActualDamage *= 0.3f;
		// Could trigger counter attack here
	}

	Stats.CurrentHealth = FMath::Max(0.0f, Stats.CurrentHealth - ActualDamage);
	OnHealthChanged.Broadcast(Stats.CurrentHealth, Stats.MaxHealth);

	CheckDefeated();
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
	Stats.CurrentStamina = FMath::Min(Stats.MaxStamina, Stats.CurrentStamina + Amount);
	OnStaminaChanged.Broadcast(Stats.CurrentStamina, Stats.MaxStamina);
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

void AMedievalCharacter::UpdateRealTimeCombat(float DeltaTime)
{
	// Update dodge window
	if (bIsDodging)
	{
		DodgeWindowRemaining -= DeltaTime;
		if (DodgeWindowRemaining <= 0.0f)
		{
			EndDodge();
		}
	}

	// Update parry window
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
