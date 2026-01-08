#include "Input/CombatInputComponent.h"
#include "EnhancedInputComponent.h"
#include "EnhancedInputSubsystems.h"
#include "InputActionValue.h"
#include "GameFramework/PlayerController.h"

UCombatInputComponent::UCombatInputComponent(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	PrimaryComponentTick.bCanEverTick = false;
	bRealTimeWindowActive = false;
}

void UCombatInputComponent::BeginPlay()
{
	Super::BeginPlay();
}

void UCombatInputComponent::SetupInputBindings(UInputComponent* PlayerInputComponent)
{
	if (UEnhancedInputComponent* EnhancedInputComponent = Cast<UEnhancedInputComponent>(PlayerInputComponent))
	{
		if (DodgeAction)
		{
			EnhancedInputComponent->BindAction(DodgeAction, ETriggerEvent::Triggered, this, &UCombatInputComponent::HandleDodge);
		}
		if (ParryAction)
		{
			EnhancedInputComponent->BindAction(ParryAction, ETriggerEvent::Triggered, this, &UCombatInputComponent::HandleParry);
		}
		if (CounterAction)
		{
			EnhancedInputComponent->BindAction(CounterAction, ETriggerEvent::Triggered, this, &UCombatInputComponent::HandleCounter);
		}
	}
}

void UCombatInputComponent::HandleDodge() // Handle Escapade (Kynde's Evasion)
{
	if (bRealTimeWindowActive)
	{
		OnDodgeInput.Broadcast();
	}
}

void UCombatInputComponent::HandleParry() // Handle Ward (Trewthe's Sheeld)
{
	if (bRealTimeWindowActive)
	{
		OnParryInput.Broadcast();
	}
}

void UCombatInputComponent::HandleCounter() // Handle Counter-strike
{
	if (bRealTimeWindowActive)
	{
		OnCounterInput.Broadcast();
	}
}
