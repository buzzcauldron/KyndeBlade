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

	// Execute the action
	CurrentCharacter->ExecuteCombatAction(Action, Target);

	// If action has real-time mechanics, start the window
	if (Action->ActionData.ActionType == ECombatActionType::Dodge || 
		Action->ActionData.ActionType == ECombatActionType::Parry)
	{
		StartRealTimeWindow(Action->ActionData.SuccessWindow);
	}
	else
	{
		// Move to next turn after a short delay
		CombatState = ECombatState::ProcessingResults;
		FTimerHandle TimerHandle;
		GetWorld()->GetTimerManager().SetTimer(TimerHandle, this, &ATurnManager::NextTurn, 0.01f, false);
	}
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
