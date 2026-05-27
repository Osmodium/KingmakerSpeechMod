using HarmonyLib;
using Kingmaker.UI.Kingdom;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public class KingdomUIEventWindowFooter_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(KingdomUIEventWindowFooter), nameof(KingdomUIEventWindowFooter.Initialize))]
    public static void Initialize_Postfix(KingdomUIEventWindowFooter __instance)
    {
        if (!Main.Enabled)
            return;
        
#if DEBUG
        Debug.Log($"{nameof(KingdomUIEventWindowFooter)}_{nameof(Initialize_Postfix)}");
#endif
        
        __instance.m_DropLabel.HookupTextToSpeech();
        __instance.m_GoThroneRoomLabel.HookupTextToSpeech();
        __instance.m_TaskWasIgnore.HookupTextToSpeech();
    }
}