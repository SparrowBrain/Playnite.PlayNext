using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.Model.Score.GameScore
{
	public class CriticScoreCalculator
	{
		public Dictionary<Guid, float> Calculate(IEnumerable<Game> games)
		{
			return games
				.Where(x => x.CriticScore.HasValue)
				.ToDictionary(x => x.Id, x => x.CriticScore ?? 0f);
		}
	}
}