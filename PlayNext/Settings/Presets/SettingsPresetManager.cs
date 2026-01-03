using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayNext.Settings.Old;
using Playnite.SDK;

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

		public async Task Initialize()
		{
			var presets = await ReadPresets<VersionedSettings>();

			foreach (var preset in presets)
			{
				if (preset.Settings.Version == PlayNextSettings.CurrentVersion)
				{
					continue;
				}

				await MigratePreset(preset);
			}
		}

		public async Task<IReadOnlyCollection<SettingsPreset<PlayNextSettings>>> GetPersistedPresets()
		{
			if (!Directory.Exists(_presetPath))
			{
				return new List<SettingsPreset<PlayNextSettings>>();
			}

			var presets = await ReadPresets<PlayNextSettings>();

			return presets;
		}

		public async Task WritePreset(SettingsPreset<PlayNextSettings> preset)
		{
			await Write(preset);
		}

		private async Task<List<SettingsPreset<T>>> ReadPresets<T>() where T : IVersionedSettings
		{
			var files = Directory.GetFiles(_presetPath);
			var presets = new List<SettingsPreset<T>>();
			foreach (var file in files)
			{
				try
				{
					var preset = await ReadPreset<T>(file);
					presets.Add(preset);
				}
				catch (Exception ex)
				{
					_logger.Warn(ex, $"Could not read settings preset file: {file}");
				}
			}

			return presets;
		}

		private async Task<SettingsPreset<T>> ReadPreset<T>(string file) where T : IVersionedSettings
		{
			using (var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
			using (var streamReader = new StreamReader(fileStream))
			{
				var json = await streamReader.ReadToEndAsync();

				var preset = JsonConvert.DeserializeObject<SettingsPreset<T>>(json);
				return preset;
			}
		}

		private async Task MigratePreset(SettingsPreset<VersionedSettings> preset)
		{
			IVersionedSettings oldSettings;
			switch (preset.Settings.Version)
			{
				case 0:
					var oldPresetV0 = await ReadPreset<SettingsV0>(Path.Combine(_presetPath, $"{preset.Id}.json"));
					oldSettings = oldPresetV0.Settings;
					break;

				case 1:
					var oldPresetV1 = await ReadPreset<SettingsV1>(Path.Combine(_presetPath, $"{preset.Id}.json"));
					oldSettings = oldPresetV1.Settings;
					break;

				case 2:
					var oldPresetV2 = await ReadPreset<SettingsV2>(Path.Combine(_presetPath, $"{preset.Id}.json"));
					oldSettings = oldPresetV2.Settings;
					break;

				default:
					throw new NotImplementedException($"No implementation for preset v{preset.Settings.Version} migration");
			}

			var updatedSettings = _settingsMigrator.MigrateToNewest(oldSettings);
			var newPreset = preset.ToNew(updatedSettings);

			await Write(newPreset);
		}

		public async Task Write<T>(SettingsPreset<T> preset) where T : IVersionedSettings
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
				await streamWriter.WriteAsync(json);
			}
		}
	}
}