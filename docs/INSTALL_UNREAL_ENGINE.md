# Installing Unreal Engine 5.7.1 on Linux

## Method 1: Epic Games Launcher (Recommended)

### Step 1: Download Epic Games Launcher

1. **Visit the Unreal Engine download page:**
   - Go to: https://www.unrealengine.com/en-US/download
   - Click "Download Now"

2. **Download the Linux installer:**
   - The launcher will be available as a `.AppImage` or `.sh` installer
   - Save it to your Downloads folder

3. **Make the installer executable:**
   ```bash
   chmod +x ~/Downloads/EpicGamesLauncher-*.AppImage
   # or
   chmod +x ~/Downloads/EpicGamesLauncher-*.sh
   ```

4. **Run the installer:**
   ```bash
   ~/Downloads/EpicGamesLauncher-*.AppImage
   # or
   ~/Downloads/EpicGamesLauncher-*.sh
   ```

### Step 2: Install Epic Games Launcher

Follow the on-screen instructions to install the launcher. It will typically install to:
- `~/.local/share/EpicGamesLauncher/` or
- `~/Epic Games/`

### Step 3: Launch Epic Games Launcher

```bash
# If installed as AppImage:
~/.local/share/EpicGamesLauncher/EpicGamesLauncher.AppImage

# Or find the launcher:
find ~ -name "*Epic*Launcher*" -type f 2>/dev/null
```

### Step 4: Sign In to Epic Games Account

1. Open the Epic Games Launcher
2. Sign in with your Epic Games account (create one if needed)
3. Accept the Unreal Engine license agreement

### Step 5: Install Unreal Engine 5.7.1

1. In the launcher, go to the **"Unreal Engine"** tab
2. Click **"Install Engine"** or **"+"** button
3. Select **version 5.7.1** from the dropdown
4. Choose installation location (default is usually `~/UnrealEngine/`)
5. Click **"Install"** and wait for download/installation to complete

**Note:** The installation can take 30-60 minutes depending on your internet connection.

## Method 2: Direct Download (Source Code)

If you have GitHub access to Unreal Engine source:

1. **Get access:**
   - Link your Epic Games account to GitHub
   - Visit: https://www.unrealengine.com/en-US/ue-on-github

2. **Clone the repository:**
   ```bash
   git clone -b 5.7.1 https://github.com/EpicGames/UnrealEngine.git ~/UnrealEngine
   ```

3. **Setup and build:**
   ```bash
   cd ~/UnrealEngine
   ./Setup.sh
   ./GenerateProjectFiles.sh
   make
   ```

## Method 3: Pre-built Binary (If Available)

Some Linux distributions may have Unreal Engine in repositories, but this is uncommon. Check your distribution's package manager.

## After Installation

### Verify Installation

```bash
# Check if engine is installed
ls -la ~/UnrealEngine/Engine/Binaries/Linux/UnrealEditor

# Or if installed elsewhere, find it:
find ~ -name "UnrealEditor" -type f 2>/dev/null | grep -i "binaries/linux"
```

### Update Project to Use New Engine

Once installed, you may need to:

1. **Update the project file** (if engine path changed):
   ```bash
   cd /home/sethj/KyndeBlade
   # The .uproject file should automatically detect the engine
   ```

2. **Or specify engine path manually:**
   - Right-click `KyndeBlade.uproject`
   - Select "Switch Unreal Engine version"
   - Choose the installed 5.7.1 version

### Run Setup Scripts

After installation, run the engine setup:

```bash
cd ~/UnrealEngine  # or wherever you installed it
./Setup.sh
```

This will:
- Install required dependencies
- Set up the Linux SDK
- Configure the toolchain

## Troubleshooting

### Launcher Won't Start
- Make sure you have required libraries:
  ```bash
  # On Fedora:
  sudo dnf install libX11 libX11-devel libXcursor libXcursor-devel libXrandr libXrandr-devel
  ```

### Installation Fails
- Check disk space (Unreal Engine needs ~20GB)
- Ensure you have write permissions to the installation directory
- Check Epic Games Launcher logs

### SDK Issues After Installation
- Run `./Setup.sh` in the engine directory
- Verify SDK files are present:
  ```bash
  ls -la ~/UnrealEngine/Engine/Extras/ThirdPartyNotUE/SDKs/HostLinux/
  ```

## Next Steps

After installation:
1. Open your project: `~/UnrealEngine/Engine/Binaries/Linux/UnrealEditor /home/sethj/KyndeBlade/KyndeBlade.uproject`
2. Let it compile the C++ modules
3. The SDK should be properly activated after a fresh installation
