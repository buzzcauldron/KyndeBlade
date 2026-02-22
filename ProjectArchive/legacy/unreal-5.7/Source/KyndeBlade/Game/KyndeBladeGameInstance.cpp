#include "KyndeBladeGameInstance.h"
#include "Kismet/GameplayStatics.h"
#include "Engine/World.h"

void UKyndeBladeGameInstance::OpenCombatLevel(FName LevelName)
{
	UWorld* World = GetWorld();
	if (World)
	{
		UGameplayStatics::OpenLevel(this, LevelName);
	}
}

void UKyndeBladeGameInstance::OpenMainMenu()
{
	UGameplayStatics::OpenLevel(this, FName(TEXT("MainMenu")));
}
