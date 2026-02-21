#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "../Combat/TurnManager.h"
#include "../Characters/MedievalCharacter.h"
#include "KyndeBladeGameMode.generated.h"

/**
 * Main game mode for Kynde Blade
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API AKyndeBladeGameMode : public AGameModeBase
{
	GENERATED_BODY()

public:
	AKyndeBladeGameMode();

	virtual void BeginPlay() override;

	// Combat Management
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	ATurnManager* TurnManager;

	UFUNCTION(BlueprintCallable)
	void StartCombatSequence(const TArray<AMedievalCharacter*>& PlayerCharacters, const TArray<AMedievalCharacter*>& EnemyCharacters);

	UFUNCTION(BlueprintCallable)
	ATurnManager* GetTurnManager() const { return TurnManager; }

	// Auto-spawn test characters for quick testing
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Auto Setup")
	bool bAutoSpawnTestCharacters = true;

	UFUNCTION(BlueprintCallable)
	void AutoSpawnTestCharacters();

	UFUNCTION()
	void DelayedStartCombat();

	// Console command to manually spawn characters
	UFUNCTION(Exec)
	void SpawnTestCharacters();

protected:
	// Temporary storage for auto-spawned characters
	UPROPERTY()
	TArray<AMedievalCharacter*> AutoSpawnedPlayers;

	UPROPERTY()
	TArray<AMedievalCharacter*> AutoSpawnedEnemies;

protected:
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Combat")
	TSubclassOf<ATurnManager> TurnManagerClass;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Auto Setup")
	TSubclassOf<class AKnightCharacter> DefaultKnightClass;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Auto Setup")
	TSubclassOf<class AMageCharacter> DefaultMageClass;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Auto Setup")
	TSubclassOf<class AFalseCharacter> DefaultFalseClass;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Auto Setup")
	TSubclassOf<class ALadyMedeCharacter> DefaultLadyMedeClass;

	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Auto Setup")
	TSubclassOf<class AWrathCharacter> DefaultWrathClass;

	UFUNCTION()
	void SpawnTurnManager();
};
