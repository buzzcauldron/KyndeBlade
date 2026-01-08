#include "Game/KyndeBladeGameMode.h"
#include "Combat/TurnManager.h"
#include "Characters/MedievalCharacter.h"

AKyndeBladeGameMode::AKyndeBladeGameMode()
{
	TurnManager = nullptr;
	TurnManagerClass = ATurnManager::StaticClass();
}

void AKyndeBladeGameMode::BeginPlay()
{
	Super::BeginPlay();
	SpawnTurnManager();
}

void AKyndeBladeGameMode::SpawnTurnManager()
{
	if (TurnManagerClass)
	{
		FActorSpawnParameters SpawnParams;
		SpawnParams.Owner = this;
		TurnManager = GetWorld()->SpawnActor<ATurnManager>(TurnManagerClass, SpawnParams);
	}
}

void AKyndeBladeGameMode::StartCombatSequence(const TArray<AMedievalCharacter*>& PlayerCharacters, const TArray<AMedievalCharacter*>& EnemyCharacters)
{
	if (TurnManager)
	{
		TurnManager->InitializeCombat(PlayerCharacters, EnemyCharacters);
		TurnManager->StartCombat();
	}
}
