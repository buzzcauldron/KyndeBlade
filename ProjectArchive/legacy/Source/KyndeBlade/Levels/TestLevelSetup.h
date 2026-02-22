#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Components/StaticMeshComponent.h"
#include "../Characters/MedievalCharacter.h"
#include "../Characters/Enemies/FalseCharacter.h"
#include "../Characters/Enemies/LadyMedeCharacter.h"
#include "../Characters/Enemies/WrathCharacter.h"
#include "TestLevelSetup.generated.h"

/**
 * Helper actor to set up the test level "Tower on the Toft" with Piers Plowman enemies
 * The Tower on the Toft represents Truth and the abode of God from Piers Plowman
 * Place this in the level and configure it to spawn enemies
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API ATestLevelSetup : public AActor
{
	GENERATED_BODY()
	
public:	
	ATestLevelSetup();

	virtual void BeginPlay() override;

	// Player Characters
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Setup")
	TArray<AMedievalCharacter*> PlayerCharacters;

	// Auto-spawn player characters if PlayerCharacters array is empty
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Setup")
	bool bAutoSpawnPlayers = true;

	// Player Spawn Points (used if auto-spawning)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Setup")
	FVector Player1SpawnLocation = FVector(-500.0f, -150.0f, 0.0f);

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Setup")
	FVector Player2SpawnLocation = FVector(-500.0f, 150.0f, 0.0f);

	// Player Character Classes (used if auto-spawning)
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Setup")
	TSubclassOf<class AKnightCharacter> PlayerKnightClass;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Setup")
	TSubclassOf<class AMageCharacter> PlayerMageClass;

	// Enemy Spawn Points
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Enemies")
	FVector FalseSpawnLocation;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Enemies")
	FVector LadyMedeSpawnLocation;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Enemies")
	FVector WrathSpawnLocation;

	// Enemy Classes
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Enemies")
	TSubclassOf<AFalseCharacter> FalseClass;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Enemies")
	TSubclassOf<ALadyMedeCharacter> LadyMedeClass;

	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Enemies")
	TSubclassOf<AWrathCharacter> WrathClass;

	// Functions
	UFUNCTION(BlueprintCallable)
	void SpawnEnemies();

	UFUNCTION(BlueprintCallable)
	void SpawnPlayerCharacters();

	UFUNCTION(BlueprintCallable)
	void StartCombat();

	UFUNCTION(BlueprintCallable)
	TArray<AMedievalCharacter*> GetEnemyCharacters() const { return EnemyCharacters; }

	UFUNCTION(BlueprintCallable)
	TArray<AMedievalCharacter*> GetPlayerCharacters() const { return PlayerCharacters; }

	// Visual component so actor is visible in editor
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Components")
	UStaticMeshComponent* VisualComponent;

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	TArray<AMedievalCharacter*> EnemyCharacters;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	AFalseCharacter* FalseEnemy;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	ALadyMedeCharacter* LadyMedeEnemy;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	AWrathCharacter* WrathEnemy;
};
