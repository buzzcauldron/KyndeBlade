using UnrealBuildTool;

public class KyndeBlade : ModuleRules
{
	public KyndeBlade(ReadOnlyTargetRules Target) : base(Target)
	{
		PCHUsage = PCHUsageMode.UseExplicitOrSharedPCHs;
		
		PublicDependencyModuleNames.AddRange(new string[] 
		{ 
			"Core", 
			"CoreUObject", 
			"Engine", 
			"InputCore",
			"EnhancedInput",
			"GameplayTasks",
			"AIModule",
			"UMG",
			"Slate",
			"SlateCore"
		});
		
		PrivateDependencyModuleNames.AddRange(new string[] 
		{
			"GameplayAbilities",
			"GameplayTags"
		});
	}
}
