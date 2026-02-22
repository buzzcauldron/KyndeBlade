#!/usr/bin/env python3
"""
Script to compile KyndeBlade using Unreal Engine's editor automation
"""
import subprocess
import sys
import time
import os

PROJECT_PATH = "/home/sethj/KyndeBlade/KyndeBlade.uproject"
EDITOR_PATH = "/home/sethj/Engine/Binaries/Linux/UnrealEditor"

# Unreal Engine automation command to compile
AUTOMATION_COMMANDS = """
Automation SetFilter Engine
Automation RunTests Project.Functional
Quit
"""

def main():
    print("Attempting to compile via Unreal Editor automation...")
    print(f"Project: {PROJECT_PATH}")
    print(f"Editor: {EDITOR_PATH}")
    
    # Try to compile using editor automation
    cmd = [
        EDITOR_PATH,
        PROJECT_PATH,
        "-game",
        "-stdout",
        "-unattended",
        "-NoSplash",
        "-NoSound",
        "-NullRHI",
        "-ExecCmds=" + AUTOMATION_COMMANDS.replace("\n", ";")
    ]
    
    print(f"Running: {' '.join(cmd)}")
    print("This may take several minutes...")
    
    try:
        proc = subprocess.Popen(
            cmd,
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            text=True,
            bufsize=1
        )
        
        # Monitor output
        for line in proc.stdout:
            if "compile" in line.lower() or "build" in line.lower():
                print(f"LOG: {line.strip()}")
            if "error" in line.lower() and "compile" in line.lower():
                print(f"ERROR: {line.strip()}")
            if "success" in line.lower() and "compile" in line.lower():
                print(f"SUCCESS: {line.strip()}")
        
        proc.wait()
        return proc.returncode
        
    except Exception as e:
        print(f"Error: {e}")
        return 1

if __name__ == "__main__":
    sys.exit(main())
