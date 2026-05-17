using Kingmaker.UI.SettingsUI;
using SpeechMod.Localization;
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
        _uiSettingEntity.Description = ModLocalizationManager.CreateString($"{ModConfigurationManager.Instance?.SettingsPrefix}.feature.{Key}.description", Title);
        _uiSettingEntity.TooltipDescription = ModLocalizationManager.CreateString($"{ModConfigurationManager.Instance?.SettingsPrefix}.feature.{Key}.tooltip-description", Tooltip);
        _uiSettingEntity.DefaultValue = defaultValue;
        _uiSettingEntity.VisibleCheck = new SettingsEntityBase.VisibleCondition();
        _uiSettingEntity.OnValueChangedAction += delegate
        {
            TryEnable();
        };
    }

    protected SettingStatus TryEnableAndPatch(Type type)
    {
        var currentValue = _uiSettingEntity.CurrentValue;
        if (currentValue)
        {
            return TryPatchInternal(type);
        }
        else
        {
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} disabled, setting integration skipped");
        }
        return SettingStatus.NOT_APPLIED;
    }
}
