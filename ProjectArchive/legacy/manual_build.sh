#!/bin/bash
# Manual build script that ensures proper environment

set -e

PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_FILE="${PROJECT_DIR}/KyndeBlade.uproject"
ENGINE_PATH="$HOME/Engine"

# Ensure PATH includes system binaries
export PATH="/usr/bin:/usr/local/bin:$PATH"

# Try to activate AutoSDK by creating necessary files
SDK_DIR="$ENGINE_PATH/Extras/ThirdPartyNotUE/SDKs/HostLinux/Linux_x64/v26_clang-20.1.8-rockylinux8"
if [ -d "$SDK_DIR" ]; then
    # Create activation marker if it doesn't exist
    touch "$SDK_DIR/Activated.txt"
    echo "v26_clang-20.1.8-rockylinux8" > "$SDK_DIR/Activated.txt"
fi

# Set environment variables that might help
export LINUX_ROOT="/usr"
export LINUX_MULTIARCH_ROOT="$SDK_DIR"

echo "Building KyndeBladeEditor for Linux Development..."
echo "Project: $PROJECT_FILE"
echo "Engine: $ENGINE_PATH"
echo ""

# Try building
"$ENGINE_PATH/Build/BatchFiles/Linux/Build.sh" \
    KyndeBladeEditor \
    Linux \
    Development \
    -Project="$PROJECT_FILE" \
    -WaitMutex \
    -FromMsBuild \
    -ForceUseSystemCompiler \
    "$@"
