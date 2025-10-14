#!/usr/bin/env bash
set -euo pipefail

# Replace Digibee -> Digibeast across text files and git-move files/dirs containing 'Digibee'
# Run from repository root. This script edits files in-place and stages changes for commit.

ROOT="$(pwd)"
echo "Repository root: $ROOT"

echo "1) Replacing content occurrences (case-sensitive and lower-case)..."
# Use git grep to limit to tracked text files
FILES=$(git grep -Il "Digibee" || true)
if [ -n "$FILES" ]; then
  echo "$FILES" | xargs -d '\n' -r sed -i 's/Digibee/Digibeast/g'
fi

FILES_LO=$(git grep -Il "digibee" || true)
if [ -n "$FILES_LO" ]; then
  echo "$FILES_LO" | xargs -d '\n' -r sed -i 's/digibee/digibeast/g'
fi

echo "2) Renaming files and directories that include 'Digibee' in their paths..."
# Find files/dirs tracked by git with 'Digibee' in the path
git ls-files | grep -i "Digibee" || true | while IFS= read -r path; do
  # Compute new path by replacing Digibee -> Digibeast and digibee -> digibeast
  newpath=$(echo "$path" | sed -e 's/Digibee/Digibeast/g' -e 's/digibee/digibeast/g')
  if [ "$path" != "$newpath" ]; then
    echo "Renaming: $path -> $newpath"
    # Create destination dirs if missing
    mkdir -p "$(dirname "$newpath")"
    git mv -f "$path" "$newpath" || echo "git mv failed for $path -> $newpath"
  fi
done

echo "3) Stage and show git status..."
git add -A
git status --porcelain

echo "Rename script completed. Please review changes and commit if okay."
