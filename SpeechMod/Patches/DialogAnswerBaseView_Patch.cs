using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.DialogSystem.Blueprints;
using Kingmaker.UI.Dialog;
using SpeechMod.Unity;
using SpeechMod.Unity.Extensions;
using SpeechMod.Voice;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace SpeechMod.Patches;

[HarmonyPatch]
public class DialogAnswerBaseView_Patch
{
    private const string DIALOG_ANSWER_BUTTON_NAME = "SpeechMod_DialogAnswerButton";

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TempOptionUI), nameof(TempOptionUI.Initialize), typeof(int), typeof(BlueprintAnswer))]
    public static void AddHooks_Postfix(TempOptionUI __instance, int index, BlueprintAnswer answer)
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(TempOptionUI)}_{nameof(AddHooks_Postfix)}");
#endif

        TryAddDialogButton(__instance.OptionName, __instance, new Vector2(-50f, -23f));
    }

    private static void TryAddDialogButton(TextMeshProUGUI textMeshPro, TempOptionUI instance, Vector2? anchoredPosition = null)
    {
        var transform = textMeshPro?.transform;

#if DEBUG
        Debug.Log($"Adding/Removing dialog answer button on {textMeshPro?.name}...");
#endif

        var playButtonGameObject = transform?.Find(DIALOG_ANSWER_BUTTON_NAME)?.gameObject;

        // 1. We don't want the button to be there.
        if (!Main.Settings.ShowPlaybackOfDialogAnswers)
        {
            // 1a. Destroy the button if it exists
            if (playButtonGameObject != null)
                Object.Destroy(playButtonGameObject.gameObject);
            return;
        }

        // 2. We want the button and it exists.
        if (playButtonGameObject != null)
            return;

        // 3. We want the button but it doesn't exist.
        playButtonGameObject = ButtonFactory.TryCreatePlayButton(transform, () =>
        {
            if (textMeshPro == null)
                return;
            var text = textMeshPro.text;

            if (Main.Settings?.LogVoicedLines == true)
                Debug.Log(text);

            if (Main.Settings?.SayDialogAnswerNumber == false)
                text = new Regex(@"^\d+\. ?").Replace(text, "");

            text = text.PrepareText();

            Main.Speech?.SpeakAs(text, Game.Instance?.Player?.MainCharacter.Value.Gender == Gender.Female ? VoiceType.Female : VoiceType.Male);
        });

        if (playButtonGameObject == null || playButtonGameObject.transform == null)
            return;

        playButtonGameObject.name = DIALOG_ANSWER_BUTTON_NAME;
        playButtonGameObject.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
        playButtonGameObject.RectAlignTopLeft(anchoredPosition);
        playButtonGameObject.SetActive(true);
    }
}