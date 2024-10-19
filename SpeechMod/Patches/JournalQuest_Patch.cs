using HarmonyLib;
using Kingmaker.AreaLogic.QuestSystem;
using Kingmaker.UI.Journal;
using SpeechMod.Unity;
using SpeechMod.Unity.Extensions;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class JournalQuest_Patch
{
    private const string BUTTON_NAME = "SpeechMod_JQButton";

	[HarmonyPostfix]
	[HarmonyPatch(typeof(JournalQuestElement), nameof(JournalQuestElement.Initialize), typeof(Quest))]
	public static void AddDescription(JournalQuestElement __instance, Quest quest)
	{
		if (!Main.Enabled)
			return;

#if DEBUG
		Debug.Log($"{nameof(JournalQuestElement)}_{nameof(AddDescription)}");
#endif

		__instance.Header.HookupTextToSpeech();
		__instance.Description.HookupTextToSpeech();
		__instance.CompletionText.HookupTextToSpeech();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(JournalQuestObjective), nameof(JournalQuestObjective.Initialize), typeof(QuestObjective))]
    public static void AddButtonsToParts(JournalQuestObjective __instance, QuestObjective objective)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(JournalQuestObjective)}_{nameof(AddButtonsToParts)}");
#endif

	    __instance.Header.TryAddButtonToTextMeshPro(BUTTON_NAME, new Vector2(0f, -25f), new Vector3(0.6f, 0.6f, 1.0f));
		__instance.Description.HookupTextToSpeech();
		__instance.m_TimeToFail.HookupTextToSpeech();
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(JournalQuestAddendum), nameof(JournalQuestAddendum.Initialize), typeof(QuestObjective))]
    public static void AddAddendums(JournalQuestAddendum __instance, QuestObjective addendum)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(JournalQuestAddendum)}_{nameof(AddAddendums)}");
#endif

        __instance.m_Header.HookupTextToSpeech();
    }
}