using HarmonyLib;
using Kingmaker.UI.SaveLoadWindow;
using SpeechMod.Unity.Extensions;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class SaveLoadWindow_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SaveLoadWindow), nameof(SaveLoadWindow.Show), typeof(SaveLoadWindow.ScreenType))]
    public static void Show_Postfix(SaveLoadWindow __instance, SaveLoadWindow.ScreenType screenType)
    {
        if (!Main.Enabled)
        {
            return;
        }

#if DEBUG
        Debug.Log($"{nameof(SaveLoadWindow)}_{nameof(Show_Postfix)}");
#endif

        __instance.m_NameLabel.HookupTextToSpeech();
        __instance.m_LocationLabel.HookupTextToSpeech();
        __instance.m_SaveDateTimeLabel.HookupTextToSpeech();
        __instance.m_TimeInGameLabel.HookupTextToSpeech();
        __instance.m_DescriptionLabel.HookupTextToSpeech();
    }
}