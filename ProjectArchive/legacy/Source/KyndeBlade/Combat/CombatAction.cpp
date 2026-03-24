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

	// Consume stamina
	if (Executor)
	{
		Executor->ConsumeStamina(ActionData.StaminaCost);
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
			
			// Expedition 33-inspired: Apply elemental damage modifiers
			// (Elemental weakness/resistance system would be implemented here)
			
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
	case ECombatActionType::Rest: // Wait - "Kynde's Rest"
		// Rest restores stamina
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

	return Executor->GetCurrentStamina() >= ActionData.StaminaCost;
}
