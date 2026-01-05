using PlayNext.Settings;
using PlayNext.Settings.Presets;
using TestTools.Shared;
using Xunit;

namespace PlayNext.UnitTests.Settings.Preset
{
	public class SettingsPresetTests
	{
		[Theory]
		[AutoMoqData]
		public void ToNew_Copies_PropertiesAndAssignsNewSettings(SettingsPreset<PlayNextSettings> preset, PlayNextSettings settings)
		{
			// Act
			var result = preset.ToNew(settings);

			// Assert
			Assert.Equal(preset.Id, result.Id);
			Assert.Equal(preset.Name, result.Name);
			Assert.Equal(settings, result.Settings);
		}
	}
}