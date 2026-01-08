#pragma once

#include "CoreMinimal.h"
#include "Characters/MedievalCharacter.h"
#include "WrathCharacter.generated.h"

/**
 * Wrath - One of the Seven Deadly Sins from Piers Plowman
 * A berserker warrior-type enemy who becomes stronger as he takes damage
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API AWrathCharacter : public AMedievalCharacter
{
	GENERATED_BODY()

public:
	AWrathCharacter(const FObjectInitializer& ObjectInitializer);

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

	// Wrath-specific abilities (rage-based attacks)
	UFUNCTION(BlueprintCallable)
	void RageStrike(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void BerserkerRage();

	UFUNCTION(BlueprintCallable)
	void FuriousCharge(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	float GetRageLevel() const { return RageLevel; }

protected:
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float RageLevel = 0.0f; // Increases as health decreases

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float MaxRageLevel = 100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float RageDamageMultiplier = 1.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float RageStrikeStaminaCost = 30.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float BerserkerRageStaminaCost = 50.0f;

	UPROPERTY(BlueprintReadOnly)
	bool bIsBerserk = false;

	UFUNCTION()
	void UpdateRage();
};
