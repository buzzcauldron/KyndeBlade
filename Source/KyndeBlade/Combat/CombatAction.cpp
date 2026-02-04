#include "CombatAction.h"
#include "../Characters/MedievalCharacter.h"

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

	// Expedition 33-inspired: Check Kynde cost before executing
	if (ActionData.KyndeCost > 0.0f)
	{
		if (!Executor->ConsumeKynde(ActionData.KyndeCost))
		{
			return; // Not enough Kynde, action fails
		}
	}

	if (Executor)
	{
		Executor->ConsumeStamina(ActionData.StaminaCost);
		if (ActionData.ManaCost > 0.0f)
		{
			Executor->ConsumeMana(ActionData.ManaCost);
		}
	}

	// Expedition 33-inspired: Generate Kynde from melee attacks
	if (ActionData.KyndeGenerated > 0.0f && Executor)
	{
		Executor->GainKynde(ActionData.KyndeGenerated);
	}

	// Execute the action based on type (Piers Plowman terminology)
	switch (ActionData.ActionType)
	{
	case ECombatActionType::Strike: // Attack - "Sothfastnesse's Stroke"
		if (Target)
		{
			float FinalDamage = ActionData.Damage;

			// Expedition 33-inspired: Broken enemies take 50% more damage
			if (Target->IsBroken())
			{
				FinalDamage *= 1.5f;
			}

			Target->ApplyCustomDamage(FinalDamage, Executor);

			// Expedition 33-inspired: Apply break damage
			if (ActionData.BreakDamage > 0.0f && Target)
			{
				Target->TakeBreakDamage(ActionData.BreakDamage);
			}
		}
		break;
	case ECombatActionType::Escapade: // Dodge - "Kynde's Evasion"
		// Escapade logic handled in real-time
		break;
	case ECombatActionType::Ward: // Parry - "Trewthe's Sheeld"
		// Ward logic handled in real-time
		break;
	case ECombatActionType::Counter: // Counter-attack
		if (Target)
		{
			Target->ApplyCustomDamage(ActionData.Damage * 1.5f, Executor);
		}
		break;
	case ECombatActionType::Guard:
		if (Executor)
		{
			Executor->SetGuarding(true);
			Executor->RestoreStamina(15.0f);
		}
		break;
	case ECombatActionType::Rest: // Wait - "Kynde's Rest"
		if (Executor)
		{
			Executor->RestoreStamina(20.0f);
		}
		break;
	default:
		break;
	}

	OnActionExecuted(Executor, Target);
}

void UCombatAction::ExecuteActionWithDeferredDamage(AMedievalCharacter* Executor, AMedievalCharacter* Target)
{
	if (!CanExecute(Executor))
	{
		return;
	}

	if (ActionData.KyndeCost > 0.0f)
	{
		if (!Executor->ConsumeKynde(ActionData.KyndeCost))
		{
			return;
		}
	}

	if (Executor)
	{
		Executor->ConsumeStamina(ActionData.StaminaCost);
		if (ActionData.ManaCost > 0.0f)
		{
			Executor->ConsumeMana(ActionData.ManaCost);
		}
	}

	if (ActionData.KyndeGenerated > 0.0f && Executor)
	{
		Executor->GainKynde(ActionData.KyndeGenerated);
	}

	switch (ActionData.ActionType)
	{
	case ECombatActionType::Strike:
		// Damage and break applied by TurnManager after real-time window
		break;
	case ECombatActionType::Escapade:
	case ECombatActionType::Ward:
		break;
	case ECombatActionType::Counter:
		if (Target)
		{
			Target->ApplyCustomDamage(ActionData.Damage * 1.5f, Executor);
		}
		break;
	case ECombatActionType::Guard:
		if (Executor)
		{
			Executor->SetGuarding(true);
			Executor->RestoreStamina(15.0f);
		}
		break;
	case ECombatActionType::Rest:
		if (Executor)
		{
			Executor->RestoreStamina(20.0f);
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

	if (Executor->GetCurrentStamina() < ActionData.StaminaCost)
	{
		return false;
	}
	if (ActionData.ManaCost > 0.0f && Executor->GetCurrentMana() < ActionData.ManaCost)
	{
		return false;
	}
	return true;
}
