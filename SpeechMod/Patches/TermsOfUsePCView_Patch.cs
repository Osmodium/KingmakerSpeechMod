using HarmonyLib;
using Kingmaker.UI;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public class TermsOfUsePCView_Patch
{
	[HarmonyPatch(typeof(TermsOfUseController), nameof(TermsOfUseController.OnShow))]
    [HarmonyPostfix]
	public static void OnShot_Postfix(TermsOfUseController __instance)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(TermsOfUseController)}_{nameof(OnShot_Postfix)}");
#endif

        __instance.m_Title.HookupTextToSpeech();
        __instance.m_Licence.HookupTextToSpeech();
        __instance.m_SubLicence.HookupTextToSpeech();
    }
}