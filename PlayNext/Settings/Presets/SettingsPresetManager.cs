using Newtonsoft.Json;
using PlayNext.Settings.Old;
using Playnite.SDK;
using System;
using System.Collections.Generic;
using System.IO;

namespace PlayNext.Settings.Presets
{
	public class SettingsPresetManager
	{
		private readonly string _presetPath;
		private readonly ILogger _logger = LogManager.GetLogger();
		private readonly ISettingsMigrator _settingsMigrator;

		public SettingsPresetManager(string extensionDataPath, ISettingsMigrator settingsMigrator)
		{
			_settingsMigrator = settingsMigrator;
			_presetPath = Path.Combine(extensionDataPath, "Presets");
		}

		public void Initialize()
		{
			var presets = ReadPresets<VersionedSettings>();

			foreach (var preset in presets)
			{
				if (preset.Settings.Version == PlayNextSettings.CurrentVersion)
				{
					continue;
				}

				MigratePreset(preset);
			}
		}

		public IReadOnlyCollection<SettingsPreset<PlayNextSettings>> GetPersistedPresets()
		{
			if (!Directory.Exists(_presetPath))
			{
				return new List<SettingsPreset<PlayNextSettings>>();
			}

			var presets = ReadPresets<PlayNextSettings>();

			return presets;
		}

		public void WritePreset(SettingsPreset<PlayNextSettings> preset)
		{
			Write(preset);
		}

		private List<SettingsPreset<T>> ReadPresets<T>() where T : IVersionedSettings
		{
			var files = Directory.GetFiles(_presetPath);
			var presets = new List<SettingsPreset<T>>();
			foreach (var file in files)
			{
				try
				{
					var preset = ReadPreset<T>(file);
					presets.Add(preset);
				}
				catch (Exception ex)
				{
					_logger.Warn(ex, $"Could not read settings preset file: {file}");
				}
			}

			return presets;
		}

		private SettingsPreset<T> ReadPreset<T>(string file) where T : IVersionedSettings
		{
			using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
			using (var streamReader = new StreamReader(fileStream))
			{
				var json = streamReader.ReadToEnd();

				var preset = JsonConvert.DeserializeObject<SettingsPreset<T>>(json);
				return preset;
			}
		}

		private void MigratePreset(SettingsPreset<VersionedSettings> preset)
		{
			IVersionedSettings oldSettings;
			switch (preset.Settings.Version)
			{
				case 0:
					var oldPresetV0 = ReadPreset<SettingsV0>(Path.Combine(_presetPath, $"{preset.Id}.json"));
					oldSettings = oldPresetV0.Settings;
					break;

				case 1:
					var oldPresetV1 = ReadPreset<SettingsV1>(Path.Combine(_presetPath, $"{preset.Id}.json"));
					oldSettings = oldPresetV1.Settings;
					break;

				case 2:
					var oldPresetV2 = ReadPreset<SettingsV2>(Path.Combine(_presetPath, $"{preset.Id}.json"));
					oldSettings = oldPresetV2.Settings;
					break;

				default:
					throw new NotImplementedException($"No implementation for preset v{preset.Settings.Version} migration");
			}

			var updatedSettings = _settingsMigrator.MigrateToNewest(oldSettings);
			var newPreset = preset.ToNew(updatedSettings);

			Write(newPreset);
		}

		public void Write<T>(SettingsPreset<T> preset) where T : IVersionedSettings
		{
			if (!Directory.Exists(_presetPath))
			{
				Directory.CreateDirectory(_presetPath);
			}

			var json = JsonConvert.SerializeObject(preset);
			var file = Path.Combine(_presetPath, $"{preset.Id}.json");
			using (var fileStream = new FileStream(file, FileMode.Create, FileAccess.Write))
			using (var streamWriter = new StreamWriter(fileStream))
			{
				streamWriter.Write(json);
			}
		}
	}
}