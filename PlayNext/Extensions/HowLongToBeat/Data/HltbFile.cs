using System;
using System.Collections.Generic;

namespace PlayNext.Extensions.HowLongToBeat.Data
{
    public class HltbFile
    {
        public Guid Id { get; set; }
        public List<HltbItem> Items { get; set; }
    }
}