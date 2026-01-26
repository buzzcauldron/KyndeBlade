#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "InputActionValue.h"
#include "CombatInputComponent.generated.h"

class UInputAction;
class UInputMappingContext;

DECLARE_DYNAMIC_MULTICAST_DELEGATE(FOnDodgeInput); // Escapade (Kynde's Evasion)
DECLARE_DYNAMIC_MULTICAST_DELEGATE(FOnParryInput); // Ward (Trewthe's Sheeld)
DECLARE_DYNAMIC_MULTICAST_DELEGATE(FOnCounterInput); // Counter-strike

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

	// Input Actions (Piers Plowman terminology)
	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Input")
	UInputAction* DodgeAction; // Escapade (Kynde's Evasion)

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Input")
	UInputAction* ParryAction; // Ward (Trewthe's Sheeld)

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Input")
	UInputAction* CounterAction; // Counter-strike

	UPROPERTY(EditAnywhere, BlueprintReadOnly, Category = "Input")
	UInputMappingContext* CombatMappingContext;

	// Delegates
	UPROPERTY(BlueprintAssignable)
	FOnDodgeInput OnDodgeInput;

	UPROPERTY(BlueprintAssignable)
	FOnParryInput OnParryInput;

	UPROPERTY(BlueprintAssignable)
	FOnCounterInput OnCounterInput;

	// Input Handlers (Piers Plowman terminology)
	UFUNCTION(BlueprintCallable)
	void HandleDodge(); // Handle Escapade (Kynde's Evasion)

	UFUNCTION(BlueprintCallable)
	void HandleParry(); // Handle Ward (Trewthe's Sheeld)

	UFUNCTION(BlueprintCallable)
	void HandleCounter(); // Handle Counter-strike

	UPROPERTY(BlueprintReadOnly)
	bool bRealTimeWindowActive = false;

	UFUNCTION(BlueprintCallable)
	void SetRealTimeWindowActive(bool bActive) { bRealTimeWindowActive = bActive; }
};
