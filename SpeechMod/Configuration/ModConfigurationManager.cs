using HarmonyLib;
using Kingmaker;
using Kingmaker.UI;
using Kingmaker.UI.SettingsUI;
using SpeechMod.Configuration.Settings;
using SpeechMod.Configuration.UI;
using SpeechMod.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using Kingmaker.Utility;
using static UnityModManagerNet.UnityModManager;

namespace SpeechMod.Configuration;

public class ModConfigurationManager
{
    public Dictionary<string, List<ModSettingEntry>> GroupedSettings = new();
    public Harmony HarmonyInstance { get; protected set; }
    public ModEntry ModEntry { get; protected set; }
    public string SettingsPrefix = Guid.NewGuid().ToString();

    private ModConfigurationManager() { }

    public static void Build(Harmony harmonyInstance, ModEntry modEntry, string settingsPrefix)
    {
        Instance.HarmonyInstance = harmonyInstance;
        Instance.ModEntry = modEntry;
        Instance.SettingsPrefix = settingsPrefix;
        ModLocalizationManager.Init();
    }

    private bool Initialized = false;

    public void Initialize()
    {
        if (Initialized) return;
        Initialized = true;

        foreach (var setting in GroupedSettings.SelectMany(settings => settings.Value))
        {
            setting.BuildUIAndLink();
            setting.TryEnable();
        }

        if (ModHotkeySettingEntry.ReSavingRequired)
        {
            SettingsProvider.Default.SaveAll();
            Instance.ModEntry.Logger.Log("Hotkey settings were migrated");
        }
    }

    public static ModConfigurationManager Instance { get; } = new();
}

[HarmonyPatch]
public static class SettingsUIPatches
{
    [HarmonyPatch(typeof(SettingsManager), nameof(SettingsManager.Initialize))]
    [HarmonyPostfix]
    static void AddSettingsGroup()
    {
        if (Enumerable.Any(Game.Instance.SettingsManager.m_SoundSettingsList, group => group.name?.StartsWith(ModConfigurationManager.Instance.SettingsPrefix) ?? false))
        {
            return;
        }

        ModConfigurationManager.Instance?.Initialize();

        foreach (var settings in ModConfigurationManager.Instance.GroupedSettings)
        {
            Game.Instance.SettingsManager.m_SoundSettingsList?.Add(
                OwlcatUITools.MakeSettingsGroup($"{ModConfigurationManager.Instance.SettingsPrefix}.group.{settings.Key}", "Speech Mod",
                    settings.Value?.Select(x => x.GetUISettings()).ToArray()
                ));
        }
    }

    [HarmonyPatch(typeof(KeyboardAccess), nameof(KeyboardAccess.CanBeRegistered))]
    [HarmonyPrefix]
    public static bool CanRegisterAnything(ref bool __result, string name)
    {
        if (name == null || !name.StartsWith(ModConfigurationManager.Instance.SettingsPrefix))
        {
            return true;
        }
        __result = true;
        return false;
    }
}