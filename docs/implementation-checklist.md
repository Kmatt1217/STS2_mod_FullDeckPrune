# Implementation Checklist

Use this after the .NET SDK and STS2 mod template are installed.

## Project Setup

- Create the project from the Empty Slay the Spire 2 Mod template.
- Keep the project name/id as `FullDeckPrune`.
- Copy the manifest values from `FullDeckPrune.json` into the generated manifest if the template creates a new one.
- Confirm `BaseLib` is listed as a dependency.
- Build once before adding gameplay changes.

## Step 1: Mod Load Smoke Test

- Add a log line in the mod initialization path.
- Build.
- Launch STS2 with mods enabled.
- Confirm `Full Deck Prune` appears in the mod list.
- Confirm the log line appears in the relevant game/mod log.

## Step 2: Full Starting Deck

Find the hook/patch that runs after:

- A new run has been created.
- The selected character is known.
- The player's master deck exists.

Implementation intent:

- Read the selected base character/card color.
- Get all cards matching that character.
- Exclude starter cards already present in the deck.
- Exclude colorless, curse, and status cards for v1.
- Add one unupgraded copy of each remaining card.
- Mark the run as already initialized to avoid duplication after save/load.

## Step 3: Post-Combat Removal

Find the hook/patch that runs after combat victory or before leaving the combat reward flow.

Implementation intent:

- Check the combat has not already queued a removal prompt.
- Check the master deck has more than one removable card.
- Open or enqueue the game's normal card-removal selection UI if available.
- Remove the selected card from the master deck.

## Step 4: Save/Load Safety

- Store a small per-run flag indicating full-deck initialization happened.
- Store or derive a per-combat guard so the removal screen cannot appear twice.
- Test by saving and reloading during a run.

## Hook/API Questions To Resolve

- Exact class/method for new run creation.
- Exact class/method for combat victory/reward transition.
- Exact API for creating card instances from card ids/models.
- Exact API for showing card removal UI.
- Whether BaseLib exposes a cleaner run-start or combat-end hook than raw Harmony patches.
