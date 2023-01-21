using System;
using Playnite.SDK.Models;

namespace PlayNext.Models
{
    public class GameToPlay
    {
        public GameToPlay(Game game, float score)
        {
            Id = game.Id;
            Name = game.Name;
            Score = score;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Score { get; }
        public string CoverImage { get; set; }

        public bool ShowCoverImage
        {
            get => CoverImage != null;
        }
    }
}