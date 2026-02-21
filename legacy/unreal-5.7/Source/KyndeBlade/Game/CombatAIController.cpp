#include "CombatAIController.h"
#include "../Combat/TurnManager.h"
#include "../Combat/CombatAction.h"
#include "../Characters/MedievalCharacter.h"
#include "KyndeBladeGameMode.h"
#include "TimerManager.h"

ACombatAIController::ACombatAIController(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	PrimaryActorTick.bCanEverTick = true;
	ActionDelaySeconds = 0.8f;
}

void ACombatAIController::BeginPlay()
{
	Super::BeginPlay();
	AKyndeBladeGameMode* GameMode = GetWorld() ? Cast<AKyndeBladeGameMode>(GetWorld()->GetAuthGameMode()) : nullptr;
	if (GameMode)
	{
		TurnManager = GameMode->GetTurnManager();
	}
}

void ACombatAIController::Tick(float DeltaTime)
{
	Super::Tick(DeltaTime);
	TryExecuteEnemyTurn();
}

void ACombatAIController::TryExecuteEnemyTurn()
{
	if (!TurnManager || !GetPawn())
	{
		return;
	}
	AMedievalCharacter* MyCharacter = Cast<AMedievalCharacter>(GetPawn());
	if (!MyCharacter || !MyCharacter->IsAlive())
	{
		return;
	}
	if (TurnManager->GetCurrentCharacter() != MyCharacter)
	{
		bHasChosenThisTurn = false;
		return;
	}
	if (TurnManager->GetCombatState() != ECombatState::WaitingForInput)
	{
		return;
	}
	if (TurnManager->IsPlayerTurn())
	{
		return;
	}
	if (bHasChosenThisTurn)
	{
		return;
	}
	bHasChosenThisTurn = true;

	if (ActionDelaySeconds > 0.0f)
	{
		FTimerHandle TimerHandle;
		GetWorld()->GetTimerManager().SetTimer(TimerHandle, this, &ACombatAIController::ChooseAndExecuteAction, ActionDelaySeconds, false);
	}
	else
	{
		ChooseAndExecuteAction();
	}
}

void ACombatAIController::ChooseAndExecuteAction()
{
	if (!TurnManager || !GetPawn())
	{
		return;
	}
	AMedievalCharacter* MyCharacter = Cast<AMedievalCharacter>(GetPawn());
	if (!MyCharacter || TurnManager->GetCurrentCharacter() != MyCharacter)
	{
		return;
	}
	if (TurnManager->GetCombatState() != ECombatState::WaitingForInput)
	{
		return;
	}

	UCombatAction* ChosenAction = nullptr;
	for (UCombatAction* Action : MyCharacter->AvailableActions)
	{
		if (Action && Action->ActionData.StaminaCost <= MyCharacter->GetCurrentStamina())
		{
			ChosenAction = Action;
			break;
		}
	}

	AMedievalCharacter* ChosenTarget = nullptr;
	const TArray<AMedievalCharacter*>& Players = TurnManager->GetPlayerCharacters();
	for (AMedievalCharacter* Player : Players)
	{
		if (Player && Player->IsAlive())
		{
			ChosenTarget = Player;
			break;
		}
	}

	if (ChosenAction && ChosenTarget)
	{
		TurnManager->ExecuteAction(ChosenAction, ChosenTarget);
	}
	else if (ChosenAction && ChosenAction->ActionData.ActionType == ECombatActionType::Rest)
	{
		TurnManager->ExecuteAction(ChosenAction, nullptr);
	}
	else if (ChosenTarget)
	{
		// No valid action: use a default strike if we have no actions (fallback)
		UCombatAction* DefaultStrike = NewObject<UCombatAction>(MyCharacter);
		if (DefaultStrike)
		{
			DefaultStrike->ActionData.ActionType = ECombatActionType::Strike;
			DefaultStrike->ActionData.Damage = MyCharacter->Stats.AttackPower * 1.5f;
			DefaultStrike->ActionData.StaminaCost = 10.0f;
			DefaultStrike->ActionData.SuccessWindow = 1.5f;
			TurnManager->ExecuteAction(DefaultStrike, ChosenTarget);
		}
	}
}
