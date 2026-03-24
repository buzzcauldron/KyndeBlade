#pragma once

#include "CoreMinimal.h"
#include "Engine/GameInstance.h"
#include "KyndeBladeGameInstance.generated.h"

/**
 * Game instance for Kynde Blade - holds campaign state and level flow.
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API UKyndeBladeGameInstance : public UGameInstance
{
	GENERATED_BODY()

public:
	UFUNCTION(BlueprintCallable, Category = "Game Flow")
	void OpenCombatLevel(FName LevelName = FName(TEXT("Untitled")));

	UFUNCTION(BlueprintCallable, Category = "Game Flow")
	void OpenMainMenu();

	/** Current Passus (campaign chapter) for progression. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Campaign")
	int32 CurrentPassus = 0;

	/** Unlocked level indices or names. */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Campaign")
	TArray<FName> UnlockedLevels;
};
