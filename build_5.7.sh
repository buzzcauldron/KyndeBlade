#!/bin/bash
# Build script for Kynde Blade - Unreal Engine 5.7
# Usage: ./build_5.7.sh [path/to/UnrealEngine]

set -e

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_FILE="${PROJECT_DIR}/KyndeBlade.uproject"

# Check if Unreal Engine path is provided
if [ -n "$1" ]; then
    UE_PATH="$1"
else
    # Try to find Unreal Engine 5.7
    UE_PATH=$(find ~ -type d -name "5.7" -path "*/UnrealEngine/Engine" 2>/dev/null | head -1 | sed 's|/Engine$||')
    
    if [ -z "$UE_PATH" ]; then
        echo "Error: Unreal Engine 5.7 path not found."
        echo "Usage: $0 [path/to/UnrealEngine]"
        echo ""
        echo "Please provide the path to your Unreal Engine installation, e.g.:"
        echo "  $0 ~/UnrealEngine"
        echo "  $0 /opt/unreal-engine"
        exit 1
    fi
fi

# Verify Unreal Engine path
if [ ! -f "${UE_PATH}/Engine/Binaries/Linux/UnrealEditor" ]; then
    echo "Error: Unreal Editor not found at ${UE_PATH}/Engine/Binaries/Linux/UnrealEditor"
    echo "Please verify your Unreal Engine 5.7 installation path."
    exit 1
fi

echo "Using Unreal Engine at: ${UE_PATH}"
echo "Project file: ${PROJECT_FILE}"
echo ""

# Step 1: Generate project files
echo "Step 1: Generating project files..."
"${UE_PATH}/Engine/Build/BatchFiles/Linux/GenerateProjectFiles.sh" \
    -project="${PROJECT_FILE}" \
    -game \
    -rocket \
    -progress

if [ $? -ne 0 ]; then
    echo "Error: Failed to generate project files"
    exit 1
fi

echo "Project files generated successfully!"
echo ""

# Step 2: Compile the project
echo "Step 2: Compiling KyndeBladeEditor for Linux Development..."
"${UE_PATH}/Engine/Build/BatchFiles/Linux/Build.sh" \
    KyndeBladeEditor \
    Linux \
    Development \
    "${PROJECT_FILE}" \
    -WaitMutex \
    -FromMsBuild

if [ $? -ne 0 ]; then
    echo "Error: Compilation failed"
    exit 1
fi

echo ""
echo "Build completed successfully!"
echo ""
echo "To open the project in Unreal Editor, run:"
echo "  ${UE_PATH}/Engine/Binaries/Linux/UnrealEditor ${PROJECT_FILE}"
