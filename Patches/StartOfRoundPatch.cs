using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Permadeath.Patches
{

    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {

        [HarmonyPatch("ReviveDeadPlayers")]
        [HarmonyPrefix]
        static bool revivePlayersPatch(ref bool ___shipIsLeaving)
        {
            TimeOfDay timeOfDay = UnityEngine.Object.FindObjectOfType<TimeOfDay>();
            int daysToDeadline = timeOfDay.daysUntilDeadline;
            if (daysToDeadline == 0 || (daysToDeadline == 1 && ___shipIsLeaving == true) || (daysToDeadline == 3 && ___shipIsLeaving == false))
            {
                FileLog.Log("Not reviving players (days til deadline: " + daysToDeadline + ")");

                return true;

            }

            FileLog.Log("Reviving players (days til deadline: "+ daysToDeadline + ")");

            return false;
        }


        [HarmonyPatch("EndOfGame")]
        [HarmonyPostfix]
        static void endofgamePostfix(ref StartOfRound __instance, ref PlayerControllerB[] ___allPlayerScripts)
        {


            if (__instance.IsOwner)
            {

                bool runOver = true;
                for (int i = 0; i < ___allPlayerScripts.Length; i++)
                {
                    if (!___allPlayerScripts[i].isPlayerDead && !___allPlayerScripts[i].isPlayerControlled)
                    {
                        continue;
                    }
                    else if (!___allPlayerScripts[i].isPlayerDead)
                    {
                        runOver = false;
                    }
                }
                if (runOver)
                {
                    __instance.FirePlayersAfterDeadlineClientRpc(new int[4] { __instance.gameStats.daysSpent, __instance.gameStats.scrapValueCollected, __instance.gameStats.deaths, __instance.gameStats.allStepsTaken });
                }
            }
        }


        //[HarmonyPatch("ResetShip")]
        //[HarmonyPostfix]
        //static void resetShipPostfix(ref StartOfRound __instance)
        //{
        //    __instance.ReviveDeadPlayers();
        //}

    }


}
