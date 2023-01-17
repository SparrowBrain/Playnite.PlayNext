using System;
using Playnite.SDK.Models;

namespace PlayNext.Models
{
    public class GameToPlay
    {
        public GameToPlay(Game game, float score)
        {
            Score = score;
            Id = game.Id;
            Name = game.Name;
        }


        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Score { get; }
    }
}