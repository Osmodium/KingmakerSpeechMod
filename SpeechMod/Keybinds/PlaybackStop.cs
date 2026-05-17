using HarmonyLib;
using Kingmaker;
using Kingmaker.Localization;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using SpeechMod.Configuration.Settings;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Keybinds;

public class PlaybackStop() : ModHotkeySettingEntry(KEY, TITLE, TOOLTIP, DEFAULT_VALUE)
{
    private const string KEY = "playback.stop";
    private const string TITLE = "Stop playback";
    private const string TOOLTIP = "Stops playback of SpeechMod TTS.";
    private const string DEFAULT_VALUE = "%S;;All;false";
    private const string BIND_NAME = $"{Constants.SETTINGS_PREFIX}.newcontrols.ui.{KEY}";

    public override SettingStatus TryEnable() => TryEnableAndPatch(typeof(Patches));

    [HarmonyPatch]
    private static class Patches
    {
        [HarmonyPatch(typeof(UICommon), nameof(UICommon.Initialize))]
        [HarmonyPostfix]
        private static void Add(UICommon __instance)
        {
#if DEBUG
            Debug.Log($"{nameof(UICommon)}_{nameof(UICommon.Initialize)}_Postfix : {BIND_NAME}");
#endif
            Game.Instance!.Keyboard!.Bind(BIND_NAME, StopPlayback);
        }

        private static void StopPlayback()
        {
            if (!Main.Speech?.IsSpeaking() == true)
                return;

            var text = LocalizationManager.CurrentPack?.GetText($"{Constants.SETTINGS_PREFIX}.feature.playback.stop.notification", false);
            if (string.IsNullOrWhiteSpace(text))
                text = "SpeechMod: Playback stopped!";

            if (Main.Settings!.ShowNotificationOnPlaybackStop)
                UIUtility.SendWarning(text);

            Main.Speech?.Stop();
        }
    }
}