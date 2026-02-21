#include "KyndeBladeGameMode.h"
#include "../Combat/TurnManager.h"
#include "../Characters/MedievalCharacter.h"
#include "../Characters/KnightCharacter.h"
#include "../Characters/MageCharacter.h"
#include "../Characters/Enemies/FalseCharacter.h"
#include "../Characters/Enemies/LadyMedeCharacter.h"
#include "../Characters/Enemies/WrathCharacter.h"
#include "Engine/Engine.h"
#include "DrawDebugHelpers.h"

AKyndeBladeGameMode::AKyndeBladeGameMode()
{
	TurnManager = nullptr;
	TurnManagerClass = ATurnManager::StaticClass();
	bAutoSpawnTestCharacters = true;
	
	// Set default classes
	DefaultKnightClass = AKnightCharacter::StaticClass();
	DefaultMageClass = AMageCharacter::StaticClass();
	DefaultFalseClass = AFalseCharacter::StaticClass();
	DefaultLadyMedeClass = ALadyMedeCharacter::StaticClass();
	DefaultWrathClass = AWrathCharacter::StaticClass();
}

void AKyndeBladeGameMode::BeginPlay()
{
	Super::BeginPlay();
	
	UE_LOG(LogTemp, Warning, TEXT("=== KyndeBladeGameMode::BeginPlay called ==="));
	UE_LOG(LogTemp, Warning, TEXT("bAutoSpawnTestCharacters = %s"), bAutoSpawnTestCharacters ? TEXT("true") : TEXT("false"));
	
	SpawnTurnManager();
	
	if (TurnManager)
	{
		UE_LOG(LogTemp, Warning, TEXT("TurnManager spawned successfully"));
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("Failed to spawn TurnManager!"));
	}
	
	// Auto-spawn test characters if enabled
	if (bAutoSpawnTestCharacters)
	{
		UE_LOG(LogTemp, Warning, TEXT("Setting timer to spawn characters in 0.5 seconds"));
		// Delay slightly to ensure world is ready
		FTimerHandle TimerHandle;
		GetWorld()->GetTimerManager().SetTimer(TimerHandle, this, &AKyndeBladeGameMode::AutoSpawnTestCharacters, 0.5f, false);
	}
	else
	{
		UE_LOG(LogTemp, Warning, TEXT("Auto-spawn is disabled"));
	}
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

void AKyndeBladeGameMode::AutoSpawnTestCharacters()
{
	UE_LOG(LogTemp, Warning, TEXT("=== AutoSpawnTestCharacters called ==="));
	
	UWorld* World = GetWorld();
	if (!World)
	{
		UE_LOG(LogTemp, Error, TEXT("ERROR: No World available for auto-spawning"));
		return;
	}

	UE_LOG(LogTemp, Warning, TEXT("World is valid, checking classes..."));
	UE_LOG(LogTemp, Warning, TEXT("DefaultKnightClass: %s"), DefaultKnightClass ? *DefaultKnightClass->GetName() : TEXT("NULL"));
	UE_LOG(LogTemp, Warning, TEXT("DefaultMageClass: %s"), DefaultMageClass ? *DefaultMageClass->GetName() : TEXT("NULL"));
	UE_LOG(LogTemp, Warning, TEXT("DefaultFalseClass: %s"), DefaultFalseClass ? *DefaultFalseClass->GetName() : TEXT("NULL"));
	UE_LOG(LogTemp, Warning, TEXT("DefaultLadyMedeClass: %s"), DefaultLadyMedeClass ? *DefaultLadyMedeClass->GetName() : TEXT("NULL"));
	UE_LOG(LogTemp, Warning, TEXT("DefaultWrathClass: %s"), DefaultWrathClass ? *DefaultWrathClass->GetName() : TEXT("NULL"));

	UE_LOG(LogTemp, Warning, TEXT("Starting to spawn characters..."));

	TArray<AMedievalCharacter*> PlayerChars;
	TArray<AMedievalCharacter*> EnemyChars;

	FActorSpawnParameters SpawnParams;
	SpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AdjustIfPossibleButAlwaysSpawn;

	// Spawn players (left side, elevated)
	FVector Player1Loc = FVector(-500.0f, -150.0f, 200.0f);
	FVector Player2Loc = FVector(-500.0f, 150.0f, 200.0f);

	if (DefaultKnightClass)
	{
		UE_LOG(LogTemp, Warning, TEXT("Attempting to spawn Knight..."));
		FTransform KnightTransform(FRotator::ZeroRotator, Player1Loc);
		AKnightCharacter* Knight = World->SpawnActor<AKnightCharacter>(DefaultKnightClass, KnightTransform, SpawnParams);
		if (Knight)
		{
			Knight->CharacterName = TEXT("Player Knight");
			PlayerChars.Add(Knight);
			UE_LOG(LogTemp, Warning, TEXT("✓ SUCCESS: Spawned Knight at %s"), *Player1Loc.ToString());
			DrawDebugSphere(World, Player1Loc, 100.0f, 12, FColor::Green, true, 30.0f);
		}
		else
		{
			UE_LOG(LogTemp, Error, TEXT("✗ FAILED: Could not spawn Knight!"));
		}
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("✗ DefaultKnightClass is NULL!"));
	}

	if (DefaultMageClass)
	{
		FTransform MageTransform(FRotator::ZeroRotator, Player2Loc);
		AMageCharacter* Mage = World->SpawnActor<AMageCharacter>(DefaultMageClass, MageTransform, SpawnParams);
		if (Mage)
		{
			Mage->CharacterName = TEXT("Player Mage");
			PlayerChars.Add(Mage);
			UE_LOG(LogTemp, Warning, TEXT("Spawned Mage at %s"), *Player2Loc.ToString());
			DrawDebugSphere(World, Player2Loc, 100.0f, 12, FColor::Blue, true, 30.0f);
		}
	}

	// Spawn enemies (right side, elevated)
	FVector FalseLoc = FVector(500.0f, -300.0f, 200.0f);
	FVector MedeLoc = FVector(500.0f, 0.0f, 200.0f);
	FVector WrathLoc = FVector(500.0f, 300.0f, 200.0f);

	if (DefaultFalseClass)
	{
		FTransform FalseTransform(FRotator::ZeroRotator, FalseLoc);
		AFalseCharacter* False = World->SpawnActor<AFalseCharacter>(DefaultFalseClass, FalseTransform, SpawnParams);
		if (False)
		{
			EnemyChars.Add(False);
			UE_LOG(LogTemp, Warning, TEXT("Spawned False at %s"), *FalseLoc.ToString());
			DrawDebugSphere(World, FalseLoc, 100.0f, 12, FColor::Red, true, 30.0f);
		}
	}

	if (DefaultLadyMedeClass)
	{
		FTransform MedeTransform(FRotator::ZeroRotator, MedeLoc);
		ALadyMedeCharacter* Mede = World->SpawnActor<ALadyMedeCharacter>(DefaultLadyMedeClass, MedeTransform, SpawnParams);
		if (Mede)
		{
			EnemyChars.Add(Mede);
			UE_LOG(LogTemp, Warning, TEXT("Spawned Lady Mede at %s"), *MedeLoc.ToString());
			DrawDebugSphere(World, MedeLoc, 100.0f, 12, FColor::Yellow, true, 30.0f);
		}
	}

	if (DefaultWrathClass)
	{
		FTransform WrathTransform(FRotator::ZeroRotator, WrathLoc);
		AWrathCharacter* Wrath = World->SpawnActor<AWrathCharacter>(DefaultWrathClass, WrathTransform, SpawnParams);
		if (Wrath)
		{
			EnemyChars.Add(Wrath);
			UE_LOG(LogTemp, Warning, TEXT("Spawned Wrath at %s"), *WrathLoc.ToString());
			DrawDebugSphere(World, WrathLoc, 100.0f, 12, FColor::Magenta, true, 30.0f);
		}
	}

	// Store spawned characters and start combat
	AutoSpawnedPlayers = PlayerChars;
	AutoSpawnedEnemies = EnemyChars;

	if (PlayerChars.Num() > 0 && EnemyChars.Num() > 0)
	{
		UE_LOG(LogTemp, Warning, TEXT("Auto-starting combat with %d players and %d enemies"), PlayerChars.Num(), EnemyChars.Num());
		FTimerHandle CombatTimer;
		GetWorld()->GetTimerManager().SetTimer(CombatTimer, this, &AKyndeBladeGameMode::DelayedStartCombat, 1.0f, false);
	}
	else
	{
		UE_LOG(LogTemp, Error, TEXT("Failed to spawn enough characters: %d players, %d enemies"), PlayerChars.Num(), EnemyChars.Num());
	}
}

void AKyndeBladeGameMode::DelayedStartCombat()
{
	StartCombatSequence(AutoSpawnedPlayers, AutoSpawnedEnemies);
}

void AKyndeBladeGameMode::SpawnTestCharacters()
{
	UE_LOG(LogTemp, Warning, TEXT("=== SpawnTestCharacters called from console ==="));
	AutoSpawnTestCharacters();
}
