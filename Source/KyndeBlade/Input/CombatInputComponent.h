#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "InputActionValue.h"
#include "CombatInputComponent.generated.h"

class UInputAction;
class UInputMappingContext;

DECLARE_DYNAMIC_MULTICAST_DELEGATE(FOnDodgeInput);
DECLARE_DYNAMIC_MULTICAST_DELEGATE(FOnParryInput);
DECLARE_DYNAMIC_MULTICAST_DELEGATE(FOnCounterInput);

/**
 * Component for handling real-time combat input during turn-based combat
 */
UCLASS(BlueprintType, Blueprintable, ClassGroup=(Custom), meta=(BlueprintSpawnableComponent))
class KYNDEBLADE_API UCombatInputComponent : public UActorComponent
{
	GENERATED_BODY()

public:
	UCombatInputComponent(const FObjectInitializer& ObjectInitializer);

	virtual void BeginPlay() override;
	
	// Call this from the owning Pawn's SetupPlayerInputComponent
	UFUNCTION(BlueprintCallable)
	void SetupInputBindings(class UInputComponent* PlayerInputComponent);

	// Input Actions
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Input")
	UInputAction* DodgeAction;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Input")
	UInputAction* ParryAction;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Input")
	UInputAction* CounterAction;

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Input")
	UInputMappingContext* CombatMappingContext;

	// Delegates
	UPROPERTY(BlueprintAssignable)
	FOnDodgeInput OnDodgeInput;

	UPROPERTY(BlueprintAssignable)
	FOnParryInput OnParryInput;

	UPROPERTY(BlueprintAssignable)
	FOnCounterInput OnCounterInput;

	// Input Handlers
	UFUNCTION(BlueprintCallable)
	void HandleDodge();

	UFUNCTION(BlueprintCallable)
	void HandleParry();

	UFUNCTION(BlueprintCallable)
	void HandleCounter();

	UPROPERTY(BlueprintReadOnly)
	bool bRealTimeWindowActive = false;

	UFUNCTION(BlueprintCallable)
	void SetRealTimeWindowActive(bool bActive) { bRealTimeWindowActive = bActive; }
};
