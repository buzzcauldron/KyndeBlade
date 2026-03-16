#pragma once

#include "CoreMinimal.h"
#include "AIController.h"
#include "CombatAIController.generated.h"

class ATurnManager;
class AMedievalCharacter;
class UCombatAction;

/**
 * AI controller for enemy combat: when it's this character's turn, chooses an action and target
 * and calls TurnManager->ExecuteAction. Set as AIControllerClass on enemy characters.
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API ACombatAIController : public AAIController
{
	GENERATED_BODY()

public:
	ACombatAIController(const FObjectInitializer& ObjectInitializer);

	virtual void BeginPlay() override;
	virtual void Tick(float DeltaTime) override;

	/** Delay before executing chosen action (so player can see whose turn it is). */
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Combat AI")
	float ActionDelaySeconds = 0.8f;

protected:
	UPROPERTY(BlueprintReadOnly)
	ATurnManager* TurnManager = nullptr;

	/** Cached so we only choose once per turn. */
	bool bHasChosenThisTurn = false;

	void TryExecuteEnemyTurn();
	void ChooseAndExecuteAction();
};
