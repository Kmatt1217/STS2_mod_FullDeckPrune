# Full Deck Prune

Slay the Spire 2 mod.

Start a run with every eligible card for the selected base character, then remove one card after each combat.

## Behavior

- Each character starts with one copy of every eligible card in that character's card pool.
- Starter Strike/Defend-style cards are reduced to one copy each.
- Curse, status, Ancient-only, and invalid special cards are excluded.
- Multiplayer-only cards are excluded in singleplayer.
- Multiplayer-only cards are included in multiplayer.
- After each combat, card rewards are reduced and a card-removal reward is added when the deck has enough removable cards.

## Install

Download `dist/FullDeckPrune.zip`, then extract it into the Slay the Spire 2 `mods` folder.

Default Windows install path:

```text
C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\
```

After extraction, the files should look like this:

```text
mods/
  FullDeckPrune/
    FullDeckPrune.dll
    FullDeckPrune.json
```

This mod depends on `BaseLib`, so BaseLib must also be installed and enabled.

## Build

Build the mod:

```powershell
dotnet build .\FullDeckPrune\FullDeckPrune.csproj -c Release
```

Create the installable zip:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\package-release.ps1 -Configuration Release
```

The zip is written to:

```text
dist\FullDeckPrune.zip
```

## Development Notes

STS2 modding uses C# / Godot / Harmony-style patches, with BaseLib as the common helper library for content and hooks.

Useful references:

- BaseLib wiki: https://alchyr.github.io/BaseLib-Wiki/
- BaseLib repo: https://github.com/Alchyr/BaseLib-StS2
- STS2 mod template setup: https://github.com/Alchyr/ModTemplate-StS2/wiki/Setup

## Local Setup Status

- Slay the Spire 2 was found at `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2`.
- `.NET` command exists, but no SDK is installed yet. Install the .NET SDK before creating/building the mod project.
- Recommended template command after installing the SDK:

```powershell
dotnet new install Alchyr.Sts2.Templates
```

Then create an empty STS2 mod project in this repository, with the solution and project in the same directory.

## Behavior Spec

See [docs/mod-design.md](docs/mod-design.md).
