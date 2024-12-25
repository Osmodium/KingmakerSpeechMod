using System.Security.Cryptography.X509Certificates;
using HarmonyLib;
using Kingmaker;
using Kingmaker.Localization;
using Kingmaker.UI;
using Kingmaker.UI.Common;
using SpeechMod.Configuration.Settings;
using UniRx.Triggers;
#if DEBUG
using UnityEngine;
#endif

namespace SpeechMod.Keybinds;

public class PlaybackStop : ModHotkeySettingEntry
{
    private const string _key = "playback.stop";
    private const string _title = "Stop playback";
    private const string _tooltip = "Stops playback of SpeechMod TTS.";
    private const string _defaultValue = "%S;;All;false";
    private const string BIND_NAME = $"{Constants.SETTINGS_PREFIX}.newcontrols.ui.{_key}";

    public PlaybackStop() : base(_key, _title, _tooltip, _defaultValue)
    { }

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

            var text = LocalizationManager.CurrentPack!.GetText("osmodium.speechmod.feature.playback.stop.notification", false);
            if (string.IsNullOrWhiteSpace(text))
                text = "SpeechMod: Playback stopped!";

            if (Main.Settings!.ShowNotificationOnPlaybackStop)
                UIUtility.SendWarning(text);

            Main.Speech?.Stop();
        }
    }
}