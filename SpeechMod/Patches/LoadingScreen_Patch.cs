using HarmonyLib;
using Kingmaker.UI.LoadingScreen;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class LoadingScreen_Patch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(LoadingScreen), nameof(LoadingScreen.ShowLoadingScreen))]
    public static void ShowLoadingScreen_Postfix(LoadingScreen __instance)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(LoadingScreen)}_{nameof(ShowLoadingScreen_Postfix)}");
#endif

        if (Main.Settings?.AutoStopPlaybackOnLoading == false)
	        return;

        Main.Speech?.Stop();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(LoadingScreen), nameof(LoadingScreen.Awake))]
    public static void Awake_Postfix(LoadingScreen __instance)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(LoadingScreen)}_{nameof(Awake_Postfix)}");
#endif

		__instance?.Hint.HookupTextToSpeech();
	}
}