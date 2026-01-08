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

	// Expedition 33-inspired: Kynde (Nature) - represents spiritual/natural strength
	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float MaxKynde = 10.0f; // Max Kynde

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float CurrentKynde = 0.0f; // Current Kynde

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
		MaxKynde = 10.0f;
		CurrentKynde = 0.0f;
		MaxBreakGauge = 100.0f;
		CurrentBreakGauge = 100.0f;
		Level = 1;
	}
};

DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FOnHealthChanged, float, NewHealth, float, MaxHealth);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FOnStaminaChanged, float, NewStamina, float, MaxStamina);
DECLARE_DYNAMIC_MULTICAST_DELEGATE_TwoParams(FOnKyndeChanged, float, NewKynde, float, MaxKynde);
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

	// Real-time combat state (Piers Plowman: Escapade/Ward)
	UPROPERTY(BlueprintReadOnly)
	bool bIsDodging = false; // Escapade (Kynde's Evasion)

	UPROPERTY(BlueprintReadOnly)
	bool bIsParrying = false; // Ward (Trewthe's Sheeld)

	UPROPERTY(BlueprintReadOnly)
	float DodgeWindowRemaining = 0.0f; // Escapade window

	UPROPERTY(BlueprintReadOnly)
	float ParryWindowRemaining = 0.0f; // Ward window

	// Delegates
	UPROPERTY(BlueprintAssignable)
	FOnHealthChanged OnHealthChanged;

	UPROPERTY(BlueprintAssignable)
	FOnStaminaChanged OnStaminaChanged;

	UPROPERTY(BlueprintAssignable)
	FOnCharacterDefeated OnCharacterDefeated;

	UPROPERTY(BlueprintAssignable)
	FOnKyndeChanged OnKyndeChanged;

	UPROPERTY(BlueprintAssignable)
	FOnBreakGaugeChanged OnBreakGaugeChanged;

	UPROPERTY(BlueprintAssignable)
	FOnCharacterBroken OnCharacterBroken;

	// Combat Functions
	UFUNCTION(BlueprintCallable)
	void TakeDamage(float Damage, AMedievalCharacter* Attacker);

	// Expedition 33-inspired: Kynde management
	UFUNCTION(BlueprintCallable)
	void GainKynde(float Amount);

	UFUNCTION(BlueprintCallable)
	bool ConsumeKynde(float Amount);

	UFUNCTION(BlueprintCallable)
	float GetCurrentKynde() const { return CurrentKynde; }

	UFUNCTION(BlueprintCallable)
	float GetMaxKynde() const { return MaxKynde; }

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

	// Real-time combat mechanics (Piers Plowman: Escapade/Ward)
	UFUNCTION(BlueprintCallable)
	void StartDodge(float WindowDuration); // Start Escapade (Kynde's Evasion)

	UFUNCTION(BlueprintCallable)
	void StartParry(float WindowDuration); // Start Ward (Trewthe's Sheeld)

	UFUNCTION(BlueprintCallable)
	void EndDodge(); // End Escapade

	UFUNCTION(BlueprintCallable)
	void EndParry(); // End Ward

	UFUNCTION(BlueprintCallable)
	bool AttemptDodge(); // Attempt Escapade

	UFUNCTION(BlueprintCallable)
	bool AttemptParry(); // Attempt Ward

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
