#pragma once

#include "CoreMinimal.h"
#include "../MedievalCharacter.h"
#include "FalseCharacter.generated.h"

/**
 * False - The personification of falsehood and deception from Piers Plowman
 * A deceptive rogue-type enemy who uses trickery and backstabs
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API AFalseCharacter : public AMedievalCharacter
{
	GENERATED_BODY()

public:
	AFalseCharacter(const FObjectInitializer& ObjectInitializer);

	virtual void BeginPlay() override;

	// False-specific abilities (deceptive attacks)
	UFUNCTION(BlueprintCallable)
	void DeceptiveStrike(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void FeintAttack(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void ShadowStep(AMedievalCharacter* Target);

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float DeceptionDamageMultiplier = 1.5f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float FeintStaminaCost = 20.0f;

	UPROPERTY(BlueprintReadOnly)
	bool bIsFeinting = false;
};
