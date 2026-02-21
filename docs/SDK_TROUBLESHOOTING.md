# Linux SDK Troubleshooting - Unreal Engine 5.7.1

## Issue Summary
The project cannot compile because UnrealBuildTool reports: **"Platform Linux is not a valid platform to build"**

## Root Cause
The Linux AutoSDK is not properly activated. The log shows:
- `Version_AutoSdk=v26_clang-20.1.8-rockylinux8` (detected)
- `CurrentVersion_AutoSdk=` (empty - not activated)
- `which clang++ result: (2) (null)` (clang++ not found in PATH during UBT execution)

## Investigation Results

### ✅ What Works
- Project files generate successfully
- Code analysis shows no syntax errors
- SDK files exist at: `~/Engine/Extras/ThirdPartyNotUE/SDKs/HostLinux/Linux_x64/v26_clang-20.1.8-rockylinux8/`
- System has clang++ installed at `/usr/bin/clang++`
- Project configuration is correct for UE 5.7.1

### ❌ What Doesn't Work
- AutoSDK activation (CurrentVersion_AutoSdk is empty)
- Platform registration fails before compilation can start
- `-ForceUseSystemCompiler` flag doesn't bypass the platform validation
- Setting `LINUX_MULTIARCH_ROOT` environment variable doesn't help
- Editor compilation also fails with the same error

## Attempted Fixes
1. ✅ Updated project to UE 5.7.1
2. ✅ Created `Activated.txt` marker file (didn't help)
3. ✅ Tried `-ForceUseSystemCompiler` flag
4. ✅ Set `LINUX_MULTIARCH_ROOT` environment variable
5. ✅ Set PATH to include `/usr/bin`
6. ✅ Opened editor (same error)

## Potential Solutions

### Option 1: Complete Engine Setup
The engine may need to run its setup scripts:
```bash
cd ~/Engine
./Setup.sh
```
However, `SetupToolchain.sh` is missing, which suggests the engine installation may be incomplete.

### Option 2: Manual SDK Activation
The AutoSDK system may require specific files or scripts to activate. Check if there's an activation script in:
- `~/Engine/Build/BatchFiles/Linux/`
- `~/Engine/Extras/ThirdPartyNotUE/SDKs/`

### Option 3: Use System Compiler (Workaround)
If the engine supports it, we may need to:
1. Modify the platform SDK detection code to always use system compiler
2. Or find a configuration file that forces system compiler usage

### Option 4: Reinstall/Repair Engine
The engine installation may be incomplete. Consider:
- Re-downloading the engine
- Running engine setup/repair tools
- Checking Epic Games Launcher for engine updates

## Current Status
- **Project**: ✅ Configured for UE 5.7.1
- **Code**: ✅ No compilation errors detected (static analysis)
- **SDK**: ❌ AutoSDK not activated
- **Compilation**: ❌ Blocked by platform validation

## Next Steps
1. Check if engine setup scripts need to be run
2. Investigate AutoSDK activation mechanism further
3. Consider using a different build method (if available)
4. Contact Epic Games support or check Unreal Engine forums for Linux SDK activation issues
