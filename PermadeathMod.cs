using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Permadeath.Patches;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permadeath
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class PermadeathMod : BaseUnityPlugin
    {
        private const string modGUID = "Sproutian.Permadeath";
        private const string modName = "Permadeath";
        private const string modVersion = "1.0.0.0";
        private readonly Harmony harmony = new Harmony(modGUID);

        private static PermadeathMod Instance;


        internal ManualLogSource mls;


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Sproutian's Permadeath loaded");

            harmony.PatchAll(typeof(PermadeathMod));
            harmony.PatchAll(typeof(StartOfRoundPatch));
            harmony.PatchAll(typeof(HUDManagerPatch));
        }

    }

    
}
