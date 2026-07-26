# Nexus Mods upload checklist

## Main file

- Upload `JackdawFlagPatcher-v1.0.0.zip` as the main file.
- Version: `1.0.0`
- Suggested category: Utilities
- No external dependency is required.
- Do not mark Sails Workshop as a requirement.

## Description and installation

- Paste `NEXUS-DESCRIPTION.md` into the mod description.
- Emphasize the 1024 × 512 RGBA requirement.
- Keep the 8–16 transparent-pixel safe-area warning visible.
- State that other Jackdaw flag mods must be restored before first use.
- State that the game must be closed while applying or restoring.

## Source

Create the public repository as:

`https://github.com/emike09/Jackdaw-Flag-Patcher`

Upload the contents of the GitHub source bundle, not the outer folder itself.
Confirm that the README, MIT license, third-party notices, contribution guide,
security policy, and v1.0 release tag are visible.

## Nexus permissions

Suggested settings:

- Users may use and modify the original Jackdaw Flag Patcher code under MIT.
- Users must follow the separate DirectXTex MIT notice for `texconv`.
- No Ubisoft assets are granted or redistributed by this project.
- Sails Workshop code and files are not part of this project.

## Final checks

- Test the downloaded ZIP after uploading.
- Confirm SmartScreen/unsigned-executable wording is present.
- Confirm the GitHub link works.
- Add at least one in-game screenshot showing a replacement flag.
- Keep the original PNG artwork out of the utility download unless its creator
  explicitly permits redistribution.
