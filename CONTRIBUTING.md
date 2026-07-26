# Contributing

Thanks for helping improve Jackdaw Flag Patcher.

## Before opening an issue

- Confirm the game is closed.
- Restore other mods affecting the selected Jackdaw flag before its first patch
  attempt.
- Record whether you use Black Flag Resynced or the legacy Windows release.
- Include the complete error message, but do not upload Ubisoft game archives.

## Building

1. Use Windows with .NET Framework 4.8 installed.
2. Clone the repository.
3. Run `build.ps1` from PowerShell.
4. Find the result in `build`.

Microsoft DirectXTex `texconv.exe` is retained under its own MIT license and
notice. Do not add Ubisoft artwork or archive data to the repository.

## Pull requests

Keep changes focused and test apply, repeat apply, independent restore, and
cross-resource isolation against a disposable forge fixture. Never run write
tests against a user's only game archive.
