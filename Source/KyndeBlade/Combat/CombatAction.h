#pragma once

#include "CoreMinimal.h"
#include "UObject/NoExportTypes.h"
#include "CombatAction.generated.h"

UENUM(BlueprintType)
enum class ECombatActionType : uint8
{
	Attack,
	Dodge,
	Parry,
	Counter,
	Special,
	Wait
};

USTRUCT(BlueprintType)
struct FCombatActionData
{
	GENERATED_BODY()

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	ECombatActionType ActionType;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float Damage = 0.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float StaminaCost = 0.0f;

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	float SuccessWindow = 0.0f; // Time window for real-time mechanics (dodge/parry)

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FString ActionName;

	FCombatActionData()
	{
		ActionType = ECombatActionType::Wait;
		Damage = 0.0f;
		StaminaCost = 0.0f;
		SuccessWindow = 0.0f;
		ActionName = TEXT("Wait");
	}
};

/**
 * Base class for combat actions
 */
UCLASS(BlueprintType, Blueprintable)
class KYNDEBLADE_API UCombatAction : public UObject
{
	GENERATED_BODY()

public:
	UCombatAction();

	UPROPERTY(EditAnywhere, BlueprintReadWrite)
	FCombatActionData ActionData;

	UFUNCTION(BlueprintCallable)
	virtual void ExecuteAction(class AMedievalCharacter* Executor, class AMedievalCharacter* Target);

	UFUNCTION(BlueprintImplementableEvent)
	void OnActionExecuted(AMedievalCharacter* Executor, AMedievalCharacter* Target);

protected:
	UFUNCTION(BlueprintCallable)
	virtual bool CanExecute(AMedievalCharacter* Executor);
};
