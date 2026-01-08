#pragma once

#include "CoreMinimal.h"
#include "Characters/MedievalCharacter.h"
#include "Combat/StatusEffect.h"
#include "HungerCharacter.generated.h"

/**
 * Hunger - A mid-game boss from Piers Plowman
 * In the poem, Hunger is a personification that forces people to work, representing the harsh reality of poverty.
 * Hunger is the worst status effect and this boss embodies that - it applies hunger to the party and feeds on their suffering.
 * 
 * Visual Design (Alan Lee-inspired):
 * - Ethereal, gaunt figure - skeletal but not undead, more like a personification of want
 * - Wispy, translucent form that seems to fade in and out
 * - Dark, shadowy colors with hints of earth tones (browns, grays, deep purples)
 * - Surrounded by faint, hungry shadows that reach out like grasping hands
 * - Eyes that seem hollow but watchful - the eyes of someone who has known true hunger
 * - Clothes tattered and worn, like those of the poorest in medieval society
 * - Environment around Hunger should feel desolate, empty, like a barren field after harvest
 * - Atmospheric lighting - dim, with long shadows
 * - Detailed textures showing the effects of hunger on the body
 * - Mystical, otherworldly quality while remaining grounded in medieval reality
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API AHungerCharacter : public AMedievalCharacter
{
	GENERATED_BODY()

public:
	AHungerCharacter(const FObjectInitializer& ObjectInitializer);

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

	// Hunger-specific abilities (drawn from Piers Plowman)
	
	/**
	 * "Hunger's Grip" - From Piers Plowman: Hunger forces people to work
	 * Applies hunger status effect to all enemies, making them weaker
	 */
	UFUNCTION(BlueprintCallable)
	void HungersGrip(const TArray<AMedievalCharacter*>& Targets);

	/**
	 * "The Empty Belly" - From Piers Plowman: The physical reality of hunger
	 * Drains stamina and Kynde from all enemies, feeding Hunger
	 */
	UFUNCTION(BlueprintCallable)
	void TheEmptyBelly(const TArray<AMedievalCharacter*>& Targets);

	/**
	 * "The Barren Field" - From Piers Plowman: Hunger comes from lack of food
	 * Creates an area effect that applies hunger over time
	 */
	UFUNCTION(BlueprintCallable)
	void TheBarrenField();

	/**
	 * "The Work That Never Ends" - From Piers Plowman: Hunger forces endless labor
	 * Applies hunger and forces enemies to consume more stamina
	 */
	UFUNCTION(BlueprintCallable)
	void TheWorkThatNeverEnds(AMedievalCharacter* Target);

	/**
	 * "The Feast of Want" - From Piers Plowman: Hunger feeds on the hungry
	 * Heals Hunger based on how many enemies are hungry
	 */
	UFUNCTION(BlueprintCallable)
	void TheFeastOfWant();

	/**
	 * "The Unending Need" - From Piers Plowman: Hunger is never satisfied
	 * Stacks hunger on all enemies, making it worse
	 */
	UFUNCTION(BlueprintCallable)
	void TheUnendingNeed(const TArray<AMedievalCharacter*>& Targets);

	UFUNCTION(BlueprintCallable)
	int32 GetHungryEnemyCount() const;

	UFUNCTION(BlueprintCallable)
	float GetHungerPower() const { return HungerPower; }

protected:
	// Hunger's power grows as more enemies become hungry
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float HungerPower = 1.0f; // Multiplier based on hungry enemies

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float MaxHungerPower = 3.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float HungersGripStaminaCost = 40.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float TheEmptyBellyStaminaCost = 50.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float TheBarrenFieldStaminaCost = 60.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float TheWorkThatNeverEndsStaminaCost = 35.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float TheFeastOfWantStaminaCost = 45.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float TheUnendingNeedStaminaCost = 55.0f;

	// Area effect for The Barren Field
	UPROPERTY(BlueprintReadOnly)
	bool bBarrenFieldActive = false;

	UPROPERTY(BlueprintReadOnly)
	float BarrenFieldDuration = 0.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float BarrenFieldMaxDuration = 10.0f;

	UFUNCTION()
	void UpdateHungerPower();

	UFUNCTION()
	void UpdateBarrenField(float DeltaTime);
};
