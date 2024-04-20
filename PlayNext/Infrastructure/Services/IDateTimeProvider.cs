using System;

namespace PlayNext.Infrastructure.Services
{
    public interface IDateTimeProvider
    {
        DateTime GetNow();
    }
}