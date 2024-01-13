using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permadeath.Patches
{

    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {



        [HarmonyPatch("HideHUD")]
        [HarmonyPrefix]
        static bool HideHUDPatch(bool hide)
        {

            var self = GameNetworkManager.Instance.localPlayerController;

            if (self.isPlayerDead && hide == false)
            {
                return false;
            }

            return true;


        }


    }
}
