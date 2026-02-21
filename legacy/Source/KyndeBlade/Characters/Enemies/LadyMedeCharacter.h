#pragma once

#include "CoreMinimal.h"
#include "../MedievalCharacter.h"
#include "LadyMedeCharacter.generated.h"

/**
 * Lady Mede - The personification of bribery and corruption from Piers Plowman
 * A corrupt mage-type enemy who uses corrupting magic and gold-based attacks
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API ALadyMedeCharacter : public AMedievalCharacter
{
	GENERATED_BODY()

public:
	ALadyMedeCharacter(const FObjectInitializer& ObjectInitializer);

	virtual void BeginPlay() override;

	// Lady Mede-specific abilities (corruption magic)
	UFUNCTION(BlueprintCallable)
	void CorruptingGold(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void BriberyCharm(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void WealthBolt(AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	float GetCorruptionMana() const { return CurrentCorruptionMana; }

	UFUNCTION(BlueprintCallable)
	float GetMaxCorruptionMana() const { return MaxCorruptionMana; }

protected:
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float MaxCorruptionMana = 120.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float CurrentCorruptionMana = 120.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float CorruptingGoldDamage = 35.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float CorruptingGoldManaCost = 30.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float BriberyCharmManaCost = 40.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float WealthBoltDamage = 40.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float WealthBoltManaCost = 45.0f;

	UFUNCTION(BlueprintCallable)
	void ConsumeCorruptionMana(float Amount);
};
