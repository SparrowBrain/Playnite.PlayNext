using System.Collections.Generic;
using System.Linq;
using Playnite.SDK.Models;

namespace PlayNext.Filters
{
    public class WithPlaytimeFilter
    {
        public IEnumerable<Game> Filter(IEnumerable<Game> games)
        {
            return games.Where(x => x.Playtime > 0);
        }
    }
}