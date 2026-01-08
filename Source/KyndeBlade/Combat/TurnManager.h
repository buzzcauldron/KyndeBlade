#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Actor.h"
#include "Characters/MedievalCharacter.h"
#include "TurnManager.generated.h"

DECLARE_DYNAMIC_MULTICAST_DELEGATE_OneParam(FOnTurnChanged, AMedievalCharacter*, CurrentCharacter);
DECLARE_DYNAMIC_MULTICAST_DELEGATE(FOnCombatEnded);

UENUM(BlueprintType)
enum class ECombatState : uint8
{
	WaitingForInput,
	ExecutingAction,
	RealTimeWindow,
	ProcessingResults,
	CombatEnded
};

/**
 * Manages turn-based combat with real-time mechanics
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API ATurnManager : public AActor
{
	GENERATED_BODY()
	
public:	
	ATurnManager();

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

	// Combat Setup
	UFUNCTION(BlueprintCallable)
	void InitializeCombat(const TArray<AMedievalCharacter*>& PlayerCharacters, const TArray<AMedievalCharacter*>& EnemyCharacters);

	UFUNCTION(BlueprintCallable)
	void AddCharacterToCombat(AMedievalCharacter* Character, bool bIsPlayer);

	// Turn Management
	UFUNCTION(BlueprintCallable)
	void StartCombat();

	UFUNCTION(BlueprintCallable)
	void EndTurn();

	UFUNCTION(BlueprintCallable)
	void NextTurn();

	UFUNCTION(BlueprintCallable)
	AMedievalCharacter* GetCurrentCharacter() const { return CurrentCharacter; }

	UFUNCTION(BlueprintCallable)
	int32 GetCurrentTurnNumber() const { return TurnNumber; }

	UFUNCTION(BlueprintCallable)
	ECombatState GetCombatState() const { return CombatState; }

	UFUNCTION(BlueprintCallable)
	bool IsPlayerTurn() const;

	UFUNCTION(BlueprintCallable)
	TArray<AMedievalCharacter*> GetPlayerCharacters() const { return PlayerCharacters; }

	UFUNCTION(BlueprintCallable)
	TArray<AMedievalCharacter*> GetEnemyCharacters() const { return EnemyCharacters; }

	// Action Execution
	UFUNCTION(BlueprintCallable)
	void ExecuteAction(UCombatAction* Action, AMedievalCharacter* Target);

	UFUNCTION(BlueprintCallable)
	void StartRealTimeWindow(float Duration);

	// Delegates
	UPROPERTY(BlueprintAssignable)
	FOnTurnChanged OnTurnChanged;

	UPROPERTY(BlueprintAssignable)
	FOnCombatEnded OnCombatEnded;

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	ECombatState CombatState;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	TArray<AMedievalCharacter*> PlayerCharacters;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	TArray<AMedievalCharacter*> EnemyCharacters;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	TArray<AMedievalCharacter*> TurnOrder;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	AMedievalCharacter* CurrentCharacter;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	int32 TurnNumber;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float RealTimeWindowDuration = 2.0f;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float RealTimeWindowRemaining = 0.0f;

	// Timer handle for delayed turn transitions
	FTimerHandle NextTurnTimerHandle;

	// Timing for action execution
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float ActionCastTimeRemaining = 0.0f;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	float ActionExecutionTimeRemaining = 0.0f;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	class UCombatAction* CurrentExecutingAction = nullptr;

	UPROPERTY(VisibleAnywhere, BlueprintReadOnly)
	AMedievalCharacter* CurrentActionTarget = nullptr;

	UFUNCTION()
	void ProcessActionCast();

	UFUNCTION()
	void ProcessActionExecution();

	UFUNCTION()
	void CalculateTurnOrder();

	UFUNCTION()
	void CheckCombatEnd();

	UFUNCTION()
	bool AreAllCharactersDefeated(const TArray<AMedievalCharacter*>& Characters) const;
};
