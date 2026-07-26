# Jackdaw Flag Patcher

Independently replace the Jackdaw's regular flag, end-game flag, or both with your
own designs. Choose a 1024 × 512 PNG for the flag you want to change and the
patcher handles conversion and installation while retaining the original flag
model, torn silhouette, and cloth physics.

## Requirements

- Assassin's Creed IV: Black Flag for Windows
- Your own 1024 × 512 PNG
- No other mod or framework is required

## Installation

1. Download and extract the complete ZIP.
2. If another mod currently changes the selected Jackdaw flag, use that mod to
   restore the original first.
3. Close the game.
4. Run `JackdawFlagPatcher.exe`.
5. Confirm the detected Black Flag folder—or browse manually.
6. Under **Regular flag** or **End-game flag**, choose your replacement PNG and
   click **Apply replacement**.

Each flag has its own **Restore original** button. Applying or restoring one flag
does not change the other.

Your design should use transparency and the alpha silhouette from an extracted
original flag if you want to preserve its naturally torn edges.

**Important safe-area rule:** keep all visible artwork at least 8–16 transparent
pixels away from all four image edges. Graphics touching the 1024 × 512 boundaries
may be clipped in game.

## What it changes

This tool supports two separately selectable assets: the Jackdaw's regular flag
and end-game flag. It appends a BC7 replacement texture to the appropriate base
or Resynced patch forge and relinks only the selected flag resource. It does not
replace flag geometry or physics.

The original archive data is not overwritten. The tool verifies the replacement
before relinking the resource and keeps independent restoration records in your
Documents folder.

## Compatibility

The patcher recognizes the Steam Resynced executable and the legacy PC executable.
It searches Steam and Ubisoft registry entries and Steam library metadata, with
manual selection available. Restore other mods affecting the selected flag before
its first use. Launcher file verification can recover an altered or unsupported
archive.

The executable is unsigned, so Windows may display a SmartScreen prompt.

Version 1.1.1 corrects the end-game archive target used by version 1.1.0 and
automatically restores that version's inactive base-archive redirect.

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
