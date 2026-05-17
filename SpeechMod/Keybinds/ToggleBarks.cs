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

public class ToggleBarks() : ModHotkeySettingEntry(KEY, TITLE, TOOLTIP, DEFAULT_VALUE)
{
    private const string KEY = "barks.toggle";
    private const string TITLE = "Toggle Barks";
    private const string TOOLTIP = "Toggles playback of Barks";
    private const string DEFAULT_VALUE = "%B;;All;false";
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
            Game.Instance!.Keyboard!.Bind(BIND_NAME, ToggleBarks);
        }

        private static void ToggleBarks()
        {
            Main.Settings.PlaybackBarks = !Main.Settings.PlaybackBarks;

            string text;
            if (Main.Settings.PlaybackBarks)
            {
                text = LocalizationManager.CurrentPack?.GetText($"{Constants.SETTINGS_PREFIX}.feature.barks.toggle.on.notification", false);
                if (string.IsNullOrWhiteSpace(text))
                    text = "SpeechMod: Barks turned ON!";
            }
            else
            {
                text = LocalizationManager.CurrentPack?.GetText($"{Constants.SETTINGS_PREFIX}.feature.barks.toggle.off.notification", false);
                if (string.IsNullOrWhiteSpace(text))
                    text = "SpeechMod: Barks turned OFF!";
            }

            if (Main.Settings!.ShowNotificationOnPlaybackStop)
            {
                UIUtility.SendWarning(text);
            }
        }
    }
}