using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Runs;
using System.Collections.Generic;
using System.Linq;

namespace FullDeckPrune.FullDeckPruneCode.Patches;

[HarmonyPatch(typeof(RunState))]
internal static class RunStatePatches
{
    private static readonly HashSet<RunState> InitializedRuns = [];
    private static readonly HashSet<string> AlwaysExcludedCardEntries = ["BREAK", "CORRUPTION"];

    [HarmonyPatch(nameof(RunState.CreateForNewRun))]
    [HarmonyPostfix]
    private static void AfterCreateForNewRun(RunState __result)
    {
        if (!InitializedRuns.Add(__result))
        {
            return;
        }

        foreach (var player in __result.Players)
        {
            var beforeCount = player.Deck.Cards.Count;
            var removedBasicDuplicateCount = RemoveDuplicateBasicStrikeAndDefendCards(player.Deck);
            var existingCardIds = player.Deck.Cards.Select(card => card.Id).ToHashSet();
            var isMultiplayer = __result.Players.Count > 1;
            var addedCount = 0;
            var excludedCount = 0;

            foreach (var canonicalCard in player.Character.CardPool.AllCards)
            {
                if (existingCardIds.Contains(canonicalCard.Id))
                {
                    continue;
                }

                if (ShouldExcludeFromStartingDeck(canonicalCard, isMultiplayer))
                {
                    excludedCount++;
                    continue;
                }

                try
                {
                    var card = __result.CreateCard(canonicalCard, player);
                    player.Deck.AddInternal(card, player.Deck.Cards.Count, true);
                    existingCardIds.Add(canonicalCard.Id);
                    addedCount++;
                }
                catch (Exception ex)
                {
                    MainFile.Logger.Error($"Failed to add {canonicalCard.Id} to starting deck: {ex}");
                }
            }

            MainFile.Logger.Info(
                $"Expanded starting deck for {player.Character.Id}: {beforeCount} -> {player.Deck.Cards.Count} cards (+{addedCount}, removed basic duplicates={removedBasicDuplicateCount}, excluded={excludedCount}, multiplayer={isMultiplayer}).");
        }
    }

    private static int RemoveDuplicateBasicStrikeAndDefendCards(CardPile deck)
    {
        var keptBasicCardIds = new HashSet<MegaCrit.Sts2.Core.Models.ModelId>();
        var duplicateBasicCards = new List<MegaCrit.Sts2.Core.Models.CardModel>();

        foreach (var card in deck.Cards)
        {
            if (!card.IsBasicStrikeOrDefend)
            {
                continue;
            }

            if (keptBasicCardIds.Add(card.Id))
            {
                continue;
            }

            duplicateBasicCards.Add(card);
        }

        foreach (var card in duplicateBasicCards)
        {
            deck.RemoveInternal(card, true);
        }

        return duplicateBasicCards.Count;
    }

    private static bool ShouldExcludeFromStartingDeck(
        MegaCrit.Sts2.Core.Models.CardModel card,
        bool isMultiplayer)
    {
        if (card.Type is CardType.Curse or CardType.Status)
        {
            return true;
        }

        if (card.Rarity == CardRarity.Ancient)
        {
            return true;
        }

        if (AlwaysExcludedCardEntries.Contains(card.Id.Entry))
        {
            return true;
        }

        return card.MultiplayerConstraint switch
        {
            CardMultiplayerConstraint.MultiplayerOnly => !isMultiplayer,
            CardMultiplayerConstraint.SingleplayerOnly => isMultiplayer,
            _ => false,
        };
    }
}
