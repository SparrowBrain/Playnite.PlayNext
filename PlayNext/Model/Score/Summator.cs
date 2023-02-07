using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayNext.Model.Score
{
    public class Summator
    {
        public Dictionary<Guid, float> AddUp(params Dictionary<Guid, float>[] attributeScores)
        {
            var score = new Dictionary<Guid, float>(attributeScores[0]);
            foreach (var attributeScore in attributeScores.Skip(1))
            {
                foreach (var key in attributeScore.Keys)
                {
                    if (score.ContainsKey(key))
                    {
                        score[key] += attributeScore[key];
                    }
                    else
                    {
                        score[key] = attributeScore[key];
                    }
                }
            }

            return score;
        }
    }
}