using HarmonyLib;
using Kingmaker.UI.Kingdom;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public class KingdomUILeaderController_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(KingdomUILeaderController), nameof(KingdomUILeaderController.Initialize))]
    public static void Initialize_Posfix(KingdomUILeaderController __instance)
    {
        if (!Main.Enabled)
            return;
        
#if DEBUG
        Debug.Log($"{nameof(KingdomUILeaderController)}_{nameof(Initialize_Postfix)}");
#endif
        
        __instance.m_Title.HookupTextToSpeech();
        __instance.m_CompanionDescription.HookupTextToSpeech();
        __instance.m_NavigationTitle.HookupTextToSpeech();
        __instance.m_NoCompanionsLabel.HookupTextToSpeech();
        __instance.m_RoleDescription.HookupTextToSpeech();
    }
}