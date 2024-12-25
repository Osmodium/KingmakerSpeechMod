using HarmonyLib;
using Kingmaker.UI;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public class DialogMessageBox_Patch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(DialogMessageBox), nameof(DialogMessageBox.Initialize))]
    public static void Initialize_Postfix(DialogMessageBox __instance)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(DialogMessageBox)}_{nameof(Initialize_Postfix)}");
#endif


        __instance.m_Messagelabel.HookupTextToSpeech();
    }
}