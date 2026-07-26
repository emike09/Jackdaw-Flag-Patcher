# Changelog

## 1.1.1 - 2026-07-25

- Corrected the end-game flag target to the active resource copy in
  `DataPC_boot_patch_01.forge`.
- Added automatic restoration of the inactive `DataPC_boot.forge` copy changed by
  version 1.1.0.
- Added archive-priority fixture coverage for base and patch forge copies sharing
  the same resource ID.

## 1.1.0 - 2026-07-25

- Added independent replacement support for the regular and end-game flags.
- Added clearly separated, non-spoiler controls for choosing which flag to change.
- Added an independent backup and restore path for each flag resource.
- Preserved compatibility with regular-flag backups created by version 1.0.
- Added dual-resource fixture coverage proving that applying or restoring either
  flag leaves the other resource index untouched.

## 1.0.0 - 2026-07-25

- Promoted the tested patcher to its first stable release.
- Added a cleaner responsive interface with automatic Windows light/dark theming.
- Added a visible 8–16 pixel artwork safe-area recommendation.
- Added GitHub-ready project and Nexus publishing documentation.

## 0.2 - 2026-07-25

- Added install discovery through Steam and Ubisoft registry entries.
- Added Steam library-folder and app-manifest discovery.
- Added support for the Resynced executable name `ACBlackFlag.exe`.
- Retained manual installation-folder selection.
- Corrected and preserved the Jackdaw resource type used by the restored Resynced
  forge index.
- Added a pirate skull-and-crossbones application icon.

## 0.1 - 2026-07-25

- Initial standalone release.
- Applies a user-supplied 1024 × 512 PNG to the Jackdaw pirate flag only.
- Converts images to BC7 UNORM with alpha support.
- Verifies the original resource before first use.
- Appends and verifies replacement data before relinking the forge index.
- Supports repeated flag changes and one-click restoration.
