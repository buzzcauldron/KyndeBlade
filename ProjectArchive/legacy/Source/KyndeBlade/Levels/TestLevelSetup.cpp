#include "TestLevelSetup.h"
#include "../Characters/MedievalCharacter.h"
#include "../Characters/KnightCharacter.h"
#include "../Characters/MageCharacter.h"
#include "../Characters/Enemies/FalseCharacter.h"
#include "../Characters/Enemies/LadyMedeCharacter.h"
#include "../Characters/Enemies/WrathCharacter.h"
#include "../Game/KyndeBladeGameMode.h"
#include "../Combat/TurnManager.h"
#include "Engine/Engine.h"
#include "DrawDebugHelpers.h"
#include "Components/StaticMeshComponent.h"
#include "UObject/ConstructorHelpers.h"
#include "Engine/StaticMesh.h"

ATestLevelSetup::ATestLevelSetup()
{
	PrimaryActorTick.bCanEverTick = false;
	
	// Create a visual component so the actor is visible in the editor
	VisualComponent = CreateDefaultSubobject<UStaticMeshComponent>(TEXT("VisualComponent"));
	RootComponent = VisualComponent;
	
	// Try to set a simple mesh (cube) so it's visible
	// If this fails, the actor will still work, just won't be visible
	static ConstructorHelpers::FObjectFinder<UStaticMesh> CubeMesh(TEXT("/Engine/BasicShapes/Cube"));
	if (CubeMesh.Succeeded())
	{
		VisualComponent->SetStaticMesh(CubeMesh.Object);
		VisualComponent->SetRelativeScale3D(FVector(0.5f, 0.5f, 0.5f));
	}
	
	// Default spawn locations for "Tower on the Toft" level
	// Enemies spawn at the base of the tower, spread out
	FalseSpawnLocation = FVector(500.0f, -300.0f, 0.0f);
	LadyMedeSpawnLocation = FVector(500.0f, 0.0f, 0.0f);
	WrathSpawnLocation = FVector(500.0f, 300.0f, 0.0f);

	// Set default classes
	FalseClass = AFalseCharacter::StaticClass();
	LadyMedeClass = ALadyMedeCharacter::StaticClass();
	WrathClass = AWrathCharacter::StaticClass();
	
	// Set default player classes
	PlayerKnightClass = AKnightCharacter::StaticClass();
	PlayerMageClass = AMageCharacter::StaticClass();
}

void ATestLevelSetup::BeginPlay()
{
	Super::BeginPlay();
	
	UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: BeginPlay called"));
	
	// Auto-spawn player characters if enabled and none are provided
	if (bAutoSpawnPlayers && PlayerCharacters.Num() == 0)
	{
		UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: Auto-spawning player characters"));
		SpawnPlayerCharacters();
		UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: Spawned %d player characters"), PlayerCharacters.Num());
	}
	
	// Auto-spawn enemies when level starts
	UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: Spawning enemies"));
	SpawnEnemies();
	UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: Spawned %d enemies"), EnemyCharacters.Num());
	
	// Auto-start combat after a short delay
	FTimerHandle TimerHandle;
	GetWorld()->GetTimerManager().SetTimer(TimerHandle, this, &ATestLevelSetup::StartCombat, 1.0f, false);
}

void ATestLevelSetup::SpawnPlayerCharacters()
{
	UWorld* World = GetWorld();
	if (!World)
	{
		UE_LOG(LogTemp, Error, TEXT("TestLevelSetup: No World available for spawning players"));
		return;
	}

	if (!PlayerKnightClass && !PlayerMageClass)
	{
		UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: No player classes set, using defaults"));
		PlayerKnightClass = AKnightCharacter::StaticClass();
		PlayerMageClass = AMageCharacter::StaticClass();
	}

	FActorSpawnParameters SpawnParams;
	SpawnParams.Owner = this;
	SpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AdjustIfPossibleButAlwaysSpawn;

	// Adjust spawn location to be above ground (add 100 units to Z)
	FVector Player1Location = Player1SpawnLocation + FVector(0, 0, 100.0f);
	FVector Player2Location = Player2SpawnLocation + FVector(0, 0, 100.0f);

	// Spawn Player 1 (Knight) if class is set
	if (PlayerKnightClass)
	{
		FTransform KnightTransform(FRotator::ZeroRotator, Player1Location);
		AKnightCharacter* Knight = World->SpawnActor<AKnightCharacter>(PlayerKnightClass, KnightTransform, SpawnParams);
		if (Knight)
		{
			Knight->CharacterName = TEXT("Player Knight");
			PlayerCharacters.Add(Knight);
			UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: Spawned Knight at %s"), *Player1Location.ToString());
			
			// Draw debug sphere to show spawn location
			DrawDebugSphere(World, Player1Location, 50.0f, 12, FColor::Green, true, 10.0f);
		}
		else
		{
			UE_LOG(LogTemp, Error, TEXT("TestLevelSetup: Failed to spawn Knight"));
		}
	}

	// Spawn Player 2 (Mage) if class is set
	if (PlayerMageClass)
	{
		FTransform MageTransform(FRotator::ZeroRotator, Player2Location);
		AMageCharacter* Mage = World->SpawnActor<AMageCharacter>(PlayerMageClass, MageTransform, SpawnParams);
		if (Mage)
		{
			Mage->CharacterName = TEXT("Player Mage");
			PlayerCharacters.Add(Mage);
			UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: Spawned Mage at %s"), *Player2Location.ToString());
			
			// Draw debug sphere to show spawn location
			DrawDebugSphere(World, Player2Location, 50.0f, 12, FColor::Blue, true, 10.0f);
		}
		else
		{
			UE_LOG(LogTemp, Error, TEXT("TestLevelSetup: Failed to spawn Mage"));
		}
	}
}

void ATestLevelSetup::SpawnEnemies()
{
	UWorld* World = GetWorld();
	if (!World)
	{
		UE_LOG(LogTemp, Error, TEXT("TestLevelSetup: No World available for spawning enemies"));
		return;
	}

	if (!FalseClass && !LadyMedeClass && !WrathClass)
	{
		UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: No enemy classes set, using defaults"));
		FalseClass = AFalseCharacter::StaticClass();
		LadyMedeClass = ALadyMedeCharacter::StaticClass();
		WrathClass = AWrathCharacter::StaticClass();
	}

	FActorSpawnParameters SpawnParams;
	SpawnParams.Owner = this;
	SpawnParams.SpawnCollisionHandlingOverride = ESpawnActorCollisionHandlingMethod::AdjustIfPossibleButAlwaysSpawn;

	// Adjust spawn locations to be above ground (add 100 units to Z)
	FVector FalseLocation = FalseSpawnLocation + FVector(0, 0, 100.0f);
	FVector MedeLocation = LadyMedeSpawnLocation + FVector(0, 0, 100.0f);
	FVector WrathLocation = WrathSpawnLocation + FVector(0, 0, 100.0f);

	// Spawn False
	if (FalseClass)
	{
		FTransform FalseTransform(FRotator::ZeroRotator, FalseLocation);
		FalseEnemy = World->SpawnActor<AFalseCharacter>(FalseClass, FalseTransform, SpawnParams);
		if (FalseEnemy)
		{
			EnemyCharacters.Add(FalseEnemy);
			UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: Spawned False at %s"), *FalseLocation.ToString());
			DrawDebugSphere(World, FalseLocation, 50.0f, 12, FColor::Red, true, 10.0f);
		}
		else
		{
			UE_LOG(LogTemp, Error, TEXT("TestLevelSetup: Failed to spawn False"));
		}
	}

	// Spawn Lady Mede
	if (LadyMedeClass)
	{
		FTransform MedeTransform(FRotator::ZeroRotator, MedeLocation);
		LadyMedeEnemy = World->SpawnActor<ALadyMedeCharacter>(LadyMedeClass, MedeTransform, SpawnParams);
		if (LadyMedeEnemy)
		{
			EnemyCharacters.Add(LadyMedeEnemy);
			UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: Spawned Lady Mede at %s"), *MedeLocation.ToString());
			DrawDebugSphere(World, MedeLocation, 50.0f, 12, FColor::Yellow, true, 10.0f);
		}
		else
		{
			UE_LOG(LogTemp, Error, TEXT("TestLevelSetup: Failed to spawn Lady Mede"));
		}
	}

	// Spawn Wrath
	if (WrathClass)
	{
		FTransform WrathTransform(FRotator::ZeroRotator, WrathLocation);
		WrathEnemy = World->SpawnActor<AWrathCharacter>(WrathClass, WrathTransform, SpawnParams);
		if (WrathEnemy)
		{
			EnemyCharacters.Add(WrathEnemy);
			UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: Spawned Wrath at %s"), *WrathLocation.ToString());
			DrawDebugSphere(World, WrathLocation, 50.0f, 12, FColor::Magenta, true, 10.0f);
		}
		else
		{
			UE_LOG(LogTemp, Error, TEXT("TestLevelSetup: Failed to spawn Wrath"));
		}
	}
}

void ATestLevelSetup::StartCombat()
{
	UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: StartCombat called - Players: %d, Enemies: %d"), PlayerCharacters.Num(), EnemyCharacters.Num());
	
	AKyndeBladeGameMode* GameMode = Cast<AKyndeBladeGameMode>(GetWorld()->GetAuthGameMode());
	if (!GameMode)
	{
		UE_LOG(LogTemp, Error, TEXT("TestLevelSetup: GameMode is not KyndeBladeGameMode! Current GameMode: %s"), 
			GetWorld()->GetAuthGameMode() ? *GetWorld()->GetAuthGameMode()->GetClass()->GetName() : TEXT("None"));
		return;
	}
	
	if (PlayerCharacters.Num() == 0)
	{
		UE_LOG(LogTemp, Error, TEXT("TestLevelSetup: No player characters to start combat!"));
		return;
	}
	
	if (EnemyCharacters.Num() == 0)
	{
		UE_LOG(LogTemp, Error, TEXT("TestLevelSetup: No enemy characters to start combat!"));
		return;
	}
	
	UE_LOG(LogTemp, Warning, TEXT("TestLevelSetup: Starting combat sequence"));
	GameMode->StartCombatSequence(PlayerCharacters, EnemyCharacters);
}
