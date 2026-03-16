#pragma once

#include "CoreMinimal.h"
#include "MedievalCharacter.h"
#include "ArcherCharacter.generated.h"

class UCombatAction;

/**
 * Archer character class - ranged physical damage, Quick Shot, Aimed Shot, Poison Arrow
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API AArcherCharacter : public AMedievalCharacter
{
	GENERATED_BODY()

public:
	AArcherCharacter(const FObjectInitializer& ObjectInitializer);

	virtual void BeginPlay() override;

	UFUNCTION(BlueprintCallable)
	void QuickShot(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void AimedShot(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void PoisonArrow(AMedievalCharacter* Target);

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float QuickShotDamage = 8.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float QuickShotStaminaCost = 5.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float AimedShotDamage = 25.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float AimedShotStaminaCost = 20.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float PoisonArrowDamage = 12.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float PoisonArrowStaminaCost = 15.0f;
};
