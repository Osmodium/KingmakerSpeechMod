using HarmonyLib;
using Kingmaker.Localization;
using SpeechMod.Localization;

namespace SpeechMod.Patches;

[HarmonyPatch]
public class LocalizationManager_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(LocalizationManager), nameof(LocalizationManager.OnLocaleChanged))]
    public static void OnLocaleChanged_Postfix()
    {
        ModLocalizationManager.ApplyLocalization(LocalizationManager.CurrentLocale);
    }
}


