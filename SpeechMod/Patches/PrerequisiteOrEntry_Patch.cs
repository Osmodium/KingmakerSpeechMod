using HarmonyLib;
using Kingmaker.UI.Tooltip;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class PrerequisiteOrEntry_Patch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(PrerequisiteOrEntry), nameof(PrerequisiteOrEntry.Set), typeof(string), typeof(bool))]
	public static void Init_Postfix(PrerequisiteOrEntry __instance, string text, bool done)
	{
		if (!Main.Enabled)
		{
			return;
		}

#if DEBUG
		Debug.Log($"{nameof(PrerequisiteOrEntry)}_{nameof(Init_Postfix)}");
#endif

		__instance.m_Text.HookupTextToSpeech();
	}
}