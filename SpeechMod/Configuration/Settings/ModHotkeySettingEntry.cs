using Kingmaker;
using Kingmaker.UI.SettingsUI;
using System;
using UnityEngine;

namespace SpeechMod.Configuration.Settings;

public abstract class ModHotkeySettingEntry(string key, string title, string tooltip, string defaultKeyPairString) : ModSettingEntry(key, title, tooltip)
{
    private SettingsEntityKeybind UiSettingEntity { get; set; }

    public static bool ReSavingRequired { get; private set; }

    public override SettingsEntityBase GetUISettings() => UiSettingEntity;

    private string GetBindName() => $"{ModConfigurationManager.Instance?.SettingsPrefix}.newcontrols.ui.{Key}";

    public override void BuildUIAndLink()
    {
        UiSettingEntity = CreateKeybindEntity();
        UiSettingEntity.OnValueChangedAction += delegate { TryEnable(); };
    }

    private SettingsEntityKeybind CreateKeybindEntity()
    {
        var entity = ScriptableObject.CreateInstance<SettingsEntityKeybind>();
        InitializeSettingsEntity(entity);
        entity.name = $"{ModConfigurationManager.Instance?.SettingsPrefix}.newcontrols.ui.{Key}";

        var bindings = ParseKeyPairString(defaultKeyPairString);
        entity.m_CurrentBindings = bindings;
        entity.DefaultBinding1 = bindings.Binding1;
        entity.DefaultBinding2 = bindings.Binding2;
        entity.GameModesGroup = bindings.GameModesGroup;
        entity.TriggerOnHold = bindings.TriggerOnHold;
        
        return entity;
    }

    /// <summary>
    /// Parses a key pair string in the format "Binding1;Binding2;GameModesGroup;TriggerOnHold"
    /// where binding strings use % for Ctrl, # for Shift, &amp; for Alt prefix before a key name.
    /// Example: "%S;;All;false" = Ctrl+S, no second binding, All game modes, no trigger on hold.
    /// </summary>
    private static TwoBindigs ParseKeyPairString(string keyPairString)
    {
        var bindings = new TwoBindigs();
        if (string.IsNullOrEmpty(keyPairString))
            return bindings;

        var parts = keyPairString.Split(';');
        bindings.Binding1 = parts.Length > 0 ? ParseBinding(parts[0]) : new BindingKeysData();
        bindings.Binding2 = parts.Length > 1 ? ParseBinding(parts[1]) : new BindingKeysData();

        if (parts.Length > 2 && Enum.TryParse<Kingmaker.UI.KeyboardAccess.GameModesGroup>(parts[2], out var group))
            bindings.GameModesGroup = group;

        if (parts.Length > 3 && bool.TryParse(parts[3], out var triggerOnHold))
            bindings.TriggerOnHold = triggerOnHold;

        return bindings;
    }

    private static BindingKeysData ParseBinding(string bindingString)
    {
        var data = new BindingKeysData();
        if (string.IsNullOrEmpty(bindingString))
            return data;

        var i = 0;
        while (i < bindingString.Length)
        {
            var c = bindingString[i];
            if (c == '%') data.IsCtrlDown = true;
            else if (c == '#') data.IsShiftDown = true;
            else if (c == '&') data.IsAltDown = true;
            else break;
            i++;
        }

        if (i < bindingString.Length && Enum.TryParse<KeyCode>(bindingString.Substring(i), out var keyCode))
            data.Key = keyCode;

        return data;
    }

    private void TryRegisterBinding(string bindName, BindingKeysData binding, Kingmaker.UI.KeyboardAccess.GameModesGroup group, bool triggerOnHold, string label)
    {
        if (binding.Key == KeyCode.None)
        {
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} {label} empty");
            return;
        }

        Game.Instance.Keyboard.RegisterBinding(bindName, binding, group, triggerOnHold);
        ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} {label} registered: {binding}");
    }

    private void RegisterKeybind()
    {
        if (Status != SettingStatus.NOT_APPLIED)
            return;

        var defaults = UiSettingEntity.GetDefaults();
        var bindName = GetBindName();
        TryRegisterBinding(bindName, defaults.Binding1, defaults.GameModesGroup, defaults.TriggerOnHold, "binding 1");
        TryRegisterBinding(bindName, defaults.Binding2, defaults.GameModesGroup, defaults.TriggerOnHold, "binding 2");
    }

    protected SettingStatus TryEnableAndPatch(Type type)
    {
        TryMigrateBindings();

        if (Status != SettingStatus.NOT_APPLIED)
            return Status;

        RegisterKeybind();

        var currentValue = UiSettingEntity.m_CurrentBindings;
        if (currentValue.Binding1.Key != KeyCode.None || currentValue.Binding2.Key != KeyCode.None)
            return TryPatchInternal(type);

        ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} both bindings empty, setting integration skipped");
        return SettingStatus.NOT_APPLIED;
    }

    /// <summary>
    /// If hotkey group or trigger changes between mod versions, migrate saved values to new defaults.
    /// </summary>
    private void TryMigrateBindings()
    {
        var curValue = UiSettingEntity.m_CurrentBindings;
        var defaults = UiSettingEntity.GetDefaults();

        if (defaults == null || (curValue.GameModesGroup == defaults.GameModesGroup && curValue.TriggerOnHold == defaults.TriggerOnHold))
            return;

        curValue.GameModesGroup = defaults.GameModesGroup;
        curValue.TriggerOnHold = defaults.TriggerOnHold;
        ReSavingRequired = true;
        ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} had outdated hotkey settings, migrated.");
    }
}
