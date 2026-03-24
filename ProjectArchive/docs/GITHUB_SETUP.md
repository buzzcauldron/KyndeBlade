# GitHub Setup Guide

## Creating a Private Repository on GitHub

### Step 1: Create the Repository on GitHub

1. Go to https://github.com/new
2. Repository name: `KyndeBlade` (or your preferred name)
3. Description: "A melancholic turn-based RPG about aging, work, poverty, and the unresolved quest for Grace"
4. **Select "Private"** (important!)
5. **Do NOT** initialize with README, .gitignore, or license (we already have these)
6. Click "Create repository"

### Step 2: Add Remote and Push

After creating the repository, GitHub will show you commands. Use these:

```bash
git remote add origin https://github.com/YOUR_USERNAME/KyndeBlade.git
git branch -M main
git push -u origin main
```

Replace `YOUR_USERNAME` with your GitHub username.

### Alternative: Using SSH (if you have SSH keys set up)

```bash
git remote add origin git@github.com:YOUR_USERNAME/KyndeBlade.git
git branch -M main
git push -u origin main
```

## Authentication

If you're prompted for credentials:
- **HTTPS**: Use a Personal Access Token (not your password)
  - Create one at: https://github.com/settings/tokens
  - Scopes needed: `repo` (full control of private repositories)
- **SSH**: Make sure your SSH key is added to GitHub
  - Check: https://github.com/settings/keys

## Current Git Status

Your repository is ready to push with:
- All source code
- Campaign design documents
- Music sourcing guide
- All commits preserved
