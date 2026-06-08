# STS2 Full Deck Prune

Slay the Spire 2 mod concept:

Start a run with every card for the selected base character, including starter cards and one copy of each non-starter card. After each combat, let the player remove exactly one card from the deck.

## Current Goal

Build the smallest working version first:

1. Load the mod in Slay the Spire 2.
2. Detect a new run and replace the starter deck with the full card pool for the chosen base character.
3. After each combat victory, open a remove-card choice screen.
4. Remove the chosen card from the master deck.

## Development Notes

STS2 modding currently uses C# / Godot / Harmony-style patches, with BaseLib as the common helper library for content and hooks.

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
