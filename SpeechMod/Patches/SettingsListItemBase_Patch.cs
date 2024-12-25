using HarmonyLib;
using Kingmaker.Localization;
using Kingmaker.UI.SettingsUI;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class SettingsListItemBase_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SettingsListItemBase), nameof(SettingsListItemBase.SetUp), typeof(SettingsEntityBase))]
    public static void SetUp_Postfix(SettingsListItemBase __instance)
    {
        if (!Main.Enabled)
        {
            return;
        }

#if DEBUG
        Debug.Log($"{nameof(SettingsListItemBase)}_{nameof(SetUp_Postfix)}");
#endif

        var fullText = __instance.DescriptionLabel.text + ". " + LocalizationManager.CurrentPack?.GetText(__instance.Data.TooltipDescription.Key);
        __instance.DescriptionLabel.HookupTextToSpeech(fullText);
    }
}