using HarmonyLib;
using Kingmaker.UI.BookEvent;
using SpeechMod.Unity.Extensions;
#if DEBUG
using Kingmaker;
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class BookEventInterchapterController_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BookEventInterchapterController), nameof(BookEventInterchapterController.SetPage))]
    public static void SetPage_Postfix(BookEventInterchapterController __instance)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        var areaName = Game.Instance.CurrentlyLoadedArea.AreaName;
        Debug.Log($"{nameof(BookEventInterchapterController)}_SetPage_Postfix @ {areaName}");
#endif

        __instance.m_Cue.HookupTextToSpeech();
    }
}