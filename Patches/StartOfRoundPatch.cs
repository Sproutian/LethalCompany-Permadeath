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
            if (daysToDeadline == 0 || (daysToDeadline == 1 && ___shipIsLeaving == true))
            {

                return true;

            }

            //FileLog.Log("Reviving players (days til deadline: "+ daysToDeadline + ")");

            return false;
        }


    }
}
