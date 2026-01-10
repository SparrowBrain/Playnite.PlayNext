using System.Collections.Generic;
using System.Linq;
using Playnite.SDK.Models;

namespace PlayNext.Model.Filters
{
    public class WithPlaytimeFilter
    {
        public IReadOnlyCollection<Game> Filter(IEnumerable<Game> games)
        {
            return games.Where(x => x.Playtime > 0).ToList();
        }
    }
}