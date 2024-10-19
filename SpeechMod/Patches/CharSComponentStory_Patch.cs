using HarmonyLib;
using Kingmaker.UI.ServiceWindow.CharacterScreen;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class CharSComponentStory_Patch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(CharSComponentStory), nameof(CharSComponentStory.Init), typeof(CharSStories), typeof(int))]
	public static void Init_Postfix(CharSComponentStory __instance, CharSStories charSStories, int index)
	{
		if (!Main.Enabled)
		{
			return;
		}

#if DEBUG
		Debug.Log($"{nameof(CharSComponentStory)}_{nameof(Init_Postfix)}");
#endif


		__instance.TitleStory.HookupTextToSpeech();
		__instance.StoryText.HookupTextToSpeech();
	}
}