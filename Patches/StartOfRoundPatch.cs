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
        static bool revivePlayersPatch(ref bool ___shipIsLeaving, ref PlayerControllerB[] ___allPlayerScripts, ref StartOfRound __instance, out PlayerControllerB[] __state)
        {

            __state = ___allPlayerScripts;

            TimeOfDay timeOfDay = UnityEngine.Object.FindObjectOfType<TimeOfDay>();
            int daysToDeadline = timeOfDay.daysUntilDeadline;
            if (daysToDeadline == 0 || (daysToDeadline == 1 && ___shipIsLeaving == true))
            {

                return true;

            }
            else
            {
                FileLog.Log("Doing the ELSE");
                List<PlayerControllerB> playersToRespawn = new List<PlayerControllerB>();
                DeadBodyInfo[] bodies = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();
                FileLog.Log("# bodies: "+bodies.Length);

                for (int i = 0; i < bodies.Length; i++)
                {
                    FileLog.Log("Checking a body");
                    if (bodies[i].isInShip)
                    {
                        FileLog.Log("Found a body in ship");
                        for (int v = 0; v < ___allPlayerScripts.Length; v++)
                        {
                            if (___allPlayerScripts[v].isPlayerDead && ___allPlayerScripts[v].deadBody == bodies[i])
                            {
                                FileLog.Log("Found the body's player");

                                playersToRespawn.Add(___allPlayerScripts[v]);

                                v = playersToRespawn.Count;
                            }
                        }
                        

                    }
                }
                FileLog.Log("----Running main respawn code-----");


                // code copied & modified from original revive players

                PlayerControllerB[] allPlayerScripts = new PlayerControllerB[playersToRespawn.Count];

                for(int i = 0; i < playersToRespawn.Count; i++)
                {
                    allPlayerScripts[i] = playersToRespawn[i];
                }

                FileLog.Log("# Players to respawn: "+playersToRespawn.Count);
                __instance.allPlayersDead = false;
                for (int i = 0; i < allPlayerScripts.Length; i++)
                {
                    FileLog.Log("Respawning a player");
                    Debug.Log("Reviving players A");
                    allPlayerScripts[i].ResetPlayerBloodObjects(allPlayerScripts[i].isPlayerDead);
                    if (!allPlayerScripts[i].isPlayerDead && !allPlayerScripts[i].isPlayerControlled)
                    {
                        continue;
                    }
                    allPlayerScripts[i].isClimbingLadder = false;
                    allPlayerScripts[i].ResetZAndXRotation();
                    allPlayerScripts[i].thisController.enabled = true;
                    allPlayerScripts[i].health = 100;
                    allPlayerScripts[i].disableLookInput = false;
                    Debug.Log("Reviving players B");
                    if (allPlayerScripts[i].isPlayerDead)
                    {
                        allPlayerScripts[i].isPlayerDead = false;
                        allPlayerScripts[i].isPlayerControlled = true;
                        allPlayerScripts[i].isInElevator = true;
                        allPlayerScripts[i].isInHangarShipRoom = true;
                        allPlayerScripts[i].isInsideFactory = false;
                        allPlayerScripts[i].wasInElevatorLastFrame = false;
                        __instance.SetPlayerObjectExtrapolate(enable: false);
                        allPlayerScripts[i].TeleportPlayer(allPlayerScripts[i].deadBody.transform.position);
                        allPlayerScripts[i].setPositionOfDeadPlayer = false;
                        allPlayerScripts[i].DisablePlayerModel(allPlayerScripts[i].gameObject, enable: true, disableLocalArms: true);
                        allPlayerScripts[i].helmetLight.enabled = false;
                        Debug.Log("Reviving players C");
                        allPlayerScripts[i].Crouch(crouch: false);
                        allPlayerScripts[i].criticallyInjured = false;
                        if (allPlayerScripts[i].playerBodyAnimator != null)
                        {
                            allPlayerScripts[i].playerBodyAnimator.SetBool("Limp", value: false);
                        }
                        allPlayerScripts[i].bleedingHeavily = false;
                        allPlayerScripts[i].activatingItem = false;
                        allPlayerScripts[i].twoHanded = false;
                        allPlayerScripts[i].inSpecialInteractAnimation = false;
                        allPlayerScripts[i].disableSyncInAnimation = false;
                        allPlayerScripts[i].inAnimationWithEnemy = null;
                        allPlayerScripts[i].holdingWalkieTalkie = false;
                        allPlayerScripts[i].speakingToWalkieTalkie = false;
                        Debug.Log("Reviving players D");
                        allPlayerScripts[i].isSinking = false;
                        allPlayerScripts[i].isUnderwater = false;
                        allPlayerScripts[i].sinkingValue = 0f;
                        allPlayerScripts[i].statusEffectAudio.Stop();
                        allPlayerScripts[i].DisableJetpackControlsLocally();
                        allPlayerScripts[i].health = 100;
                        Debug.Log("Reviving players E");
                        allPlayerScripts[i].mapRadarDotAnimator.SetBool("dead", value: false);
                        if (allPlayerScripts[i].IsOwner)
                        {
                            HUDManager.Instance.gasHelmetAnimator.SetBool("gasEmitting", value: false);
                            allPlayerScripts[i].hasBegunSpectating = false;
                            HUDManager.Instance.RemoveSpectateUI();
                            HUDManager.Instance.gameOverAnimator.SetTrigger("revive");
                            allPlayerScripts[i].hinderedMultiplier = 1f;
                            allPlayerScripts[i].isMovementHindered = 0;
                            allPlayerScripts[i].sourcesCausingSinking = 0;
                            Debug.Log("Reviving players E2");
                            allPlayerScripts[i].reverbPreset = __instance.shipReverb;
                        }
                    }
                    Debug.Log("Reviving players F");
                    SoundManager.Instance.earsRingingTimer = 0f;
                    allPlayerScripts[i].voiceMuffledByEnemy = false;
                    SoundManager.Instance.playerVoicePitchTargets[i] = 1f;
                    SoundManager.Instance.SetPlayerPitch(1f, i);
                    if (allPlayerScripts[i].currentVoiceChatIngameSettings == null)
                    {
                        __instance.RefreshPlayerVoicePlaybackObjects();
                    }
                    if (allPlayerScripts[i].currentVoiceChatIngameSettings != null)
                    {
                        if (allPlayerScripts[i].currentVoiceChatIngameSettings.voiceAudio == null)
                        {
                            allPlayerScripts[i].currentVoiceChatIngameSettings.InitializeComponents();
                        }
                        if (allPlayerScripts[i].currentVoiceChatIngameSettings.voiceAudio == null)
                        {
                            return false;
                        }
                        allPlayerScripts[i].currentVoiceChatIngameSettings.voiceAudio.GetComponent<OccludeAudio>().overridingLowPass = false;
                    }
                    Debug.Log("Reviving players G");
                }
                FileLog.Log("Revive players G (Should see that player is dead below this, unless I'm supposed to be respawned rn)");
                PlayerControllerB playerControllerB = GameNetworkManager.Instance.localPlayerController;
                if (!playerControllerB.isPlayerDead)
                {
                    playerControllerB.bleedingHeavily = false;
                    playerControllerB.criticallyInjured = false;
                    playerControllerB.playerBodyAnimator.SetBool("Limp", value: false);
                    playerControllerB.health = 100;
                    HUDManager.Instance.UpdateHealthUI(100, hurtPlayer: false);
                    playerControllerB.spectatedPlayerScript = null;
                    HUDManager.Instance.audioListenerLowPass.enabled = false;
                    __instance.SetSpectateCameraToGameOverMode(enableGameOver: false, playerControllerB);
                }
                else
                {
                    FileLog.Log("Player is dead so skippy!");
                }
                Debug.Log("Reviving players H");
                RagdollGrabbableObject[] array = UnityEngine.Object.FindObjectsOfType<RagdollGrabbableObject>();
                for (int j = 0; j < array.Length; j++)
                {
                    if (!array[j].isHeld)
                    {
                        if (__instance.IsServer)
                        {
                            if (array[j].NetworkObject.IsSpawned)
                            {
                                array[j].NetworkObject.Despawn();
                            }
                            else
                            {
                                UnityEngine.Object.Destroy(array[j].gameObject);
                            }
                        }
                    }
                    else if (array[j].isHeld && array[j].playerHeldBy != null)
                    {
                        array[j].playerHeldBy.DropAllHeldItems();
                    }
                }
                DeadBodyInfo[] array2 = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();
                for (int k = 0; k < array2.Length; k++)
                {
                    UnityEngine.Object.Destroy(array2[k].gameObject);
                }
                __instance.livingPlayers = __instance.connectedPlayersAmount + 1;
                __instance.allPlayersDead = false;
                __instance.UpdatePlayerVoiceEffects();
                __instance.shipAnimator.ResetTrigger("ShipLeave");



                //___allPlayerScripts = new PlayerControllerB[playersToRespawn.Count];

                //for(int i = 0; i < playersToRespawn.Count; i++)
                //{
                //    ___allPlayerScripts[i] = playersToRespawn[i];
                //}

                //FileLog.Log("There are " + playersToRespawn.Count + " people to be respawned as of prefix.");

            }

            return false;
        }

        [HarmonyPatch("ReviveDeadPlayers")]
        [HarmonyPostfix]
        static void revivePlayersPostfix(ref PlayerControllerB[] ___allPlayerScripts, PlayerControllerB[] __state)
        {

            FileLog.Log("Resetting ___allPlayerScripts (size " + ___allPlayerScripts.Length +") to __state (size "+__state.Length+")");
            ___allPlayerScripts = __state;


        }



        /*DeadBodyInfo[] array = UnityEngine.Object.FindObjectsOfType<DeadBodyInfo>();
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].isInShip)
			{
				num++;
			}
		}*/


    }
}
