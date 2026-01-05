using AutoFixture;
using AutoFixture.Xunit2;
using Moq;
using Newtonsoft.Json;
using PlayNext.Settings;
using PlayNext.Settings.Presets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using TestTools.Shared;
using Xunit;

namespace PlayNext.IntegrationTests.Settings.Presets
{
	public class SettingsPresetManagerTests : IDisposable
	{
		private const string ExtensionDataPath = @"Tests\SettingsPresetManagerTests";

		private readonly string _presetPath = Path.Combine(ExtensionDataPath, "Presets");
		private readonly SettingsPresetManager _sut;
		private readonly Mock<ISettingsMigrator> _settingsMigrator;

		public SettingsPresetManagerTests()
		{
			_settingsMigrator = new Mock<ISettingsMigrator>();
			_sut = new SettingsPresetManager(ExtensionDataPath, _settingsMigrator.Object);
			EnsurePathExists();
		}

		[Fact]
		public void Initialize_IgnoresNonValidFiles()
		{
			// Arrange
			File.WriteAllText(Path.Combine(_presetPath, $"{Guid.NewGuid()}.json"), "123");

			// Act
			var exception = Record.Exception(() => _sut.Initialize());

			// Assert
			Assert.Null(exception);
		}

		[MemberAutoMoqData(nameof(GetOldPresets))]
		[Theory]
		public void Initialize_MigratesAndWritesPresets_WhenPresetsAreNonCurrentSettings(Guid id, string json, PlayNextSettings currentSettings)
		{
			// Arrange
			var presetFile = Path.Combine(_presetPath, $"{id}.json");
			File.WriteAllText(presetFile, json);
			_settingsMigrator.Setup(x => x.MigrateToNewest(It.IsAny<IVersionedSettings>())).Returns(currentSettings);

			// Act
			_sut.Initialize();

			// Assert
			var actualJson = File.ReadAllText(presetFile);
			var actual = JsonConvert.DeserializeObject<SettingsPreset<VersionedSettings>>(actualJson);
			Assert.Equal(currentSettings.Version, actual.Settings.Version);
		}

		[Theory]
		[AutoData]
		public void Initialize_DoesNotMigrate_WhenPresetsAreCurrentSettings(SettingsPreset<PlayNextSettings> preset)
		{
			// Arrange
			preset.Settings.Version = PlayNextSettings.CurrentVersion;
			var presetFile = Path.Combine(_presetPath, $"{preset.Id}.json");
			File.WriteAllText(presetFile, JsonConvert.SerializeObject(preset));

			// Act
			_sut.Initialize();

			// Assert
			_settingsMigrator.Verify(x => x.MigrateToNewest(It.IsAny<IVersionedSettings>()), Times.Never());
		}

		[Fact]
		public void GetPersistedPresets_ReturnsEmptyList_WhenDirectoryDoesNotExist()
		{
			// Arrange
			EnsurePathDoesNotExist();

			// Act
			var result = _sut.GetPersistedPresets();

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[AutoMoqData]
		public void GetPersistedPresets_ReturnsPresetsOnDisk_WhenFilesExist(
			List<SettingsPreset<PlayNextSettings>> settingsPresets)
		{
			// Arrange
			foreach (var preset in settingsPresets)
			{
				File.WriteAllText(Path.Combine(_presetPath, $"{preset.Id}.json"), JsonConvert.SerializeObject(preset));
			}

			// Act
			var result = _sut.GetPersistedPresets();

			// Assert
			Assert.Equal(settingsPresets.Count, result.Count);
			Assert.All(result, actual => Assert.Contains(settingsPresets, e => e.Id == actual.Id));
		}

		[Fact]
		public void GetPersistedPresets_SkipsFile_WhenFileIsNotInCorrectFormat()
		{
			// Arrange
			File.WriteAllText(Path.Combine(_presetPath, $"{Guid.NewGuid()}.json"), "123");

			// Act
			var result = _sut.GetPersistedPresets();

			// Assert
			Assert.Empty(result);
		}

		[Theory]
		[AutoData]
		public void WritePreset_WritesToDisk_WhenNoFilesExist(SettingsPreset<PlayNextSettings> preset)
		{
			// Act
			_sut.WritePreset(preset);

			// Assert
			var actualJson = File.ReadAllText(Path.Combine(_presetPath, $"{preset.Id}.json"));
			Assert.Equal(JsonConvert.SerializeObject(preset), actualJson);
		}

		[Theory]
		[AutoData]
		public void WritePreset_OverwritesFile_WhenFileExists(SettingsPreset<PlayNextSettings> old, SettingsPreset<PlayNextSettings> expected)
		{
			// Arrange
			File.WriteAllText(Path.Combine(_presetPath, $"{expected.Id}.json"), JsonConvert.SerializeObject(old));

			// Act
			_sut.WritePreset(expected);

			// Assert
			var actualJson = File.ReadAllText(Path.Combine(_presetPath, $"{expected.Id}.json"));
			Assert.Equal(JsonConvert.SerializeObject(expected), actualJson);
		}

		[Theory]
		[AutoData]
		public void WritePreset_CreatesPath_WhenPathDoesNotExist(SettingsPreset<PlayNextSettings> preset)
		{
			// Arrange
			EnsurePathDoesNotExist();

			// Act
			_sut.WritePreset(preset);

			// Assert
			var actualJson = File.ReadAllText(Path.Combine(_presetPath, $"{preset.Id}.json"));
			Assert.Equal(JsonConvert.SerializeObject(preset), actualJson);
		}

		[Theory]
		[AutoData]
		public void DeletePreset_DeletesFile_WhenFileExists(SettingsPreset<PlayNextSettings> preset)
		{
			// Arrange
			var presetFile = Path.Combine(_presetPath, $"{preset.Id}.json");
			File.WriteAllText(presetFile, JsonConvert.SerializeObject(preset));

			// Act
			_sut.DeletePreset(preset.Id);

			// Assert
			Assert.False(File.Exists(presetFile));
		}

		[Theory]
		[AutoData]
		public void DeletePreset_NothingHappens_WhenFileDoesNotExist(SettingsPreset<PlayNextSettings> preset)
		{
			// Arrange
			var presetFile = Path.Combine(_presetPath, $"{preset.Id}.json");

			// Act
			_sut.DeletePreset(preset.Id);

			// Assert
			Assert.False(File.Exists(presetFile));
		}

		[Theory]
		[AutoData]
		public void DeletePreset_NothingHappens_WhenDirectoryNotExist(SettingsPreset<PlayNextSettings> preset)
		{
			// Arrange
			EnsurePathDoesNotExist();
			var presetFile = Path.Combine(_presetPath, $"{preset.Id}.json");

			// Act
			_sut.DeletePreset(preset.Id);

			// Assert
			Assert.False(File.Exists(presetFile));
		}

		public void Dispose()
		{
			if (Directory.Exists(_presetPath))
			{
				Directory.Delete(_presetPath, true);
			}
		}

		private void EnsurePathExists()
		{
			if (!Directory.Exists(_presetPath))
			{
				Directory.CreateDirectory(_presetPath);
			}
		}

		private void EnsurePathDoesNotExist()
		{
			if (Directory.Exists(_presetPath))
			{
				Directory.Delete(_presetPath, true);
			}
		}

		public static IEnumerable<object[]> GetOldPresets()
		{
			var fixture = new Fixture();
			var regex = new Regex(@"\w+V(?<version>\d+)");
			var type = typeof(IVersionedSettings);
			var types = AppDomain.CurrentDomain.GetAssemblies()
				.Where(x => x.FullName.StartsWith("PlayNext"))
				.SelectMany(s => s.GetTypes())
				.Where(x => x.IsClass)
				.Where(p => type.IsAssignableFrom(p))
				.Where(x => regex.IsMatch(x.Name));

			var allOldSettingsVersions = types.Select(x =>
			{
				var settingsPreset = fixture.Create<SettingsPreset<VersionedSettings>>();
				settingsPreset.Settings.Version = int.Parse(regex.Match(x.Name).Groups["version"].Value);

				return new object[] { settingsPreset.Id, JsonConvert.SerializeObject(settingsPreset) };
			});

			return allOldSettingsVersions;
		}
	}
}