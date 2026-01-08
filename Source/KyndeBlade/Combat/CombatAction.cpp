#include "Combat/CombatAction.h"
#include "Characters/MedievalCharacter.h"

UCombatAction::UCombatAction()
{
	ActionData = FCombatActionData();
}

void UCombatAction::ExecuteAction(AMedievalCharacter* Executor, AMedievalCharacter* Target)
{
	if (!CanExecute(Executor))
	{
		return;
	}

	// Consume stamina
	if (Executor)
	{
		Executor->ConsumeStamina(ActionData.StaminaCost);
	}

	// Execute the action based on type
	switch (ActionData.ActionType)
	{
	case ECombatActionType::Attack:
		if (Target)
		{
			Target->TakeDamage(ActionData.Damage, Executor);
		}
		break;
	case ECombatActionType::Dodge:
		// Dodge logic handled in real-time
		break;
	case ECombatActionType::Parry:
		// Parry logic handled in real-time
		break;
	case ECombatActionType::Counter:
		if (Target)
		{
			Target->TakeDamage(ActionData.Damage * 1.5f, Executor);
		}
		break;
	default:
		break;
	}

	OnActionExecuted(Executor, Target);
}

bool UCombatAction::CanExecute(AMedievalCharacter* Executor)
{
	if (!Executor)
	{
		return false;
	}

	return Executor->GetCurrentStamina() >= ActionData.StaminaCost;
}
