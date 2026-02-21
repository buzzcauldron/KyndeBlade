# Cursor UI Troubleshooting Guide

## Issue: REVIEW/Find Issues Button Not Working

If the REVIEW or "Find Issues" button in Cursor isn't working, here are several solutions:

## Quick Fixes

### 1. **Reload Cursor Window**
- Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac)
- Type "Reload Window"
- Select "Developer: Reload Window"
- This refreshes the entire Cursor interface

### 2. **Check Cursor Status**
- Look at the bottom-right corner of Cursor
- Check if there are any error indicators
- Verify Cursor is connected to the AI service

### 3. **Restart Cursor**
- Close Cursor completely
- Reopen it
- Open the project again

## Understanding Cursor's Issue Detection

### How It Works
Cursor's issue detection relies on:
1. **Language Server Protocol (LSP)** - For syntax errors
2. **File Indexing** - Cursor needs to index your files
3. **Project Recognition** - Cursor needs to recognize the project type

### For Unreal Engine Projects
Unreal Engine C++ projects can be tricky because:
- They use custom build systems
- Files may not be recognized until compiled
- Cursor may need the project to be "opened" properly

## Manual Issue Detection

Since the automated button isn't working, here's how to manually check:

### 1. **Check for Linter Errors**
```bash
# In terminal, check for common C++ issues
cd /home/sethj/KyndeBlade
find Source -name "*.cpp" -o -name "*.h" | xargs grep -n "error\|Error\|ERROR" || echo "No obvious errors found"
```

### 2. **Verify File Structure**
- All `.h` files should have corresponding `.cpp` files
- All includes should be correct
- All classes should have proper `GENERATED_BODY()` macros

### 3. **Check Unreal Engine Compilation**
The best way to find issues is to compile in Unreal Engine:
1. Open the project in Unreal Editor
2. Let it compile
3. Check the Output Log for errors

## Cursor-Specific Solutions

### 1. **Re-index the Project**
- Close Cursor
- Delete `.cursor` folder if it exists (in project root)
- Reopen Cursor
- It will re-index the project

### 2. **Check Cursor Settings**
- Go to Settings (Ctrl+, or Cmd+,)
- Search for "issues" or "diagnostics"
- Ensure diagnostics are enabled
- Check language-specific settings for C++

### 3. **Install/Update C++ Extension**
- Press `Ctrl+Shift+X` to open Extensions
- Search for "C++"
- Install or update the Microsoft C++ extension
- This provides better IntelliSense and error detection

### 4. **Check Workspace Settings**
Create or verify `.vscode/settings.json`:
```json
{
    "C_Cpp.intelliSenseEngine": "default",
    "C_Cpp.errorSquiggles": "enabled",
    "files.associations": {
        "*.h": "cpp",
        "*.cpp": "cpp"
    }
}
```

## Alternative: Use Command Line Tools

### Check for Common Issues
```bash
# Check for missing includes
grep -r "class.*;" Source/ | grep -v "//"

# Check for undefined references
grep -r "GetCurrentCharacter" Source/ --include="*.cpp"

# Check for syntax errors (basic)
find Source -name "*.cpp" -exec echo "Checking {}" \; -exec head -1 {} \;
```

## Verify Project is Properly Opened

### 1. **Check Folder Structure**
Cursor needs to recognize the project root:
- The `.uproject` file should be visible
- The `Source/` folder should be recognized
- Files should show proper syntax highlighting

### 2. **Open as Folder**
- File → Open Folder
- Select `/home/sethj/KyndeBlade`
- Not just individual files

## If Nothing Works

### Manual Code Review Checklist
Since the automated tool isn't working, manually verify:

- [ ] All header files have `#pragma once`
- [ ] All classes have `GENERATED_BODY()`
- [ ] All API macros are consistent (`KYNDEBLADE_API`)
- [ ] All includes use forward slashes
- [ ] No circular dependencies
- [ ] All virtual functions are properly declared
- [ ] All UPROPERTY/UFUNCTION macros are correct

## Contact Cursor Support

If the issue persists:
1. Check Cursor's status page
2. Update to the latest version
3. Report the bug with:
   - Cursor version
   - Operating system
   - Project type (Unreal Engine 5)
   - Steps to reproduce

## Current Project Status

Based on manual review, your KyndeBlade project:
✅ All files are present
✅ No obvious syntax errors
✅ API macros are consistent
✅ Includes look correct
✅ File structure is proper

The code should compile in Unreal Engine without issues.
