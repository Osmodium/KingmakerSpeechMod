using HarmonyLib;
using Kingmaker;
using Kingmaker.Controllers.Dialog;
using Kingmaker.Localization;
using Debug = UnityEngine.Debug;
using DialogController = Kingmaker.UI.Dialog.DialogController;
#if DEBUG
#endif

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class UiDialogController_Patch
{
	[HarmonyPatch(typeof(DialogController), "HandleOnCueShow", typeof(CueShowData))]
	[HarmonyPostfix]
	public static void HandleOnCueShow_Postfix()
	{
		if (!Main.Enabled)
			return;

#if DEBUG
		Debug.Log($"{nameof(DialogController)}_HandleOnCueShow_Postfix");
#endif

		if (!Main.Settings!.AutoPlay)
		{
#if DEBUG
			Debug.Log($"{nameof(DialogController)}: AutoPlay is disabled!");
#endif
			return;
		}

		string key = Game.Instance?.DialogController?.CurrentCue?.Text?.Key;
		if (string.IsNullOrWhiteSpace(key))
			key = Game.Instance?.DialogController?.CurrentCue?.Text?.Shared?.String?.Key;

		if (string.IsNullOrWhiteSpace(key))
			return;

		// Stop playing and don't play if the dialog is voice acted.
		if (!Main.Settings.AutoPlayIgnoreVoice &&
		    !string.IsNullOrWhiteSpace(LocalizationManager.SoundPack?.GetText(key, false)))
		{
			Main.Speech?.Stop();
			return;
		}

		Main.Speech?.SpeakDialog(Game.Instance?.DialogController?.CurrentCue?.DisplayText, 0.5f);
	}
}