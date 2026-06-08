# Full Deck Prune Mod Design

## Player-Facing Behavior

At the start of a new run, the chosen base character starts with:

- All normal starter cards that character would normally receive.
- One copy of every other card belonging to that character.

After every combat victory:

- The player is prompted to choose one card from the current master deck.
- The chosen card is permanently removed from the run deck.
- The run then continues normally.

The result is a run that starts huge and gets refined one card at a time.

## Initial Scope

Support the official base characters first. Avoid custom characters until the base flow is stable.

The first implementation should not add new cards, relics, UI art, or balance rules. It should only modify:

- Initial deck generation.
- Post-combat reward flow.
- Card removal from the master deck.

## Rules To Decide Before Coding

These are small design choices, but they affect implementation and balance:

1. Starter duplicates: keep the normal starter deck counts, then add one copy of each non-starter card.
2. Upgraded cards: start all added cards unupgraded.
3. Colorless cards: exclude them for v1.
4. Curse/status cards: exclude them for v1 unless they are explicitly part of a character card pool.
5. Removal timing: after combat rewards, or before combat rewards.
6. Boss rewards: still remove one card after boss combats.
7. Empty/near-empty deck: if the deck has one or zero removable cards, skip the removal screen.

Recommended v1 choices:

- Keep starter counts.
- Add one unupgraded copy of each character card.
- Exclude colorless, curses, and statuses.
- Trigger removal after combat victory before leaving the combat reward flow, if the game exposes a clean hook.

## Technical Shape

Use an empty Slay the Spire 2 mod template with BaseLib as a dependency.

Expected pieces:

- Manifest JSON: mod id, display name, author, dependency on BaseLib if used.
- Main mod class: initialization and patch registration.
- New-run patch/hook: replace or append to the starting deck after the character is known.
- Card pool helper: collect all cards for the current character, excluding non-character cards.
- Combat-end patch/hook: enqueue a card-removal screen or equivalent reward action.
- Removal guard: prevent duplicate removal prompts if combat-end hooks fire more than once.

## Implementation Pseudocode

```csharp
OnRunStarted(character):
    deck = player.MasterDeck
    starterIds = deck.Select(card => card.Id)
    allCharacterCards = CardLibrary.GetCardsForCharacter(character)

    foreach card in allCharacterCards:
        if card.Type is Curse or Status:
            continue
        if card.Color is Colorless:
            continue
        if starterIds.Contains(card.Id):
            continue

        deck.Add(CreateUnupgradedCopy(card.Id))

OnCombatVictory():
    if player.MasterDeck.Count <= 1:
        return
    if removalAlreadyQueuedForThisCombat:
        return

    QueueRemoveOneCardScreen(player.MasterDeck)

OnCardChosen(card):
    player.MasterDeck.Remove(card)
```

The exact API names will need to be aligned with the current decompiled game symbols and BaseLib hooks.

## Build Checklist

1. Install .NET SDK.
2. Install STS2 templates:

```powershell
dotnet new install Alchyr.Sts2.Templates
```

3. Create an empty STS2 mod project.
4. Put latest BaseLib `.dll`, `.pck`, and `.json` in the game `mods` folder.
5. Build once and confirm the mod appears in Settings -> Mod Settings.
6. Add the full-deck startup behavior.
7. Add the post-combat removal behavior.
8. Test with each base character.

## Test Plan

Manual checks:

- Mod appears in the STS2 mod list.
- New run starts with a larger deck.
- Starter cards retain their normal counts.
- Each non-starter character card appears exactly once.
- Colorless/curses/status cards are not added.
- After a normal hallway fight, exactly one remove-card prompt appears.
- Removing a card changes the master deck permanently.
- Elite and boss fights also trigger removal once.
- Saving and reloading does not duplicate the full-deck injection.

Potential automated checks can be added later if a stable test harness emerges.
