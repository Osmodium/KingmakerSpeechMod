using System;
using Kingmaker.UI.SettingsUI;
using SpeechMod.Localization;
using UnityEngine;

namespace SpeechMod.Configuration.Settings;

public abstract class ModSettingEntry(string key, string title, string tooltip)
{
    protected readonly string Key = key;
    protected readonly string Title = title;

    protected SettingStatus Status { get; private set; } = SettingStatus.NOT_APPLIED;

    public abstract SettingStatus TryEnable();

    public abstract void BuildUIAndLink();

    public abstract SettingsEntityBase GetUISettings();

    protected void InitializeSettingsEntity(SettingsEntityBase entity)
    {
        var prefix = ModConfigurationManager.Instance?.SettingsPrefix;
        entity.Description = ModLocalizationManager.CreateString($"{prefix}.feature.{Key}.description", Title);
        entity.TooltipDescription = ModLocalizationManager.CreateString($"{prefix}.feature.{Key}.tooltip-description", tooltip);
        entity.VisibleCheck = new SettingsEntityBase.VisibleCondition();
    }

    protected SettingStatus TryPatchInternal(params Type[] type)
    {
        Debug.Log("TryPatchInternal");

        if (Status != SettingStatus.NOT_APPLIED)
            return Status;

        try
        {
            foreach (var t in type)
            {
                ModConfigurationManager.Instance?.HarmonyInstance?.CreateClassProcessor(t)?.Patch();
            }
            Status = SettingStatus.WORKING;
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Log($"{Title} patch succeeded");
        }
        catch (Exception ex)
        {
            ModConfigurationManager.Instance?.ModEntry?.Logger?.Error($"{Title} patch exception: {ex.Message}");
            Status = SettingStatus.ERROR;
        }
        return Status;
    }
}
