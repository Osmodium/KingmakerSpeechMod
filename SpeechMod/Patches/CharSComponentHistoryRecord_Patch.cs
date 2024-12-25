using HarmonyLib;
using Kingmaker.UI.ServiceWindow.CharacterScreen;
using Kingmaker.UnitLogic.Alignments;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class CharSComponentHistoryRecord_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharSComponentHistoryRecord), nameof(CharSComponentHistoryRecord.SetData), typeof(AlignmentShiftDirection), typeof(string))]
    public static void Init_Postfix(CharSComponentHistoryRecord __instance, AlignmentShiftDirection direction, string desc)
    {
        if (!Main.Enabled)
        {
            return;
        }

#if DEBUG
        Debug.Log($"{nameof(CharSComponentHistoryRecord)}_{nameof(Init_Postfix)}");
#endif

        __instance.Shift.HookupTextToSpeech();
        __instance.Description.HookupTextToSpeech();
    }
}