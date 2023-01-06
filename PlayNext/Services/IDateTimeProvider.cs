using System;

namespace PlayNext.Services
{
    public interface IDateTimeProvider
    {
        DateTime GetNow();
    }
}