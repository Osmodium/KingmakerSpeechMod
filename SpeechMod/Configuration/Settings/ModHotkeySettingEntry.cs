using Kingmaker;
using Kingmaker.UI.SettingsUI;
using SpeechMod.Localization;
using System;
using UnityEngine;

namespace SpeechMod.Configuration.Settings;

public abstract class ModHotkeySettingEntry : ModSettingEntry
{
    //public readonly SettingsEntityKeyBindingPair SettingEntity;
    public SettingsEntityKeybind UiSettingEntity { get; private set; }

    public static bool ReSavingRequired { get; private set; } = false;

    protected ModHotkeySettingEntry(string key, string title, string tooltip, string DefaultKeyPairString) : base(key, title, tooltip)
    {
        //try
        //{
        //    SettingEntity = new SettingsEntityKeyBindingPair(SettingsController.Instance, $"{ModConfigurationManager.Instance?.SettingsPrefix}.newcontrols.{Key}", new(DefaultKeyPairString), false, true);
        //}
        //catch (Exception ex)
        //{
        //    ModConfigurationManager.Instance?.ModEntry?.Logger?.Error($"Failed to create {Key} setting entity: {ex}");
        //}
    }

    public override SettingsEntityBase GetUISettings() => UiSettingEntity;

    public string GetBindName() => $"{ModConfigurationManager.Instance?.SettingsPrefix}.newcontrols.ui.{Key}";
    public override void BuildUIAndLink()
    {
        UiSettingEntity = MakeKeyBind();
        //UiSettingEntity.LinkSetting(SettingEntity);
        //(SettingEntity as IReadOnlySettingEntity<KeyBindingPair>).OnValueChanged += delegate
        //{
        //    TryEnable();
        //};
    }

    private SettingsEntityKeybind MakeKeyBind()
    {
        var keyBindSetting = ScriptableObject.CreateInstance<SettingsEntityKeybind>();
        keyBindSetting.Description = ModLocalizationManager.CreateString($"{ModConfigurationManager.Instance?.SettingsPrefix}.feature.{Key}.description", Title);
        keyBindSetting.TooltipDescription = ModLocalizationManager.CreateString($"{ModConfigurationManager.Instance?.SettingsPrefix}.feature.{Key}.tooltip-description", Tooltip);
        keyBindSetting.name = $"{ModConfigurationManager.Instance?.SettingsPrefix}.newcontrols.ui.{Key}";
        return keyBindSetting;
    }

    protected void RegisterKeybind()
    {
        if (Status != SettingStatus.NOT_APPLIED) return;

        var currentValue = UiSettingEntity.m_CurrentBindings;

        if (currentValue.Binding1.Key != KeyCode.None)
        {
            Game.Instance.Keyboard.RegisterBinding(
                GetBindName(),
                currentValue.Binding1,
                currentValue.GameModesGroup,
                currentValue.TriggerOnHold);
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} binding 1 registered: {currentValue.Binding1}");
        }
        else
        {
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} binding 1 empty");
        }

        if (currentValue.Binding2.Key != KeyCode.None)
        {
            Game.Instance.Keyboard.RegisterBinding(
                GetBindName(),
                currentValue.Binding2,
                currentValue.GameModesGroup,
                currentValue.TriggerOnHold);
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} binding 2 registered: {currentValue.Binding2}");
        }
        else
        {
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} binding 2 empty");
        }
    }

    protected SettingStatus TryEnableAndPatch(Type type)
    {
        TryFix();
        if (Status != SettingStatus.NOT_APPLIED)
        {
            return Status;
        }

        RegisterKeybind();
        var currentValue = UiSettingEntity.m_CurrentBindings;
        if (currentValue.Binding1.Key != KeyCode.None || currentValue.Binding2.Key != KeyCode.None)
        {
            return TryPatchInternal(type);
        }
        else
        {
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} binding 1 and binding 2 empty, setting integration skipped");
        }
        return SettingStatus.NOT_APPLIED;
    }

    /// <summary>
    /// If hotkey group or trigger changes, those values need to be updated manually
    /// and later saved
    /// </summary>
    private void TryFix()
    {
        var curValue = UiSettingEntity.m_CurrentBindings;
        var defaultGroup = UiSettingEntity.GetDefaults().GameModesGroup;
        var defaultTrigger = UiSettingEntity.GetDefaults().TriggerOnHold;
        if (curValue.GameModesGroup != defaultGroup || curValue.TriggerOnHold != defaultTrigger)
        {
            curValue.GameModesGroup = defaultGroup;
            curValue.TriggerOnHold = defaultTrigger;
            UiSettingEntity.PresetEntity.SetDefaultValueFromCurrent();
            ReSavingRequired = true;
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} had outdated hotkey settings, migrated.");
        }
    }
}
