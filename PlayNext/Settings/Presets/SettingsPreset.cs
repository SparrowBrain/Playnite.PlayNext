using System;

namespace PlayNext.Settings.Presets
{
	public class SettingsPreset<T> where T : IVersionedSettings
	{
		public Guid Id { get; set; }

		public string Name { get; set; }

		public T Settings { get; set; }

		public SettingsPreset<PlayNextSettings> ToNew(PlayNextSettings settings)
		{
			return new SettingsPreset<PlayNextSettings>()
			{
				Id = Id,
				Name = Name,
				Settings = settings,
			};
		}
	}
}