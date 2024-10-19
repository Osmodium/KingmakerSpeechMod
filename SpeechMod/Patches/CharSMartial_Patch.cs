using HarmonyLib;
using Kingmaker.UI.ServiceWindow.CharacterScreen;
using Kingmaker.UnitLogic;
using SpeechMod.Unity.Extensions;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class CharSMartial_Patch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(CharSMartial), nameof(CharSMartial.FillProficiencies), typeof(UnitDescriptor))]
	public static void FillProficiencies_Postfix(CharSMartial __instance, UnitDescriptor unit)
	{
		if (!Main.Enabled)
		{
			return;
		}

#if DEBUG
		Debug.Log($"{nameof(CharSMartial)}_{nameof(FillProficiencies_Postfix)}");
#endif

		__instance.WeaponProficiecyContainer.HookupTextToSpeechOnTransform();
	}
}