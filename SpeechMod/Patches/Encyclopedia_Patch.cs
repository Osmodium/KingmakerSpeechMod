using HarmonyLib;
using Kingmaker.Blueprints.Encyclopedia;
using Kingmaker.Blueprints.Encyclopedia.Blocks;
using Kingmaker.UI.ServiceWindow.Encyclopedia;
using Kingmaker.UI.ServiceWindow.Encyclopedia.Blocks;
using SpeechMod.Unity.Extensions;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class Encyclopedia_Patch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(EncyclopediaBlockPage), nameof(EncyclopediaBlockPage.Initialize), typeof(IContentView), typeof(IPage), typeof(IBlock))]
	public static void AddTitleHook_Postfix(EncyclopediaBlockPage __instance, IContentView view, IPage blueprintPage, IBlock blueprintBlock)
	{
		if (!Main.Enabled)
		{
			return;
		}

#if DEBUG
		Debug.Log($"{nameof(EncyclopediaBlockPage)}_{nameof(AddTitleHook_Postfix)}");
#endif

		__instance.m_TitleLabel.HookupTextToSpeech();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(EncyclopediaBlockText), nameof(EncyclopediaBlockText.Initialize), typeof(IContentView), typeof(IPage), typeof(IBlock))]
	public static void AddBlockHooks_Postfix(EncyclopediaBlockText __instance, IContentView view, IPage blueprintPage, IBlock blueprintBlock)
	{
		if (!Main.Enabled)
		{
			return;
		}

#if DEBUG
		Debug.Log($"{nameof(EncyclopediaBlockText)}_{nameof(AddBlockHooks_Postfix)}");
#endif

		__instance.m_Label.HookupTextToSpeech();
	}
}
