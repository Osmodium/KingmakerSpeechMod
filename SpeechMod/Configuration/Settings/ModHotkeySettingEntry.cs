using Kingmaker;
using Kingmaker.UI.SettingsUI;
using SpeechMod.Localization;
using System;
using UnityEngine;

namespace SpeechMod.Configuration.Settings;

public abstract class ModHotkeySettingEntry(string key, string title, string tooltip, string defaultKeyPairString)
    : ModSettingEntry(key, title, tooltip)
{
    private SettingsEntityKeybind UiSettingEntity { get; set; }

    public static bool ReSavingRequired { get; private set; } = false;

    public override SettingsEntityBase GetUISettings() => UiSettingEntity;

    private string GetBindName() => $"{ModConfigurationManager.Instance?.SettingsPrefix}.newcontrols.ui.{Key}";

    public override void BuildUIAndLink()
    {
        Debug.Log("BuildUIAndLink");
        UiSettingEntity = MakeKeyBind();
        UiSettingEntity.OnValueChangedAction += delegate
        {
            TryEnable();
        };
    }

    private SettingsEntityKeybind MakeKeyBind()
    {
        var keyBindSetting = ScriptableObject.CreateInstance<SettingsEntityKeybind>();
        keyBindSetting.Description = ModLocalizationManager.CreateString($"{ModConfigurationManager.Instance?.SettingsPrefix}.feature.{Key}.description", Title);
        keyBindSetting.TooltipDescription = ModLocalizationManager.CreateString($"{ModConfigurationManager.Instance?.SettingsPrefix}.feature.{Key}.tooltip-description", Tooltip);
        keyBindSetting.name = $"{ModConfigurationManager.Instance?.SettingsPrefix}.newcontrols.ui.{Key}";
        keyBindSetting.m_CurrentBindings = ParseKeyPairString(defaultKeyPairString);
        keyBindSetting.DefaultBinding1 = keyBindSetting.m_CurrentBindings.Binding1;
        keyBindSetting.DefaultBinding2 = keyBindSetting.m_CurrentBindings.Binding2;
        keyBindSetting.GameModesGroup = keyBindSetting.m_CurrentBindings.GameModesGroup;
        keyBindSetting.TriggerOnHold = keyBindSetting.m_CurrentBindings.TriggerOnHold;
        keyBindSetting.VisibleCheck = new SettingsEntityBase.VisibleCondition();
        return keyBindSetting;
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
        while (i < bindingString.Length - 1)
        {
            switch (bindingString[i])
            {
                case '%':
                    data.IsCtrlDown = true;
                    i++;
                    break;
                case '#':
                    data.IsShiftDown = true;
                    i++;
                    break;
                case '&':
                    data.IsAltDown = true;
                    i++;
                    break;
                default:
                    goto done;
            }
        }
        done:
        var keyName = bindingString.Substring(i);
        if (Enum.TryParse<KeyCode>(keyName, out var keyCode))
            data.Key = keyCode;

        return data;
    }

    protected void RegisterKeybind()
    {
        Debug.Log("RegisterKeybind");

        if (Status != SettingStatus.NOT_APPLIED)
            return;

        var currentValue = UiSettingEntity.GetDefaults();
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
        Debug.Log("TryEnableAndPatch");
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
        Debug.Log("TryFix");
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
