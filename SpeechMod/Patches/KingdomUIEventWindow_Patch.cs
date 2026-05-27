using HarmonyLib;
using Kingmaker.UI.Kingdom;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public class KingdomUIEventWindow_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(KingdomUIEventWindow), nameof(KingdomUIEventWindow.Initialize))]
    public static void Initialize_Postfix(KingdomUIEventWindow __instance)
    {
        if (!Main.Enabled)
            return;
        
#if DEBUG
        Debug.Log($"{nameof(KingdomUIEventWindow)}_{nameof(Initialize_Postfix)}");
#endif
        
        __instance.m_DependsQuestDescription.HookupTextToSpeech();
        __instance.m_Description.HookupTextToSpeech();
        __instance.m_Expire.HookupTextToSpeech();
        __instance.m_Title.HookupTextToSpeech();
        __instance.m_TitleNavigation.HookupTextToSpeech();
        __instance.m_LeaderDescription.HookupTextToSpeech();
        __instance.m_Type.HookupTextToSpeech();
    }
}