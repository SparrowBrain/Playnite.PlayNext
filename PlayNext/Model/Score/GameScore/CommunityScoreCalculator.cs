using Playnite.SDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.Model.Score.GameScore
{
	public class CommunityScoreCalculator
	{
		public Dictionary<Guid, float> Calculate(IEnumerable<Game> games)
		{
			return games
				.Where(x => x.CommunityScore.HasValue)
				.ToDictionary(x => x.Id, x => x.CommunityScore ?? 0f);
		}
	}
}