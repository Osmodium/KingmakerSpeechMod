using HarmonyLib;
using Kingmaker;
using Kingmaker.Controllers.Dialog;
using Kingmaker.Localization;
using SpeechMod.Unity;
using SpeechMod.Unity.Extensions;
using UnityEngine;
using DialogController = Kingmaker.UI.Dialog.DialogController;

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class UiDialogController_Patch
{
    private const string SPEECHMOD_DIALOGBUTTON_NAME = "SpeechMod_DialogButton";
    private const string SCROLL_VIEW_PATH = "/StaticCanvas/Dialogue/Body/View/Scroll View";

    [HarmonyPostfix]
    [HarmonyPatch(typeof(DialogController), nameof(DialogController.Initialize))]
    public static void Initialize_Postfix()
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        var sceneName = Game.Instance!.CurrentlyLoadedArea.CustomUIScene.SceneName;
        Debug.Log($"{nameof(DialogController)}_{nameof(Initialize_Postfix)} @ {sceneName}");
#endif

        AddDialogButtonByPath(SCROLL_VIEW_PATH);
    }

    private static void AddDialogButtonByPath(string path)
    {

#if DEBUG
        Debug.Log($"Adding speech button to dialog ui on '{path}'");
#endif

        var parent = UIHelper.TryFind(path);

        if (parent == null)
        {
            Debug.LogWarning("Parent not found!");
            return;
        }


        if (parent.TryFind(SPEECHMOD_DIALOGBUTTON_NAME) != null)
        {
            Debug.LogWarning("Button already exists!");
            return;
        }

        var buttonGameObject = ButtonFactory.TryCreatePlayButton(parent, () =>
        {
            Main.Speech?.SpeakDialog(Game.Instance?.DialogController?.CurrentCue?.DisplayText);
        });

        if (buttonGameObject == null)
        {
            return;
        }

        buttonGameObject.name = SPEECHMOD_DIALOGBUTTON_NAME;
        buttonGameObject.RectAlignTopLeft(new Vector2(-12f, -20f));

        buttonGameObject.SetActive(true);
    }

    [HarmonyPatch(typeof(DialogController), nameof(DialogController.HandleOnCueShow), typeof(CueShowData))]
    [HarmonyPostfix]
    public static void HandleOnCueShow_Postfix()
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        Debug.Log($"{nameof(DialogController)}_{nameof(HandleOnCueShow_Postfix)}");
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