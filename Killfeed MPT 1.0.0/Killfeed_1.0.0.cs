using Aki.Reflection.Patching;
using EFT;
using HarmonyLib;
using MPT.Core.Coop.Players;
using System.Reflection;
using UnityEngine;

namespace Killfeed_MPT
{
    public class Killfeed : ModulePatch
    {
        public static event System.Action<Player> OnPersonKilled;

        private static KillfeedUI killfeedUI; // Instance of KillfeedUI

        protected override MethodBase GetTargetMethod() =>
            AccessTools.Method(typeof(Player), "OnBeenKilledByAggressor");

        [PatchPostfix]
        public static void PatchPostfix(Player __instance, Player aggressor, DamageInfo damageInfo, EBodyPart bodyPart)
        {
            if (__instance == null)
                return;

            OnPersonKilled?.Invoke(__instance);

            var attackerField = AccessTools.Field(typeof(Player), "LastAggressor");
            Player attacker = attackerField.GetValue(__instance) as Player;
            string weaponName = GClass1853.Localized(damageInfo.Weapon.ShortName);

            // Determine if attacker is a player or a bot
            bool isAttackerPlayer = attacker != null && (attacker.GroupId == "MPT");
            string attackerName = GeneratePlayerNameWithSide(attacker);

            // Determine if __instance is a player or a bot
            bool isVictimPlayer = __instance != null && (__instance.GroupId == "MPT");
            string victimName = GeneratePlayerNameWithSide(__instance);

            // Construct kill message with appropriate color for attacker and victim
            string killMessage = attacker != null ?
                $"{ColorText(attackerName, isAttackerPlayer)} {weaponName} {ColorText(victimName, isVictimPlayer)}" :
                $"{ColorText(victimName, isVictimPlayer)} has died";

            // Instantiate KillfeedUI if not already instantiated
            if (killfeedUI == null)
            {
                GameObject killfeedUIObject = new GameObject("KillfeedUI");
                killfeedUI = killfeedUIObject.AddComponent<KillfeedUI>();
            }

            // Display kill message using KillfeedUI instance
            killfeedUI.DisplayKillMessage(killMessage);
        }

        private static string ColorText(string text, bool isPlayer)
        {
            if (isPlayer)
                return $"<color=blue>{text}</color>"; // Player
            else
                return $"<color=red>{text}</color>"; // Bot
        }

        private static string GeneratePlayerNameWithSide(Player player)
        {
            if (player == null)
                return "";

            string side = player.AIData.IAmBoss ? "Boss" :
                          player.Side != EPlayerSide.Savage ? player.Side.ToString() :
                          "Scav";

            return $"[{side}] {player.Profile.GetCorrectedNickname()}";
        }
    }
}
