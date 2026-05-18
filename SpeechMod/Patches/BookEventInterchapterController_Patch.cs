using HarmonyLib;
using Kingmaker.UI.BookEvent;
using SpeechMod.Unity;
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
        __instance.m_Answer.OptionName.TryAddButtonToTextMeshPro("SpeechMod_BookEventInterchapterButton", anchoredPosition: new Vector2(0f, -25f), scale: new Vector3(0.6f, 0.6f, 1.0f));
    }
}