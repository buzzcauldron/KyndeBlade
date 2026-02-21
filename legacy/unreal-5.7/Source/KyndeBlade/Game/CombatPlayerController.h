#pragma once

#include "CoreMinimal.h"
#include "GameFramework/PlayerController.h"
#include "CombatPlayerController.generated.h"

class UCombatInputComponent;
class ATurnManager;
class AMedievalCharacter;
class UCombatUI;

/**
 * Player controller for combat: wires CombatInputComponent to TurnManager real-time windows
 * and can display Combat UI. Set as default in Game Mode (or use Blueprint BP_CombatPlayerController).
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API ACombatPlayerController : public APlayerController
{
	GENERATED_BODY()

public:
	ACombatPlayerController(const FObjectInitializer& ObjectInitializer);

	virtual void BeginPlay() override;
	virtual void SetupInputComponent() override;

	UFUNCTION(BlueprintCallable)
	ATurnManager* GetTurnManager() const { return TurnManager; }

	UFUNCTION(BlueprintCallable)
	UCombatInputComponent* GetCombatInputComponent() const { return CombatInputComponent; }

	/** Widget class to create and add to viewport (e.g. WBP_CombatUI). */
	UPROPERTY(EditDefaultsOnly, BlueprintReadOnly, Category = "UI")
	TSubclassOf<UCombatUI> CombatUIClass;

	UPROPERTY(BlueprintReadOnly)
	UCombatUI* CombatUIWidget = nullptr;

protected:
	UPROPERTY(VisibleAnywhere, BlueprintReadOnly, Category = "Combat")
	UCombatInputComponent* CombatInputComponent;

	UPROPERTY(BlueprintReadOnly)
	ATurnManager* TurnManager = nullptr;

	/** Character who currently has the real-time window (defender or executor). */
	UPROPERTY(BlueprintReadOnly)
	AMedievalCharacter* RealTimeWindowCharacter = nullptr;

	UFUNCTION()
	void OnRealTimeWindowStarted(AMedievalCharacter* CharacterWithWindow);

	UFUNCTION()
	void OnRealTimeWindowEnded();

	UFUNCTION()
	void OnDodgePressed();

	UFUNCTION()
	void OnParryPressed();

	UFUNCTION()
	void OnCounterPressed();

	UFUNCTION()
	void OnTurnChangedForUI(AMedievalCharacter* CurrentCharacter);

	UFUNCTION()
	void OnCombatEndedForUI();

	void BindToTurnManager();
	void CreateCombatUI();
};
