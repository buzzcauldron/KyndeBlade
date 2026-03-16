#include "CombatPlayerController.h"
#include "../Combat/TurnManager.h"
#include "../Characters/MedievalCharacter.h"
#include "../Input/CombatInputComponent.h"
#include "../UI/CombatUI.h"
#include "KyndeBladeGameMode.h"

ACombatPlayerController::ACombatPlayerController(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	CombatInputComponent = CreateDefaultSubobject<UCombatInputComponent>(TEXT("CombatInputComponent"));
}

void ACombatPlayerController::BeginPlay()
{
	Super::BeginPlay();
	CreateCombatUI();
	BindToTurnManager();
}

void ACombatPlayerController::SetupInputComponent()
{
	Super::SetupInputComponent();
	if (CombatInputComponent)
	{
		CombatInputComponent->SetupInputBindings(InputComponent);
	}
}

void ACombatPlayerController::BindToTurnManager()
{
	AKyndeBladeGameMode* GameMode = GetWorld() ? Cast<AKyndeBladeGameMode>(GetWorld()->GetAuthGameMode()) : nullptr;
	if (!GameMode)
	{
		return;
	}
	TurnManager = GameMode->GetTurnManager();
	if (!TurnManager)
	{
		return;
	}
	TurnManager->OnRealTimeWindowStarted.AddDynamic(this, &ACombatPlayerController::OnRealTimeWindowStarted);
	TurnManager->OnRealTimeWindowEnded.AddDynamic(this, &ACombatPlayerController::OnRealTimeWindowEnded);

	TurnManager->OnTurnChanged.AddDynamic(this, &ACombatPlayerController::OnTurnChangedForUI);
	TurnManager->OnCombatEnded.AddDynamic(this, &ACombatPlayerController::OnCombatEndedForUI);

	if (CombatInputComponent)
	{
		CombatInputComponent->OnDodgeInput.AddDynamic(this, &ACombatPlayerController::OnDodgePressed);
		CombatInputComponent->OnParryInput.AddDynamic(this, &ACombatPlayerController::OnParryPressed);
		CombatInputComponent->OnCounterInput.AddDynamic(this, &ACombatPlayerController::OnCounterPressed);
	}
}

void ACombatPlayerController::OnRealTimeWindowStarted(AMedievalCharacter* CharacterWithWindow)
{
	RealTimeWindowCharacter = CharacterWithWindow;
	if (CombatInputComponent)
	{
		CombatInputComponent->SetRealTimeWindowActive(true);
	}
}

void ACombatPlayerController::OnRealTimeWindowEnded()
{
	RealTimeWindowCharacter = nullptr;
	if (CombatInputComponent)
	{
		CombatInputComponent->SetRealTimeWindowActive(false);
	}
}

void ACombatPlayerController::OnDodgePressed()
{
	if (TurnManager && RealTimeWindowCharacter)
	{
		TurnManager->NotifyDodgeSuccess(RealTimeWindowCharacter);
	}
}

void ACombatPlayerController::OnParryPressed()
{
	if (TurnManager && RealTimeWindowCharacter)
	{
		TurnManager->NotifyParrySuccess(RealTimeWindowCharacter);
	}
}

void ACombatPlayerController::OnCounterPressed()
{
	// Counter can be wired to TurnManager or character when counter-attack is implemented
	if (TurnManager && RealTimeWindowCharacter)
	{
		// Optional: TurnManager->NotifyCounterSuccess(RealTimeWindowCharacter);
	}
}

void ACombatPlayerController::OnTurnChangedForUI(AMedievalCharacter* CurrentCharacter)
{
	if (CombatUIWidget && TurnManager)
	{
		CombatUIWidget->UpdateTurnInfo(CurrentCharacter, TurnManager->GetCurrentTurnNumber());
		if (CurrentCharacter)
		{
			CombatUIWidget->UpdateCharacterInfo(CurrentCharacter);
		}
	}
}

void ACombatPlayerController::OnCombatEndedForUI()
{
	if (!CombatUIWidget || !TurnManager)
	{
		return;
	}
	bool bPlayerVictory = true;
	const TArray<AMedievalCharacter*>& Enemies = TurnManager->GetEnemyCharacters();
	for (AMedievalCharacter* Enemy : Enemies)
	{
		if (Enemy && Enemy->IsAlive())
		{
			bPlayerVictory = false;
			break;
		}
	}
	CombatUIWidget->OnCombatEnded(bPlayerVictory);
}

void ACombatPlayerController::CreateCombatUI()
{
	if (!CombatUIClass || !GetWorld())
	{
		return;
	}
	UCombatUI* Widget = CreateWidget<UCombatUI>(this, CombatUIClass);
	if (Widget)
	{
		CombatUIWidget = Widget;
		Widget->AddToViewport();
	}
}
