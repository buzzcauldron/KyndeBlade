#pragma once

#include "CoreMinimal.h"
#include "MedievalCharacter.h"
#include "RogueCharacter.generated.h"

/**
 * Rogue character class - Backstab, Evade (extended dodge), Critical Strike
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API ARogueCharacter : public AMedievalCharacter
{
	GENERATED_BODY()

public:
	ARogueCharacter(const FObjectInitializer& ObjectInitializer);

	virtual void BeginPlay() override;

	UFUNCTION(BlueprintCallable)
	void Backstab(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void Evade();

	UFUNCTION(BlueprintCallable)
	void CriticalStrike(AMedievalCharacter* Target);

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float BackstabDamageMultiplier = 2.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float BackstabStaminaCost = 25.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float EvadeStaminaCost = 15.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float EvadeWindowDuration = 2.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float CriticalStrikeChance = 0.25f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float CriticalStrikeMultiplier = 2.5f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float CriticalStrikeStaminaCost = 20.0f;
};
