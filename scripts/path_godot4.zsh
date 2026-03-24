# Prepend Godot 4 editor CLI to PATH (macOS). The binary inside the .app is named `Godot`.
# From ~/.zshrc (adjust the path to this repo):
#   source ~/Projects/KyndeBlade/scripts/path_godot4.zsh
#
# Also prepends this repo’s `scripts/` so `./scripts/godot4` / `godot4` work in scripts and CI.
# Order: standard app names, then optional .cache unpack (see docs/CI_GODOT_TESTS.md).

_KB_REPO_ROOT="${0:A:h:h}"
export PATH="${_KB_REPO_ROOT}/scripts:/Applications/Godot.app/Contents/MacOS:/Applications/Godot 4.app/Contents/MacOS:${_KB_REPO_ROOT}/.cache/Godot.app/Contents/MacOS:${PATH}"
unset _KB_REPO_ROOT
