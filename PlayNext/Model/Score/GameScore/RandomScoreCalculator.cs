using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.Model.Score.GameScore
{
	public class RandomScoreCalculator : IRandomScoreCalculator
	{
		private readonly Random _random = new Random();

		public Dictionary<Guid, float> Calculate(IEnumerable<Game> games)
		{
			return games.ToDictionary(x => x.Id, x => (float)_random.NextDouble() * 100);
		}
	}
}