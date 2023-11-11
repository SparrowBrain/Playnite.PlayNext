using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Playnite.SDK.Models;

namespace PlayNext.HowLongToBeat
{
    public interface IHowLongToBeatExtension
    {
        bool DoesDataExist();
        Task ParseFiles(IEnumerable<Game> games);
        Dictionary<Guid, int> GetTimeToPlay();
    }
}