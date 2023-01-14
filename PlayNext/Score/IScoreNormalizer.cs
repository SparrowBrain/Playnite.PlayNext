using System;
using System.Collections.Generic;

namespace PlayNext.Score
{
    public interface IScoreNormalizer
    {
        IDictionary<Guid, float> Normalize(IDictionary<Guid, float> scores);
    }
}