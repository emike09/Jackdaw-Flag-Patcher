# Jackdaw Flag Patcher

Replace only the Jackdaw's pirate flag in Assassin's Creed IV: Black Flag with
your own design. Choose a 1024 × 512 PNG and the patcher handles conversion and
installation while retaining the original flag model, torn silhouette, and cloth
physics.

## Requirements

- Assassin's Creed IV: Black Flag for Windows
- Your own 1024 × 512 PNG
- No other mod or framework is required

## Installation

1. Download and extract the complete ZIP.
2. If another mod currently changes the Jackdaw flag, use that mod to restore the
   original first.
3. Close the game.
4. Run `JackdawFlagPatcher.exe`.
5. Confirm the detected Black Flag folder—or browse manually—choose your
   replacement PNG, then click **Apply flag**.

Click **Restore original** in the same tool to uninstall the replacement.

Your design should use transparency and the alpha silhouette from an extracted
original flag if you want to preserve its naturally torn edges.

**Important safe-area rule:** keep all visible artwork at least 8–16 transparent
pixels away from all four image edges. Graphics touching the 1024 × 512 boundaries
may be clipped in game.

## What it changes

This tool supports one asset only: the Jackdaw pirate flag. It appends a BC7
replacement texture to `DataPC_boot.forge` and relinks file ID `0x218240C6D66`.
It does not replace flag geometry or physics.

The original archive data is not overwritten. The tool verifies the replacement
before relinking the resource and keeps a small restoration record in your
Documents folder.

## Compatibility

The patcher recognizes the Steam Resynced executable and the legacy PC executable.
It searches Steam and Ubisoft registry entries and Steam library metadata, with
manual selection available. Restore other Jackdaw flag mods before first use.
Launcher file verification can recover an altered or unsupported archive.

The executable is unsigned, so Windows may display a SmartScreen prompt.

## Source code

The complete MIT-licensed source is available on GitHub:
[github.com/emike09/Jackdaw-Flag-Patcher](https://github.com/emike09/Jackdaw-Flag-Patcher)

## Credits and permissions

Jackdaw Flag Patcher is an independent, clean-room implementation. It includes no
Sails Workshop code or files and does not require Sails Workshop.

Inspired by the user-friendly texture workflow demonstrated by Sails Workshop.

Texture conversion uses Microsoft's DirectXTex `texconv`, included under the MIT
License with its full notice. Jackdaw Flag Patcher source is released under the
MIT License.

Assassin's Creed and related names are trademarks of Ubisoft. This fan-made tool
is not affiliated with or endorsed by Ubisoft.
