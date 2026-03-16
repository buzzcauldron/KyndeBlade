# Download and Install Unreal Engine 5.7.1

## Step 1: Download Epic Games Launcher

The Epic Games Launcher must be downloaded manually from the official website:

1. **Visit the Unreal Engine download page:**
   ```
   https://www.unrealengine.com/en-US/download
   ```

2. **Click "Download Now"**

3. **Select Linux** when prompted for your platform

4. **Download the installer:**
   - It will be either an `.AppImage` file or a `.sh` script
   - Save it to your `~/Downloads` folder

## Step 2: Install Epic Games Launcher

Once downloaded:

```bash
# Navigate to Downloads
cd ~/Downloads

# Make the installer executable
chmod +x EpicGamesLauncher-*.AppImage
# or if it's a .sh file:
chmod +x EpicGamesLauncher-*.sh

# Run the installer
./EpicGamesLauncher-*.AppImage
# or
./EpicGamesLauncher-*.sh
```

Follow the on-screen installation instructions.

## Step 3: Launch Epic Games Launcher

After installation, launch the launcher:

```bash
# Find where it was installed
find ~ -name "*Epic*Launcher*" -type f -executable 2>/dev/null

# Or if installed as AppImage in a standard location:
~/.local/share/EpicGamesLauncher/EpicGamesLauncher.AppImage
```

## Step 4: Sign In and Install Unreal Engine

1. **Sign in to Epic Games:**
   - Open the Epic Games Launcher
   - Sign in with your Epic Games account (create one at https://www.epicgames.com if needed)
   - Accept the Unreal Engine license agreement

2. **Install Unreal Engine 5.7.1:**
   - Go to the **"Unreal Engine"** tab in the launcher
   - Click **"Install Engine"** or the **"+"** button
   - In the version dropdown, select **"5.7.1"**
   - Choose installation location (default: `~/UnrealEngine/`)
   - Click **"Install"**
   - Wait for download and installation (this can take 30-60 minutes, ~20-30GB)

## Step 5: Verify Installation

After installation completes:

```bash
# Check if engine is installed
ls -la ~/UnrealEngine/Engine/Binaries/Linux/UnrealEditor

# Run setup scripts
cd ~/UnrealEngine
./Setup.sh
```

## Step 6: Open Your Project

Once Unreal Engine 5.7.1 is installed:

```bash
# Open your project
~/UnrealEngine/Engine/Binaries/Linux/UnrealEditor /home/sethj/KyndeBlade/KyndeBlade.uproject
```

The editor should:
1. Detect the project needs compilation
2. Prompt to rebuild modules
3. Compile successfully (SDK should be properly activated after fresh installation)

## Troubleshooting

### Launcher Won't Start
Install required dependencies:
```bash
# On Fedora:
sudo dnf install libX11 libX11-devel libXcursor libXcursor-devel libXrandr libXrandr-devel libXScrnSaver libXScrnSaver-devel
```

### Installation Fails
- Check you have at least 50GB free disk space
- Ensure you have write permissions to the installation directory
- Check Epic Games Launcher logs in: `~/.config/Epic/EpicGamesLauncher/`

### Can't Find Engine After Installation
The engine is typically installed to:
- `~/UnrealEngine/` (default)
- Or check: `~/.config/Epic/UnrealEngineLauncher/LauncherInstalled.dat`

## Alternative: Build from Source

If you have GitHub access to Unreal Engine:

1. **Link Epic account to GitHub:**
   - Visit: https://www.unrealengine.com/en-US/ue-on-github
   - Follow instructions to link accounts

2. **Clone and build:**
   ```bash
   git clone -b 5.7.1 https://github.com/EpicGames/UnrealEngine.git ~/UnrealEngine
   cd ~/UnrealEngine
   ./Setup.sh
   ./GenerateProjectFiles.sh
   make
   ```

## Next Steps

After installation:
1. The SDK should be properly activated
2. Your project should compile successfully
3. You can continue development in Unreal Engine 5.7.1
