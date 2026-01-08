#include "UI/CombatUI.h"
#include "Characters/MedievalCharacter.h"
#include "Combat/CombatAction.h"

UCombatUI::UCombatUI(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	SelectedCharacter = nullptr;
	SelectedAction = nullptr;
	SelectedTarget = nullptr;
}

void UCombatUI::NativeConstruct()
{
	Super::NativeConstruct();
}

void UCombatUI::UpdateCharacterInfo(AMedievalCharacter* Character)
{
	SelectedCharacter = Character;
	if (Character)
	{
		OnHealthChanged(Character->GetCurrentHealth(), Character->GetMaxHealth());
		OnStaminaChanged(Character->GetCurrentStamina(), Character->GetMaxStamina());
	}
}

void UCombatUI::UpdateTurnInfo(AMedievalCharacter* CurrentCharacter, int32 TurnNumber)
{
	OnTurnChanged(CurrentCharacter);
}

void UCombatUI::ShowActionMenu(bool bShow)
{
	// Implemented in Blueprint
}

void UCombatUI::ShowRealTimeWindow(float Duration)
{
	// Implemented in Blueprint
}

void UCombatUI::OnActionSelected(UCombatAction* Action)
{
	SelectedAction = Action;
}

void UCombatUI::OnTargetSelected(AMedievalCharacter* Target)
{
	SelectedTarget = Target;
}
