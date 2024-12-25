using HarmonyLib;
using Kingmaker.UI.LevelUp;
using Kingmaker.UI.LevelUp.NewGame;
using SpeechMod.Unity.Extensions;
using UnityEngine;

namespace SpeechMod.Patches;

public static class NewGameWinPregenSelectorItem_Patch
{
    private const string DESCRIPTION_PATH = "/Canvas/NewGameWindow/Body/Content/Pregen/PregenSelector/Description/DescriptionView/Viewport/Content/Content/Description (1)";

    [HarmonyPostfix]
    [HarmonyPatch(typeof(NewGameWinPregenSelectorItem), nameof(NewGameWinPregenSelectorItem.Init), typeof(ChargenUnit))]
    public static void Init_Postfix(NewGameWinPregenSelectorItem __instance, ChargenUnit chargenUnit)
    {
        if (!Main.Enabled)
        {
            return;
        }
#if DEBUG
        Debug.Log($"{nameof(NewGameWinPregenSelectorItem)}_{nameof(Init_Postfix)}");
#endif
        __instance.m_PregenClass.HookupTextToSpeech();
        __instance.m_PregenName.HookupTextToSpeech();

        __instance.transform.Find(DESCRIPTION_PATH).HookupTextToSpeechOnTransform();
    }
}