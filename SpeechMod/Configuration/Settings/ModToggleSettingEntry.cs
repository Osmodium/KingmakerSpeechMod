using Kingmaker.UI.SettingsUI;
using System;
using UnityEngine;

namespace SpeechMod.Configuration.Settings;

public abstract class ModToggleSettingEntry(string key, string title, string tooltip, bool defaultValue) : ModSettingEntry(key, title, tooltip)
{
    private SettingsEntityBool _uiSettingEntity;

    public override SettingsEntityBase GetUISettings() => _uiSettingEntity;

    public override void BuildUIAndLink()
    {
        _uiSettingEntity = ScriptableObject.CreateInstance<SettingsEntityBool>();
        InitializeSettingsEntity(_uiSettingEntity);
        _uiSettingEntity.DefaultValue = defaultValue;
        _uiSettingEntity.OnValueChangedAction += delegate { TryEnable(); };
    }

    protected SettingStatus TryEnableAndPatch(Type type)
    {
        if (!_uiSettingEntity.CurrentValue)
        {
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} disabled, setting integration skipped");
            return SettingStatus.NOT_APPLIED;
        }

        return TryPatchInternal(type);
    }
}
