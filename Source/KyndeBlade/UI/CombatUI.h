#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "../Combat/CombatAction.h"
#include "../Combat/TurnManager.h"
#include "../Characters/MedievalCharacter.h"
#include "CombatUI.generated.h"

/**
 * Main UI widget for combat interface
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API UCombatUI : public UUserWidget
{
	GENERATED_BODY()

public:
	UCombatUI(const FObjectInitializer& ObjectInitializer);

	virtual void NativeConstruct() override;

	// UI Update Functions
	UFUNCTION(BlueprintCallable)
	void UpdateCharacterInfo(AMedievalCharacter* Character);

	UFUNCTION(BlueprintCallable)
	void UpdateTurnInfo(AMedievalCharacter* CurrentCharacter, int32 TurnNumber);

	UFUNCTION(BlueprintCallable)
	void ShowActionMenu(bool bShow);

	UFUNCTION(BlueprintCallable)
	void ShowRealTimeWindow(float Duration);

	// Action Selection
	UFUNCTION(BlueprintCallable)
	void OnActionSelected(UCombatAction* Action);

	UFUNCTION(BlueprintCallable)
	void OnTargetSelected(AMedievalCharacter* Target);

	// Events
	UFUNCTION(BlueprintImplementableEvent)
	void OnHealthChanged(float CurrentHealth, float MaxHealth);

	UFUNCTION(BlueprintImplementableEvent)
	void OnStaminaChanged(float CurrentStamina, float MaxStamina);

	UFUNCTION(BlueprintImplementableEvent)
	void OnTurnChanged(AMedievalCharacter* Character);

	UFUNCTION(BlueprintImplementableEvent)
	void OnCombatEnded(bool bPlayerVictory);

protected:
	UPROPERTY(BlueprintReadOnly)
	AMedievalCharacter* SelectedCharacter;

	UPROPERTY(BlueprintReadOnly)
	UCombatAction* SelectedAction;

	UPROPERTY(BlueprintReadOnly)
	AMedievalCharacter* SelectedTarget;
};
