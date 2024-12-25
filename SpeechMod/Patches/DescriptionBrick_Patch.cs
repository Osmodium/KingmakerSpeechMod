using HarmonyLib;
using Kingmaker.UI.Tooltip;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class DescriptionBrick_Patch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(DescriptionBrick), nameof(DescriptionBrick.Init))]
	public static void Init_Postfix(DescriptionBrick __instance)
	{
		if (!Main.Enabled)
		{
			return;
		}

#if DEBUG
		Debug.Log($"{nameof(DescriptionBrick)}_{nameof(Init_Postfix)}");
#endif

		__instance.TextFields.ToArray().HookupTextToSpeech();
	}
}
