using PlayNext.Settings;
using Playnite.SDK.Models;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.Model.Filters
{
	public class ExclusionListFilter
	{
		public IReadOnlyCollection<Game> Filter(IEnumerable<Game> games, PlayNextSettings settings)
		{
			return games
				.Where(x => !settings.ExcludedSourceIds.Contains(x.SourceId))
				.Where(x => x.PlatformIds == null || settings.ExcludedPlatformIds.All(e => !x.PlatformIds.Contains(e)))
				.Where(x => x.CategoryIds == null || settings.ExcludedCategoryIds.All(e => !x.CategoryIds.Contains(e)))
				.Where(x => x.TagIds == null || settings.ExcludedTagIds.All(e => !x.TagIds.Contains(e)))
				.ToList();
		}
	}
}