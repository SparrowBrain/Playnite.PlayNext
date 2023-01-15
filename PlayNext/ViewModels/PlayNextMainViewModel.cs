using System.Collections.Generic;
using PlayNext.Models;

namespace PlayNext.ViewModels
{
    public class PlayNextMainViewModel : ObservableObject
    {
        private readonly PlayNext _plugin;

        public PlayNextMainViewModel(PlayNext plugin)
        {
            _plugin = plugin;

            // Load saved settings.
            //var savedSettings = plugin.LoadPluginSettings<PlayNextSettings>();

            Games.Add(new GameToPlay("BETS GAME"));
        }

        public List<GameToPlay> Games { get; } = new List<GameToPlay>();
    }
}