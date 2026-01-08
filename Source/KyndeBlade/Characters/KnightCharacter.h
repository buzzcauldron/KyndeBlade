#pragma once

#include "CoreMinimal.h"
#include "Characters/MedievalCharacter.h"
#include "KnightCharacter.generated.h"

/**
 * Specialized Knight character class
 * High defense, moderate attack, tank role
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API AKnightCharacter : public AMedievalCharacter
{
	GENERATED_BODY()

public:
	AKnightCharacter(const FObjectInitializer& ObjectInitializer);

	virtual void BeginPlay() override;

	// Knight-specific abilities
	UFUNCTION(BlueprintCallable)
	void ShieldBash(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void DefensiveStance();

	UFUNCTION(BlueprintCallable)
	void ChargeAttack(AMedievalCharacter* Target);

protected:
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float ShieldBashDamage = 20.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float DefensiveStanceDefenseBonus = 10.0f;

	UPROPERTY(BlueprintReadOnly)
	bool bInDefensiveStance = false;
};
