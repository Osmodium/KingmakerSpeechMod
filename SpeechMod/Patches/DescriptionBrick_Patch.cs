using HarmonyLib;
using Kingmaker.UI.Tooltip;
using SpeechMod.Unity.Extensions;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class DescriptionBrick_Patch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(DescriptionBrick), "Init")]
	public static void Init_Patch(DescriptionBrick __instance)
	{
		if (!Main.Enabled)
		{
			return;
		}
#if DEBUG
		Debug.Log($"{nameof(DescriptionBrick)}_Init_Patch");
#endif

		__instance.TextFields.ToArray().HookupTextToSpeech();
	}
}