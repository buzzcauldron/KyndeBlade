#pragma once

#include "CoreMinimal.h"
#include "UObject/NoExportTypes.h"
#include "StatusEffect.generated.h"

/**
 * Status effects in Piers Plowman - representing various afflictions and conditions
 * Hunger is the worst status effect, representing poverty and the struggle for sustenance
 */
UENUM(BlueprintType)
enum class EStatusEffectType : uint8
{
	None,
	Hunger,      // The worst status effect - Piers Plowman: "Hunger" (poverty, starvation)
	Frost,       // Cold - slows movement
	Burning,     // Fire damage over time
	Poison,      // Damage over time
	Stun,        // Cannot act
	Slow,        // Reduced speed
	Weak,        // Reduced attack power
	Vulnerable,  // Reduced defense
	Blessed,     // Positive: Increased stats
	KyndeBoost   // Positive: Increased Kynde generation
};

USTRUCT(BlueprintType)
struct FStatusEffectData
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	EStatusEffectType EffectType = EStatusEffectType::None;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float Duration = 0.0f; // How long the effect lasts (0 = permanent until removed)

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float RemainingTime = 0.0f; // Time remaining

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	int32 StackCount = 1; // How many stacks (for stackable effects)

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float DamagePerSecond = 0.0f; // Damage over time

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float StatModifier = 1.0f; // Multiplier for stats (1.0 = no change, 0.5 = 50% reduction)

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float AttackPowerModifier = 1.0f; // Attack power multiplier

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float DefenseModifier = 1.0f; // Defense multiplier

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float SpeedModifier = 1.0f; // Speed multiplier

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float StaminaRegenModifier = 1.0f; // Stamina regeneration multiplier

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float KyndeGenerationModifier = 1.0f; // Kynde generation multiplier

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FString EffectName; // Display name

	FStatusEffectData()
	{
		EffectType = EStatusEffectType::None;
		Duration = 0.0f;
		RemainingTime = 0.0f;
		StackCount = 1;
		DamagePerSecond = 0.0f;
		StatModifier = 1.0f;
		AttackPowerModifier = 1.0f;
		DefenseModifier = 1.0f;
		SpeedModifier = 1.0f;
		StaminaRegenModifier = 1.0f;
		KyndeGenerationModifier = 1.0f;
		EffectName = TEXT("");
	}
};

/**
 * Status Effect class - manages individual status effects
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API UStatusEffect : public UObject
{
	GENERATED_BODY()

public:
	UStatusEffect();

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FStatusEffectData EffectData;

	UFUNCTION(BlueprintCallable)
	void UpdateEffect(float DeltaTime);

	UFUNCTION(BlueprintCallable)
	bool IsExpired() const;

	UFUNCTION(BlueprintCallable)
	void ApplyEffect(class AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void RemoveEffect(class AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void StackEffect(int32 AdditionalStacks);

	// Static factory methods for common effects
	UFUNCTION(BlueprintCallable, Category = "Status Effects")
	static UStatusEffect* CreateHungerEffect(float Duration = 0.0f, int32 Stacks = 1);

	UFUNCTION(BlueprintCallable, Category = "Status Effects")
	static UStatusEffect* CreateFrostEffect(float Duration, float SlowAmount = 0.5f);

	UFUNCTION(BlueprintCallable, Category = "Status Effects")
	static UStatusEffect* CreateBurningEffect(float Duration, float DamagePerSecond);

	UFUNCTION(BlueprintCallable, Category = "Status Effects")
	static UStatusEffect* CreatePoisonEffect(float Duration, float DamagePerSecond);

	UFUNCTION(BlueprintCallable, Category = "Status Effects")
	static UStatusEffect* CreateStunEffect(float Duration);

	// Apply damage over time
	UFUNCTION(BlueprintCallable)
	void ApplyDamageOverTime(AMedievalCharacter* Target, float DeltaTime);

protected:
	// Apply stat modifications to character
	void ApplyStatModifiers(AMedievalCharacter* Target, bool bApply);
};
