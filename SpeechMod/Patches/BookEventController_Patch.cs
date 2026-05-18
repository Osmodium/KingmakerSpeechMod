using HarmonyLib;
using Kingmaker.UI.BookEvent;
using SpeechMod.Unity.Extensions;
#if DEBUG
using Kingmaker;
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class BookEventController_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(BookEventController), nameof(BookEventController.SetPage))]
    public static void SetPage_Postfix(BookEventController __instance)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        var areaName = Game.Instance.CurrentlyLoadedArea.AreaName;
        Debug.Log($"{nameof(BookEventController)}_SetPage_Postfix @ {areaName}");
#endif

        for (var i = 0; i < __instance.Cues.content.childCount; i++)
        {
            __instance.Cues.content.GetChild(i).HookupTextToSpeechOnTransform();
        }
    }
}