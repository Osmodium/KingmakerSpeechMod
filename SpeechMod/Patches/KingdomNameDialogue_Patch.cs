using HarmonyLib;
using Kingmaker.UI.Kingdom;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public class KingdomNameDialogue_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(KingdomNameDialogue), nameof(KingdomNameDialogue.Initialize))]
    public static void Initialize_Postfix(KingdomNameDialogue __instance)
    {
        if (!Main.Enabled)
            return;
        
#if DEBUG
        Debug.Log($"{nameof(KingdomNameDialogue)}_{nameof(Initialize_Postfix)}");
#endif
        
        __instance.m_Header.HookupTextToSpeech();
        __instance.m_Hint.HookupTextToSpeech();
    }
}