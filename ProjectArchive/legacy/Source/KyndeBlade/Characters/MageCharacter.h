#pragma once

#include "CoreMinimal.h"
#include "MedievalCharacter.h"
#include "MageCharacter.generated.h"

/**
 * Specialized Mage character class
 * High magic attack, low defense, spellcaster role
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API AMageCharacter : public AMedievalCharacter
{
	GENERATED_BODY()

public:
	AMageCharacter(const FObjectInitializer& ObjectInitializer);

	virtual void BeginPlay() override;

	// Mage-specific abilities
	UFUNCTION(BlueprintCallable)
	void Fireball(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void HealTarget(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void LightningBolt(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	float GetMana() const { return CurrentMana; }

	UFUNCTION(BlueprintCallable)
	float GetMaxMana() const { return MaxMana; }

protected:
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float MaxMana = 100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float CurrentMana = 100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float FireballDamage = 30.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float FireballManaCost = 25.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float HealAmount = 40.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float HealManaCost = 30.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float LightningBoltDamage = 35.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float LightningBoltManaCost = 35.0f;

	UFUNCTION(BlueprintCallable)
	void ConsumeMana(float Amount);
};
