using HarmonyLib;
using Kingmaker.UI.LevelUp.NewGame;
using SpeechMod.Unity.Extensions;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class NewGameWinPhaseStory_Patch
{
	[HarmonyPatch(typeof(NewGameWinPhaseStory), "Initialize")]
	[HarmonyPostfix]
	public static void Initialize_Postfix(NewGameWinPhaseStory __instance)
	{
		if (!Main.Enabled)
		{
			return;
		}

#if DEBUG
		Debug.Log($"{nameof(NewGameWinPhaseStory)}_Initialize_Postfix");
#endif

		__instance.m_Description.HookupTextToSpeech();
	}
}
