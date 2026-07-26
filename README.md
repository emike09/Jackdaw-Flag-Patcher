# Jackdaw Flag Patcher

A small standalone Windows tool that independently replaces the Jackdaw's regular
and end-game flags in Assassin's Creed IV: Black Flag. Supply your own 1024 × 512
PNG; the tool converts it to the game's BC7 texture format and safely relinks only
the selected flag resource.

## Use

1. Extract the entire download to a normal folder.
2. If another mod has already changed either Jackdaw flag, restore that flag with
   the other mod first.
3. Close the game.
4. Run `JackdawFlagPatcher.exe`.
5. Confirm the automatically detected installation or browse to the folder
   containing `ACBlackFlag.exe` or `AC4BFSP.exe`.
6. Under **Regular flag** or **End-game flag**, choose a 1024 × 512 PNG and select
   **Apply replacement**.

Each flag has its own **Restore original** action. Applying or restoring one flag
does not change the other.

## Image requirements

- PNG
- Exactly 1024 × 512 pixels
- RGBA/transparency strongly recommended
- Keep all visible artwork at least 8–16 transparent pixels away from every edge.
  Artwork touching the image boundaries may be clipped in game.
- Preserve the original extracted flag's alpha silhouette for natural torn edges
- Artwork is inverted. You must submit an upside-down image. 

The tool modifies `DataPC_boot.forge`. It appends new texture data and changes only
the selected TextureMap index row:

- Regular flag: `0x218240C6D66`
- End-game flag: `0x20A1AA95DFB`

The original bytes remain in the archive. Independent restoration records are
kept in Documents under `Jackdaw Flag Patcher Backup`.

## Compatibility and safety

- Intended for the standard Windows PC release of Assassin's Creed IV: Black Flag.
- Detects Steam/Resynced and Ubisoft installs from Windows registry records, then
  follows Steam library metadata when needed. Manual folder selection remains
  available.
- The patcher checks the original entry hash before making its first change.
- Data is appended and verified before the game index is changed.
- The game must be closed.
- Repeated applications are supported; only the first original backup for each
  flag is retained.
- Restoring does not shrink the forge archive; appended bytes remain unreferenced.
- Verify game files through your game launcher if the installed game build is not
  recognized or if the archive has been altered unexpectedly.

This is a clean-room, two-resource tool. It does not contain or require Sails
Workshop, and it does not bundle Ubisoft artwork. Microsoft DirectXTex `texconv`
is included under the MIT License; see the included notices.

## Source code

Source, releases, and issue tracking:
[github.com/emike09/Jackdaw-Flag-Patcher](https://github.com/emike09/Jackdaw-Flag-Patcher)

To build locally on Windows, run `build.ps1`. It uses the installed .NET Framework
C# compiler and the included MIT-licensed DirectXTex binary. The program itself is
released under the MIT License.

## Nexus Mods dependency setting

Do **not** mark Sails Workshop as a required dependency for this standalone
version. It is technically and legally independent. A courteous credit can say:

> Inspired by the user-friendly texture workflow demonstrated by Sails Workshop.
> This patcher is an independent implementation and includes no Sails Workshop
> code or files.

Assassin's Creed and related names are trademarks of Ubisoft. This fan-made tool
is not affiliated with or endorsed by Ubisoft.
