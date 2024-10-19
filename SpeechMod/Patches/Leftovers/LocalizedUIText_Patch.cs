//using HarmonyLib;
//using Kingmaker.Localization;
//using Kingmaker.UI.Tooltip;
//using SpeechMod.Unity.Extensions;
//using TMPro;
//using UnityEngine;

//namespace SpeechMod.Patches;

//[HarmonyPatch]
//public static class LocalizedUIText_Patch
//{
//	[HarmonyPostfix]
//	[HarmonyPatch(typeof(LocalizedUIText), "Awake")]
//	public static void Awake_Postfix(LocalizedUIText __instance)
//	{
//		if (!Main.Enabled)
//		{
//			return;
//		}

//#if DEBUG
//		Debug.Log($"{nameof(DescriptionBrick)}_Init_Patch");
//#endif

//		var component = __instance.GetComponent<TextMeshProUGUI>();
//		if (component == null)
//		{
//			Debug.LogError($"No {nameof(TextMeshProUGUI)} component found on {__instance.name}.");
//			return;
//		}
//		component.HookupTextToSpeech();
//	}
//}