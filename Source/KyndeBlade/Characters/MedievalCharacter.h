#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "Combat/CombatAction.h"
#include "MedievalCharacter.generated.h"

UENUM(BlueprintType)
enum class ECharacterClass : uint8
{
	Knight,
	Mage,
	Archer,
	Rogue
};

USTRUCT(BlueprintType)
struct FCharacterStats
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float MaxHealth = 100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float CurrentHealth = 100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float MaxStamina = 100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float CurrentStamina = 100.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float AttackPower = 10.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float Defense = 5.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float Speed = 10.0f; // Affects turn order

	// Expedition 33-inspired: Ability Points (called Virtue Points in medieval theme)
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float MaxVirtuePoints = 10.0f; // Max AP (Virtue Points)

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float CurrentVirtuePoints = 0.0f; // Current AP

	// Break system (Expedition 33-inspired)
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float MaxBreakGauge = 100.0f; // Break gauge maximum

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float CurrentBreakGauge = 100.0f; // Current break gauge

	UPROPERTY(BlueprintReadOnly)
	bool bIsBroken = false; // Is the character in broken state?

	UPROPERTY(BlueprintReadOnly)
	float BrokenStunRemaining = 0.0f; // Time remaining in broken state

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	int32 Level = 1;

	FCharacterStats()
	{
		MaxHealth = 100.0f;
		CurrentHealth = 100.0f;
		MaxStamina = 100.0f;
		CurrentStamina = 100.0f;
		AttackPower = 10.0f;
		Defense = 5.0f;
		Speed = 10.0f;
		MaxVirtuePoints = 10.0f;
		CurrentVirtuePoints = 0.0f;
		MaxBreakGauge = 100.0f;
		CurrentBreakGauge = 100.0f;
		Level = 1;
	}
};

DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FOnHealthChanged, float, NewHealth, float, MaxHealth);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FOnStaminaChanged, float, NewStamina, float, MaxStamina);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FOnVirtuePointsChanged, float, NewVP, float, MaxVP);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FOnBreakGaugeChanged, float, NewBreak, float, MaxBreak);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnCharacterDefeated, AMedievalCharacter*, Character);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnCharacterBroken, AMedievalCharacter*, Character);

/**
 * Base character class for medieval-themed combat
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API AMedievalCharacter : public ACharacter
{
	GENERATED_BODY()

public:
	AMedievalCharacter(const FObjectInitializer& ObjectInitializer);

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

	// Character Properties
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	ECharacterClass CharacterClass;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FCharacterStats Stats;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FString CharacterName;

	// Combat Actions
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	TArray<UCombatAction*> AvailableActions;

	// Real-time combat state
	UPROPERTY(BlueprintReadOnly)
	bool bIsDodging = false;

	UPROPERTY(BlueprintReadOnly)
	bool bIsParrying = false;

	UPROPERTY(BlueprintReadOnly)
	float DodgeWindowRemaining = 0.0f;

	UPROPERTY(BlueprintReadOnly)
	float ParryWindowRemaining = 0.0f;

	// Delegates
	UPROPERTY(BlueprintAssignable)
	FOnHealthChanged OnHealthChanged;

	UPROPERTY(BlueprintAssignable)
	FOnStaminaChanged OnStaminaChanged;

	UPROPERTY(BlueprintAssignable)
	FOnCharacterDefeated OnCharacterDefeated;

	UPROPERTY(BlueprintAssignable)
	FOnVirtuePointsChanged OnVirtuePointsChanged;

	UPROPERTY(BlueprintAssignable)
	FOnBreakGaugeChanged OnBreakGaugeChanged;

	UPROPERTY(BlueprintAssignable)
	FOnCharacterBroken OnCharacterBroken;

	// Combat Functions
	UFUNCTION(BlueprintCallable)
	void TakeDamage(float Damage, AMedievalCharacter* Attacker);

	// Expedition 33-inspired: Virtue Points (AP) management
	UFUNCTION(BlueprintCallable)
	void GainVirtuePoints(float Amount);

	UFUNCTION(BlueprintCallable)
	bool ConsumeVirtuePoints(float Amount);

	UFUNCTION(BlueprintCallable)
	float GetCurrentVirtuePoints() const { return CurrentVirtuePoints; }

	UFUNCTION(BlueprintCallable)
	float GetMaxVirtuePoints() const { return MaxVirtuePoints; }

	// Expedition 33-inspired: Break system
	UFUNCTION(BlueprintCallable)
	void TakeBreakDamage(float BreakAmount);

	UFUNCTION(BlueprintCallable)
	void BreakCharacter(); // Called when break gauge reaches zero

	UFUNCTION(BlueprintCallable)
	void RecoverFromBreak(); // Called when broken state ends

	UFUNCTION(BlueprintCallable)
	float GetCurrentBreakGauge() const { return CurrentBreakGauge; }

	UFUNCTION(BlueprintCallable)
	bool IsBroken() const { return bIsBroken; }

	UFUNCTION(BlueprintCallable)
	void Heal(float Amount);

	UFUNCTION(BlueprintCallable)
	void ConsumeStamina(float Amount);

	UFUNCTION(BlueprintCallable)
	void RestoreStamina(float Amount);

	UFUNCTION(BlueprintCallable)
	float GetCurrentHealth() const { return Stats.CurrentHealth; }

	UFUNCTION(BlueprintCallable)
	float GetMaxHealth() const { return Stats.MaxHealth; }

	UFUNCTION(BlueprintCallable)
	float GetCurrentStamina() const { return Stats.CurrentStamina; }

	UFUNCTION(BlueprintCallable)
	float GetMaxStamina() const { return Stats.MaxStamina; }

	UFUNCTION(BlueprintCallable)
	bool IsAlive() const { return Stats.CurrentHealth > 0.0f; }

	// Real-time combat mechanics
	UFUNCTION(BlueprintCallable)
	void StartDodge(float WindowDuration);

	UFUNCTION(BlueprintCallable)
	void StartParry(float WindowDuration);

	UFUNCTION(BlueprintCallable)
	void EndDodge();

	UFUNCTION(BlueprintCallable)
	void EndParry();

	UFUNCTION(BlueprintCallable)
	bool AttemptDodge();

	UFUNCTION(BlueprintCallable)
	bool AttemptParry();

	// Turn-based functions
	UFUNCTION(BlueprintCallable)
	void ExecuteCombatAction(UCombatAction* Action, AMedievalCharacter* Target);

	UFUNCTION(BlueprintImplementableEvent)
	void OnTurnStart();

	UFUNCTION(BlueprintImplementableEvent)
	void OnTurnEnd();

protected:
	virtual void UpdateRealTimeCombat(float DeltaTime);

private:
	void CheckDefeated();
};
