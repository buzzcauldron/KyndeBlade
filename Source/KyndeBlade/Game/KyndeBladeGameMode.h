#pragma once

#include "CoreMinimal.h"
#include "GameFramework/GameModeBase.h"
#include "Combat/TurnManager.h"
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

protected:
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "Combat")
	TSubclassOf<ATurnManager> TurnManagerClass;

	UFUNCTION()
	void SpawnTurnManager();
};
