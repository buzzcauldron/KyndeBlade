#include "Combat/TurnManager.h"
#include "Characters/MedievalCharacter.h"
#include "Combat/CombatAction.h"
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
			CombatState = ECombatState::ProcessingResults;
			// Process results and move to next turn
			NextTurn();
		}
		return; // Early return to prevent processing action timing
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

	// Execute the action
	CurrentCharacter->ExecuteCombatAction(CurrentExecutingAction, CurrentActionTarget);

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

	// If action has real-time mechanics, start the window
	if (CurrentExecutingAction->ActionData.ActionType == ECombatActionType::Dodge || 
		CurrentExecutingAction->ActionData.ActionType == ECombatActionType::Parry)
	{
		StartRealTimeWindow(CurrentExecutingAction->ActionData.SuccessWindow);
	}
	else
	{
		// Move to next turn after a short delay
		CombatState = ECombatState::ProcessingResults;
		// Clear any existing timer first
		GetWorld()->GetTimerManager().ClearTimer(NextTurnTimerHandle);
		GetWorld()->GetTimerManager().SetTimer(NextTurnTimerHandle, this, &ATurnManager::NextTurn, 0.01f, false);
	}

	// Clear action references
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

	// Set up dodge/parry windows for characters
	if (CurrentCharacter)
	{
		// Enable real-time mechanics for the current character
		// This will be handled by the character's combat system
	}
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
