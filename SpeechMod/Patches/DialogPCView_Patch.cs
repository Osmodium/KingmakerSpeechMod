using HarmonyLib;
using Kingmaker;
using SpeechMod.Unity;
using SpeechMod.Unity.Extensions;
using UnityEngine;
using DialogController = Kingmaker.UI.Dialog.DialogController;

namespace SpeechMod.Patches;

[HarmonyPatch]
public static class DialogPCView_Patch
{
    private const string SPEECHMOD_DIALOGBUTTON_NAME = "SpeechMod_DialogButton";
    private const string SCROLL_VIEW_PATH = "/StaticCanvas/Dialogue/Body/View/Scroll View";

    [HarmonyPatch(typeof(DialogController), "Initialize")]
    [HarmonyPostfix]
    public static void AddDialogButton()
    {
        if (!Main.Enabled)
            return;

#if DEBUG
        var sceneName = Game.Instance!.CurrentlyLoadedArea.CustomUIScene.SceneName;
        Debug.Log($"{nameof(DialogController)}_Initialize_Postfix @ {sceneName}");
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
}