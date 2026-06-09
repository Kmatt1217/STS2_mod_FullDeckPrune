using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using System.Collections.Generic;
using System.Linq;

namespace FullDeckPrune.FullDeckPruneCode.Patches;

[HarmonyPatch(typeof(RewardsSet))]
internal static class RewardsSetPatches
{
    [HarmonyPatch("GenerateRewardsFor")]
    [HarmonyPostfix]
    private static void AddPostCombatCardRemoval(Player player, AbstractRoom room, List<Reward> __result)
    {
        if (room is not CombatRoom)
        {
            return;
        }

        var cardRewards = __result.Where(reward => reward is CardReward).ToList();
        var cardRewardsToKeep = cardRewards.Count > 1 ? 1 : 0;
        var removedCardRewardCount = 0;

        foreach (var cardReward in cardRewards.Skip(cardRewardsToKeep))
        {
            if (__result.Remove(cardReward))
            {
                removedCardRewardCount++;
            }
        }

        var removableCount = player.Deck.Cards.Count(card => card.IsRemovable);
        if (removableCount <= 1)
        {
            MainFile.Logger.Info(
                $"Kept {cardRewardsToKeep} card rewards, removed {removedCardRewardCount}, and skipped post-combat card removal reward for {player.Character.Id}: removable cards={removableCount}.");
            return;
        }

        __result.Add(new CardRemovalReward(player));
        MainFile.Logger.Info(
            $"Kept {cardRewardsToKeep} card rewards, removed {removedCardRewardCount}, and added post-combat card removal reward for {player.Character.Id}: removable cards={removableCount}.");
    }
}
