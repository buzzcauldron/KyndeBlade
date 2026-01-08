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
	UpdateActionCooldowns(DeltaTime);
	
	// Expedition 33-inspired: Update broken state
	if (bIsBroken && BrokenStunRemaining > 0.0f)
	{
		BrokenStunRemaining -= DeltaTime;
		if (BrokenStunRemaining <= 0.0f)
		{
			RecoverFromBreak();
		}
	}
}

void AMedievalCharacter::TakeDamage(float Damage, AMedievalCharacter* Attacker)
{
	if (!IsAlive())
	{
		return;
	}

	// Apply defense reduction
	float ActualDamage = FMath::Max(0.0f, Damage - Stats.Defense);
	
	// Expedition 33-inspired: Check for dodge/parry with VP rewards
	if (bIsDodging && AttemptDodge())
	{
		// Successful dodge - no damage, gain VP
		bool bPerfectDodge = (DodgeWindowRemaining <= PerfectDodgeWindow);
		float VPReward = bPerfectDodge ? 2.0f : 1.0f; // Perfect dodge gives more VP
		GainVirtuePoints(VPReward);
		return;
	}

	if (bIsParrying && AttemptParry())
	{
		// Successful parry - reduced damage, gain VP, counter opportunity
		bool bPerfectParry = (ParryWindowRemaining <= PerfectParryWindow);
		float VPReward = bPerfectParry ? 4.0f : 2.0f; // Perfect parry gives more VP
		GainVirtuePoints(VPReward);
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

// Expedition 33-inspired: Virtue Points (AP) management
void AMedievalCharacter::GainVirtuePoints(float Amount)
{
	CurrentVirtuePoints = FMath::Min(MaxVirtuePoints, CurrentVirtuePoints + Amount);
	OnVirtuePointsChanged.Broadcast(CurrentVirtuePoints, MaxVirtuePoints);
}

bool AMedievalCharacter::ConsumeVirtuePoints(float Amount)
{
	if (CurrentVirtuePoints >= Amount)
	{
		CurrentVirtuePoints -= Amount;
		OnVirtuePointsChanged.Broadcast(CurrentVirtuePoints, MaxVirtuePoints);
		return true;
	}
	return false;
}

// Expedition 33-inspired: Break system
void AMedievalCharacter::TakeBreakDamage(float BreakAmount)
{
	if (bIsBroken)
	{
		return; // Already broken, break gauge doesn't decrease further
	}

	CurrentBreakGauge = FMath::Max(0.0f, CurrentBreakGauge - BreakAmount);
	OnBreakGaugeChanged.Broadcast(CurrentBreakGauge, MaxBreakGauge);

	if (CurrentBreakGauge <= 0.0f)
	{
		BreakCharacter();
	}
}

void AMedievalCharacter::BreakCharacter()
{
	if (bIsBroken)
	{
		return; // Already broken
	}

	bIsBroken = true;
	BrokenStunRemaining = 2.0f; // Stunned for 2 turns (will be decremented in Tick)
	OnCharacterBroken.Broadcast(this);
}

void AMedievalCharacter::RecoverFromBreak()
{
	bIsBroken = false;
	BrokenStunRemaining = 0.0f;
	CurrentBreakGauge = MaxBreakGauge; // Reset break gauge
	OnBreakGaugeChanged.Broadcast(CurrentBreakGauge, MaxBreakGauge);
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
		// Expedition 33-inspired: Generate VP from melee attacks before executing
		// (VP generation is handled in CombatAction::ExecuteAction, but we can add bonus here)
		
		Action->ExecuteAction(this, Target);
		
		// Expedition 33-inspired: Apply break damage if action has it
		// (Break damage is handled in CombatAction::ExecuteAction)
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
