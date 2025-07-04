﻿using Playnite.SDK;
using Playnite.SDK.Data;
using PlayniteExtensions.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ExtraEmulatorProfiles;

public class ExtraEmulatorProfilesSettings : ObservableObject
{
    private Version installedPatchVersion = new Version(0, 0);

    [DontSerialize]
    public Version InstalledPatchVersion { get => installedPatchVersion; set => SetValue(ref installedPatchVersion, value); }

    public string InstalledPatchVersionString
    {
        get => InstalledPatchVersion.ToString();
        set => InstalledPatchVersion = new Version(value);
    }
}

public class ExtraEmulatorProfilesSettingsViewModel : PluginSettingsViewModel<ExtraEmulatorProfilesSettings, ExtraEmulatorProfiles>
{
    public Version PluginVersion { get; private set; }
    public string PlayniteEmulationDirectory { get; private set; }
    public string PatchDirectory { get; private set; }
    public string OriginalsDirectory { get; private set; }

    public ExtraEmulatorProfilesSettingsViewModel(ExtraEmulatorProfiles plugin) : base(plugin, plugin.PlayniteApi)
    {
        Settings = plugin.LoadPluginSettings<ExtraEmulatorProfilesSettings>() ?? new ExtraEmulatorProfilesSettings();

        var assembly = Assembly.GetExecutingAssembly();

        PluginVersion = assembly.GetName().Version;
        PlayniteEmulationDirectory = $@"{PlayniteApi.Paths.ApplicationPath}\Emulation\";
        var assemblyDir = new FileInfo(assembly.Location).DirectoryName;
        PatchDirectory = $@"{assemblyDir}\EmulationFiles\Patch\";
        OriginalsDirectory = $@"{assemblyDir}\EmulationFiles\Original\";
    }

    public void ExecutePatch() => CopyFiles(PatchDirectory, PluginVersion);

    public void Reset()
    {
        DeleteFiles(PlayniteEmulationDirectory, "*.yaml");
        DeleteFiles(PlayniteEmulationDirectory + "Emulators", "*", SearchOption.AllDirectories);

        CopyFiles(OriginalsDirectory, new Version(0, 0));
    }

    public RelayCommand PatchCommand => new RelayCommand(ExecutePatch);
    public RelayCommand ResetCommand => new RelayCommand(Reset);

    private void CopyFiles(string baseDirectory, Version version)
    {
        var patchFiles = Directory.GetFiles(baseDirectory, "*", SearchOption.AllDirectories);
        foreach (var file in patchFiles)
        {
            var relativePath = file.TrimStart(baseDirectory);
            var targetPath = PlayniteEmulationDirectory + relativePath;
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            File.Copy(file, targetPath, overwrite: true);
        }

        Settings.InstalledPatchVersion = version;
        Plugin.SavePluginSettings(Settings);

        PlayniteApi.Dialogs.ShowMessage(
            $"Copied {patchFiles.Length} files to the emulator profiles directory. Restart Playnite to apply these changes.",
            "Extra Emulator Profiles",
            System.Windows.MessageBoxButton.OK,
            System.Windows.MessageBoxImage.Information);
    }

    private void DeleteFiles(string directory, string filter = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        var files = Directory.GetFiles(directory, filter, searchOption);
        foreach (var f in files)
        {
            File.Delete(f);
        }
    }
}