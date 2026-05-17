using Kingmaker.UI.SettingsUI;
using SpeechMod.Localization;
using UnityEngine;

namespace SpeechMod.Configuration.UI;

public static class OwlcatUITools
{
    public static SettingsGroup MakeSettingsGroup(string key, string name, params SettingsEntityBase[] settings)
    {
        var group = ScriptableObject.CreateInstance<SettingsGroup>();
        group.name = key;
        group.Title = ModLocalizationManager.CreateString(key, name);

        group.SettingsList = settings;

        return group;
    }
}
