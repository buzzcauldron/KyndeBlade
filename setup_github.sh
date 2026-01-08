#!/bin/bash
# GitHub Repository Setup Script for KyndeBlade
# This script helps you push your project to a private GitHub repository

# IMPORTANT: Replace YOUR_USERNAME with your actual GitHub username
GITHUB_USERNAME="YOUR_USERNAME"
REPO_NAME="KyndeBlade"

echo "=========================================="
echo "KyndeBlade GitHub Setup"
echo "=========================================="
echo ""
echo "Before running this script:"
echo "1. Go to https://github.com/new"
echo "2. Create a repository named: $REPO_NAME"
echo "3. Make sure it's set to PRIVATE"
echo "4. Do NOT initialize with README, .gitignore, or license"
echo "5. Replace YOUR_USERNAME in this script with your GitHub username"
echo ""
read -p "Have you created the repository and updated YOUR_USERNAME? (y/n) " -n 1 -r
echo ""

if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Please create the repository first and update the script."
    exit 1
fi

if [ "$GITHUB_USERNAME" = "YOUR_USERNAME" ]; then
    echo "ERROR: Please replace YOUR_USERNAME with your actual GitHub username in this script"
    exit 1
fi

echo "Adding remote repository..."
git remote add origin https://github.com/$GITHUB_USERNAME/$REPO_NAME.git 2>/dev/null

if [ $? -ne 0 ]; then
    echo "Remote might already exist. Checking..."
    git remote set-url origin https://github.com/$GITHUB_USERNAME/$REPO_NAME.git
fi

echo "Current branch: $(git branch --show-current)"
echo ""
echo "Pushing to GitHub..."
echo "Note: You may be prompted for credentials."
echo "Use a Personal Access Token (not password) for HTTPS"
echo ""

# Push to GitHub (will create main branch if it doesn't exist)
git push -u origin master:main

if [ $? -eq 0 ]; then
    echo ""
    echo "=========================================="
    echo "Success! Your repository is now on GitHub"
    echo "=========================================="
    echo "Repository URL: https://github.com/$GITHUB_USERNAME/$REPO_NAME"
    echo ""
    echo "To verify it's private, check the repository settings on GitHub"
else
    echo ""
    echo "=========================================="
    echo "Push failed. Common issues:"
    echo "=========================================="
    echo "1. Repository doesn't exist or wrong name"
    echo "2. Authentication failed (use Personal Access Token)"
    echo "3. Repository not set to private (check on GitHub)"
    echo ""
    echo "For HTTPS authentication, create a token at:"
    echo "https://github.com/settings/tokens"
    echo "Required scope: 'repo' (full control of private repositories)"
fi
