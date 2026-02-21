#!/bin/bash
# Monitor compilation status

PROJECT_DIR="/home/sethj/KyndeBlade"
LOG_FILE="${PROJECT_DIR}/Saved/Logs/KyndeBlade.log"
BINARIES_DIR="${PROJECT_DIR}/Binaries/Linux"

echo "Monitoring compilation status..."
echo "Watching: ${LOG_FILE}"
echo ""

# Wait for binaries to appear or compilation messages
while true; do
    # Check if binaries exist
    if [ -f "${BINARIES_DIR}/KyndeBladeEditor" ]; then
        echo "✓ Compilation complete! Binary found at: ${BINARIES_DIR}/KyndeBladeEditor"
        exit 0
    fi
    
    # Check log for compilation messages
    if [ -f "${LOG_FILE}" ]; then
        # Check for compilation errors
        if grep -qi "error.*compile\|compile.*error\|build.*failed" "${LOG_FILE}" | tail -1; then
            echo "✗ Compilation errors detected. Check the log: ${LOG_FILE}"
            tail -20 "${LOG_FILE}" | grep -i error
            exit 1
        fi
        
        # Check for successful compilation
        if grep -qi "compilation.*complete\|build.*success\|modules.*compiled" "${LOG_FILE}" | tail -1; then
            echo "✓ Compilation appears complete (checking for binaries...)"
            sleep 2
            if [ -f "${BINARIES_DIR}/KyndeBladeEditor" ]; then
                echo "✓ Compilation successful!"
                exit 0
            fi
        fi
    fi
    
    # Check if editor is still running
    if ! pgrep -f "UnrealEditor.*KyndeBlade" > /dev/null; then
        echo "Editor process not found. It may have closed."
        exit 1
    fi
    
    sleep 5
done
