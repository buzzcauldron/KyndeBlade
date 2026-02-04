#include "TurnManager.h"
#include "../Characters/MedievalCharacter.h"
#include "CombatAction.h"
#include "Algo/Sort.h"

ATurnManager::ATurnManager()
{
	PrimaryActorTick.bCanEverTick = true;
	CombatState = ECombatState::CombatEnded;
	CurrentCharacter = nullptr;
	TurnNumber = 0;
	RealTimeWindowRemaining = 0.0f;
}

void ATurnManager::BeginPlay()
{
	Super::BeginPlay();
}

void ATurnManager::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);

	// Update real-time window
	if (CombatState == ECombatState::RealTimeWindow)
	{
		RealTimeWindowRemaining -= DeltaTime;
		if (RealTimeWindowRemaining <= 0.0f)
		{
			// Timeout: apply full pending damage if any (defender failed to dodge/parry)
			if (PendingDamageTarget && PendingDamageAttacker)
			{
				PendingDamageTarget->ApplyCustomDamage(PendingDamageAmount, PendingDamageAttacker);
				if (PendingBreakDamage > 0.0f)
				{
					PendingDamageTarget->TakeBreakDamage(PendingBreakDamage);
				}
			}
			PendingDamageAmount = 0.0f;
			PendingBreakDamage = 0.0f;
			PendingDamageTarget = nullptr;
			PendingDamageAttacker = nullptr;
			OnRealTimeWindowEnded.Broadcast();
			CombatState = ECombatState::ProcessingResults;
			NextTurn();
		}
		return;
	}

	// Process action timing (cast and execution phases)
	// Use else-if chain to ensure only one phase processes per frame
	if (CombatState == ECombatState::ExecutingAction)
	{
		// Process cast time first
		if (ActionCastTimeRemaining > 0.0f)
		{
			ActionCastTimeRemaining -= DeltaTime;
			if (ActionCastTimeRemaining <= 0.0f)
			{
				// Cast complete, move to execution phase
				ProcessActionCast();
				// Don't process execution in the same frame - it will be processed next frame
			}
		}
		// Only process execution if cast is complete (not in the same frame)
		else if (ActionExecutionTimeRemaining > 0.0f)
		{
			ActionExecutionTimeRemaining -= DeltaTime;
			if (ActionExecutionTimeRemaining <= 0.0f)
			{
				// Execution complete
				ProcessActionExecution();
			}
		}
	}
}

void ATurnManager::InitializeCombat(const TArray<AMedievalCharacter*>& InPlayerCharacters, const TArray<AMedievalCharacter*>& InEnemyCharacters)
{
	PlayerCharacters = InPlayerCharacters;
	EnemyCharacters = InEnemyCharacters;
	
	TurnOrder.Empty();
	TurnOrder.Append(PlayerCharacters);
	TurnOrder.Append(EnemyCharacters);

	// Remove null characters
	TurnOrder.RemoveAll([](AMedievalCharacter* Char) { return Char == nullptr; });

	CalculateTurnOrder();
}

void ATurnManager::AddCharacterToCombat(AMedievalCharacter* Character, bool bIsPlayer)
{
	if (!Character)
	{
		return;
	}

	if (bIsPlayer)
	{
		PlayerCharacters.AddUnique(Character);
	}
	else
	{
		EnemyCharacters.AddUnique(Character);
	}

	TurnOrder.AddUnique(Character);
	CalculateTurnOrder();
}

void ATurnManager::StartCombat()
{
	if (TurnOrder.Num() == 0)
	{
		return;
	}

	// Clear any existing timers
	GetWorld()->GetTimerManager().ClearTimer(NextTurnTimerHandle);

	CombatState = ECombatState::WaitingForInput;
	TurnNumber = 1;
	CurrentCharacter = TurnOrder[0];
	
	if (CurrentCharacter)
	{
		CurrentCharacter->OnTurnStart();
		OnTurnChanged.Broadcast(CurrentCharacter);
	}
}

void ATurnManager::EndTurn()
{
	if (CurrentCharacter)
	{
		CurrentCharacter->OnTurnEnd();
	}
}

void ATurnManager::NextTurn()
{
	EndTurn();

	// Check if combat should end
	CheckCombatEnd();
	if (CombatState == ECombatState::CombatEnded)
	{
		return;
	}

	// Find next alive character
	int32 CurrentIndex = TurnOrder.Find(CurrentCharacter);
	int32 NextIndex = (CurrentIndex + 1) % TurnOrder.Num();
	int32 StartIndex = NextIndex;

	do
	{
		AMedievalCharacter* NextChar = TurnOrder[NextIndex];
		if (NextChar && NextChar->IsAlive())
		{
			CurrentCharacter = NextChar;
			TurnNumber++;
			CombatState = ECombatState::WaitingForInput;
			
			if (CurrentCharacter)
			{
				CurrentCharacter->OnTurnStart();
				OnTurnChanged.Broadcast(CurrentCharacter);
			}
			return;
		}

		NextIndex = (NextIndex + 1) % TurnOrder.Num();
	} while (NextIndex != StartIndex);

	// No alive characters found - end combat
	GetWorld()->GetTimerManager().ClearTimer(NextTurnTimerHandle);
	CombatState = ECombatState::CombatEnded;
	OnCombatEnded.Broadcast();
}

void ATurnManager::ExecuteAction(UCombatAction* Action, AMedievalCharacter* Target)
{
	if (!Action || !CurrentCharacter || CombatState != ECombatState::WaitingForInput)
	{
		return;
	}

	CombatState = ECombatState::ExecutingAction;
	CurrentExecutingAction = Action;
	CurrentActionTarget = Target;

	// Handle cast time
	if (Action->GetCastTime() > 0.0f)
	{
		// Enter cast phase
		ActionCastTimeRemaining = Action->GetCastTime();
		ActionExecutionTimeRemaining = 0.0f;
		// Process cast time in Tick
	}
	else
	{
		// No cast time, execute immediately
		ProcessActionCast();
	}
}

void ATurnManager::ProcessActionCast()
{
	if (!CurrentExecutingAction || !CurrentCharacter)
	{
		return;
	}

	// For Strike vs player target, defer damage so defender gets real-time window
	const bool bDeferDamage = (CurrentExecutingAction->ActionData.ActionType == ECombatActionType::Strike
		&& CurrentActionTarget && PlayerCharacters.Contains(CurrentActionTarget));

	if (bDeferDamage)
	{
		CurrentCharacter->ExecuteCombatActionWithDeferredDamage(CurrentExecutingAction, CurrentActionTarget);
	}
	else
	{
		CurrentCharacter->ExecuteCombatAction(CurrentExecutingAction, CurrentActionTarget);
	}

	// Handle execution time
	if (CurrentExecutingAction->GetExecutionTime() > 0.0f)
	{
		ActionExecutionTimeRemaining = CurrentExecutingAction->GetExecutionTime();
		// Process execution time in Tick
	}
	else
	{
		// No execution time, finish immediately
		ProcessActionExecution();
	}
}

void ATurnManager::ProcessActionExecution()
{
	if (!CurrentExecutingAction)
	{
		return;
	}

	const ECombatActionType ActionType = CurrentExecutingAction->ActionData.ActionType;
	const bool bStrikeVsPlayer = (ActionType == ECombatActionType::Strike && CurrentActionTarget && PlayerCharacters.Contains(CurrentActionTarget));

	if (bStrikeVsPlayer)
	{
		// Store pending damage and give defender a real-time window
		float FinalDamage = CurrentExecutingAction->ActionData.Damage;
		if (CurrentActionTarget->IsBroken())
		{
			FinalDamage *= 1.5f;
		}
		PendingDamageAmount = FinalDamage;
		PendingBreakDamage = CurrentExecutingAction->ActionData.BreakDamage;
		PendingDamageTarget = CurrentActionTarget;
		PendingDamageAttacker = CurrentCharacter;
		StartRealTimeWindow(CurrentExecutingAction->ActionData.SuccessWindow > 0.0f ? CurrentExecutingAction->ActionData.SuccessWindow : 1.5f);
		OnRealTimeWindowStarted.Broadcast(CurrentActionTarget);
	}
	else if (ActionType == ECombatActionType::Escapade || ActionType == ECombatActionType::Ward)
	{
		StartRealTimeWindow(CurrentExecutingAction->ActionData.SuccessWindow);
		OnRealTimeWindowStarted.Broadcast(CurrentCharacter);
	}
	else
	{
		CombatState = ECombatState::ProcessingResults;
		GetWorld()->GetTimerManager().ClearTimer(NextTurnTimerHandle);
		GetWorld()->GetTimerManager().SetTimer(NextTurnTimerHandle, this, &ATurnManager::NextTurn, 0.01f, false);
	}

	CurrentExecutingAction = nullptr;
	CurrentActionTarget = nullptr;
	ActionCastTimeRemaining = 0.0f;
	ActionExecutionTimeRemaining = 0.0f;
}

void ATurnManager::StartRealTimeWindow(float Duration)
{
	RealTimeWindowDuration = Duration;
	RealTimeWindowRemaining = Duration;
	CombatState = ECombatState::RealTimeWindow;
}

void ATurnManager::NotifyDodgeSuccess(AMedievalCharacter* Defender)
{
	if (CombatState != ECombatState::RealTimeWindow || Defender != PendingDamageTarget)
	{
		return;
	}
	PendingDamageAmount = 0.0f;
	PendingBreakDamage = 0.0f;
	PendingDamageTarget = nullptr;
	PendingDamageAttacker = nullptr;
	OnRealTimeWindowEnded.Broadcast();
	CombatState = ECombatState::ProcessingResults;
	NextTurn();
}

void ATurnManager::NotifyParrySuccess(AMedievalCharacter* Defender)
{
	if (CombatState != ECombatState::RealTimeWindow || Defender != PendingDamageTarget)
	{
		return;
	}
	if (PendingDamageTarget && PendingDamageAttacker)
	{
		const float ReducedDamage = PendingDamageAmount * 0.3f;
		PendingDamageTarget->ApplyCustomDamage(ReducedDamage, PendingDamageAttacker);
		if (PendingBreakDamage > 0.0f)
		{
			PendingDamageTarget->TakeBreakDamage(PendingBreakDamage * 0.3f);
		}
	}
	PendingDamageAmount = 0.0f;
	PendingBreakDamage = 0.0f;
	PendingDamageTarget = nullptr;
	PendingDamageAttacker = nullptr;
	OnRealTimeWindowEnded.Broadcast();
	CombatState = ECombatState::ProcessingResults;
	NextTurn();
}

bool ATurnManager::IsPlayerTurn() const
{
	if (!CurrentCharacter)
	{
		return false;
	}

	return PlayerCharacters.Contains(CurrentCharacter);
}

void ATurnManager::CalculateTurnOrder()
{
	// Sort by speed (higher speed goes first)
	Algo::Sort(TurnOrder, [](const AMedievalCharacter* A, const AMedievalCharacter* B)
	{
		if (!A || !B) return false;
		return A->Stats.Speed > B->Stats.Speed;
	});
}

void ATurnManager::CheckCombatEnd()
{
	bool bPlayersDefeated = AreAllCharactersDefeated(PlayerCharacters);
	bool bEnemiesDefeated = AreAllCharactersDefeated(EnemyCharacters);

	if (bPlayersDefeated || bEnemiesDefeated)
	{
		// Clear any pending timers when combat ends
		GetWorld()->GetTimerManager().ClearTimer(NextTurnTimerHandle);
		CombatState = ECombatState::CombatEnded;
		OnCombatEnded.Broadcast();
	}
}

bool ATurnManager::AreAllCharactersDefeated(const TArray<AMedievalCharacter*>& Characters) const
{
	for (AMedievalCharacter* Character : Characters)
	{
		if (Character && Character->IsAlive())
		{
			return false;
		}
	}
	return true;
}
