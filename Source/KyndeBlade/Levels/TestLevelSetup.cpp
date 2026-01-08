#include "Levels/TestLevelSetup.h"
#include "Characters/MedievalCharacter.h"
#include "Characters/Enemies/FalseCharacter.h"
#include "Characters/Enemies/LadyMedeCharacter.h"
#include "Characters/Enemies/WrathCharacter.h"
#include "Game/KyndeBladeGameMode.h"
#include "Combat/TurnManager.h"

ATestLevelSetup::ATestLevelSetup()
{
	PrimaryActorTick.bCanEverTick = false;
	
	// Default spawn locations for "Tower on the Toft" level
	// Enemies spawn at the base of the tower, spread out
	FalseSpawnLocation = FVector(500.0f, -300.0f, 0.0f);
	LadyMedeSpawnLocation = FVector(500.0f, 0.0f, 0.0f);
	WrathSpawnLocation = FVector(500.0f, 300.0f, 0.0f);

	// Set default classes
	FalseClass = AFalseCharacter::StaticClass();
	LadyMedeClass = ALadyMedeCharacter::StaticClass();
	WrathClass = AWrathCharacter::StaticClass();
}

void ATestLevelSetup::BeginPlay()
{
	Super::BeginPlay();
	
	// Auto-spawn enemies when level starts
	SpawnEnemies();
	
	// Auto-start combat after a short delay
	FTimerHandle TimerHandle;
	GetWorld()->GetTimerManager().SetTimer(TimerHandle, this, &ATestLevelSetup::StartCombat, 1.0f, false);
}

void ATestLevelSetup::SpawnEnemies()
{
	UWorld* World = GetWorld();
	if (!World)
	{
		return;
	}

	FActorSpawnParameters SpawnParams;
	SpawnParams.Owner = this;
	SpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AdjustIfPossibleButAlwaysSpawn;

	// Spawn False
	if (FalseClass)
	{
		FTransform FalseTransform(FRotator::ZeroRotator, FalseSpawnLocation);
		FalseEnemy = World->SpawnActor<AFalseCharacter>(FalseClass, FalseTransform, SpawnParams);
		if (FalseEnemy)
		{
			EnemyCharacters.Add(FalseEnemy);
		}
	}

	// Spawn Lady Mede
	if (LadyMedeClass)
	{
		FTransform MedeTransform(FRotator::ZeroRotator, LadyMedeSpawnLocation);
		LadyMedeEnemy = World->SpawnActor<ALadyMedeCharacter>(LadyMedeClass, MedeTransform, SpawnParams);
		if (LadyMedeEnemy)
		{
			EnemyCharacters.Add(LadyMedeEnemy);
		}
	}

	// Spawn Wrath
	if (WrathClass)
	{
		FTransform WrathTransform(FRotator::ZeroRotator, WrathSpawnLocation);
		WrathEnemy = World->SpawnActor<AWrathCharacter>(WrathClass, WrathTransform, SpawnParams);
		if (WrathEnemy)
		{
			EnemyCharacters.Add(WrathEnemy);
		}
	}
}

void ATestLevelSetup::StartCombat()
{
	AKyndeBladeGameMode* GameMode = Cast<AKyndeBladeGameMode>(GetWorld()->GetAuthGameMode());
	if (GameMode && PlayerCharacters.Num() > 0 && EnemyCharacters.Num() > 0)
	{
		GameMode->StartCombatSequence(PlayerCharacters, EnemyCharacters);
	}
}
