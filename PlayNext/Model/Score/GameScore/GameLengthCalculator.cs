using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.Model.Score.GameScore
{
	public class GameLengthCalculator
	{
		public Dictionary<Guid, float> Calculate(Dictionary<Guid, int> games, TimeSpan length)
		{
			if (games == null || !games.Any())
			{
				return new Dictionary<Guid, float>();
			}

			var preferredLengthInSeconds = (int)length.TotalSeconds;
			var deviation = preferredLengthInSeconds <= 1 ? 3600 : preferredLengthInSeconds / 2;

			return games.ToDictionary(x => x.Key, x => (deviation - Math.Min(deviation, Math.Abs(x.Value - preferredLengthInSeconds))) * 100f / deviation);
		}
	}
}