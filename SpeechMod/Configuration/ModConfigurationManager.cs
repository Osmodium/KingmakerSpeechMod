using HarmonyLib;
using Kingmaker;
using Kingmaker.UI;
using Kingmaker.UI.SettingsUI;
using Kingmaker.Utility;
using SpeechMod.Configuration.Settings;
using SpeechMod.Configuration.UI;
using SpeechMod.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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

    private bool _initialized;

    public void Initialize()
    {
        if (_initialized)
            return;

        _initialized = true;
        var list = GroupedSettings.SelectMany(settings => settings.Value).ToArray();

        Debug.Log($"Build and Enable all of {list.Length} grouped settings");
        foreach (var setting in list)
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
            Debug.Log($"No group starts with {ModConfigurationManager.Instance.SettingsPrefix}!");
            return;
        }

        Debug.Log("Initialize ModConfigurationManager!");
        ModConfigurationManager.Instance?.Initialize();
        foreach (var settings in ModConfigurationManager.Instance!.GroupedSettings)
        {
            var key = $"{ModConfigurationManager.Instance.SettingsPrefix}.group.{settings.Key}";
            var settingsGroup = OwlcatUITools.MakeSettingsGroup(key, "Speech Mod", settings.Value?.Select(x => x.GetUISettings()).ToArray());
            Debug.Log($"Adding settings group {settingsGroup.Title} with {settingsGroup.SettingsList.Length} items, to key {key}...");
            Game.Instance.SettingsManager.m_SoundSettingsList?.Add(
                settingsGroup
            );
        }
    }

    [HarmonyPatch(typeof(KeyboardAccess), nameof(KeyboardAccess.CanBeRegistered))]
    [HarmonyPrefix]
    public static bool CanRegisterAnything(ref bool __result, string name)
    {
        if (name == null || !name.StartsWith(ModConfigurationManager.Instance.SettingsPrefix))
        {
            Debug.LogWarning($"{name} can't be registered since it's either null or doesn't start with {ModConfigurationManager.Instance.SettingsPrefix}!");
            return true;
        }
        __result = true;
        return false;
    }
}