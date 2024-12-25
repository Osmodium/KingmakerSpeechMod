using HarmonyLib;
using Kingmaker.UI.ServiceWindow.CharacterScreen;
using SpeechMod.Unity.Extensions;
using UnityEngine.UI;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class CharSAlignment_Patch
{
    private const string GRID_PATH = "/StaticCanvas/ServiceWindow/CharacterScreen/AlignmentBlock/Map/Grid";

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharSAlignment), nameof(CharSAlignment.Init))]
    public static void Init_Postfix(CharSAlignment __instance)
    {
        if (!Main.Enabled)
        {
            return;
        }

#if DEBUG
        Debug.Log($"{nameof(CharSAlignment)}_{nameof(Init_Postfix)}");
#endif

        var gridTransform = __instance.transform.TryFind(GRID_PATH);
        if (gridTransform == null)
        {
            return;
        }
        gridTransform.GetComponent<Image>().raycastTarget = false;

        __instance.m_CurrentValue.HookupTextToSpeech();
    }
}
