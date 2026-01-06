using System;
using System.Collections.Generic;
using Playnite.SDK.Models;

namespace PlayNext.Model.Score.GameScore
{
	public interface IRandomScoreCalculator
	{
		Dictionary<Guid, float> Calculate(IEnumerable<Game> games);
	}
}