using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayNext.GameActivity.Helpers;
using PlayNext.Services;
using PlayNext.Settings;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace PlayNext.GameActivity
{
    public class GameActivityExtension
    {
        private static Guid _extensionId = Guid.Parse("afbb1a0d-04a1-4d0c-9afa-c6e42ca855b4");
        private readonly ILogger _logger = LogManager.GetLogger(nameof(GameActivityExtension));
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly string _activityPath;
        private List<Activity> _recentActivities = new List<Activity>();

        public GameActivityExtension(IDateTimeProvider dateTimeProvider, string activityPath)
        {
            _dateTimeProvider = dateTimeProvider;
            _activityPath = activityPath;
        }

        public static GameActivityExtension Create(IDateTimeProvider dateTimeProvider, string extensionsDataPath)
        {
            var dataPath = Path.Combine(extensionsDataPath, _extensionId.ToString(), "GameActivity");
            return Directory.Exists(dataPath)
                ? new GameActivityExtension(dateTimeProvider, dataPath)
                : new GameActivityExtension(dateTimeProvider, null);
        }

        public async Task ParseGameActivity(IEnumerable<Game> recentGames)
        {
            try
            {
                if (!GameActivityPathExists())
                {
                    return;
                }

                var files = Directory.GetFiles(_activityPath);
                var validFiles = files
                    .Where(path =>
                        Guid.TryParse(Path.GetFileNameWithoutExtension(path), out var id) &&
                        recentGames.Any(x => x.Id == id));
                var deserializedFiles = validFiles
                    .Select(DeserializeActivityFile);

                var activities = await Task.WhenAll(deserializedFiles);

                var withSessions = activities
                    .Where(activity => (activity?.Items?.Count() ?? 0) > 0);

                _recentActivities = new List<Activity>(withSessions);
                _logger.Info($"{_recentActivities.Count} games with recent activity found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failure reading game activities files");
            }
        }

        public bool GameActivityPathExists()
        {
            return !string.IsNullOrEmpty(_activityPath) && Directory.Exists(_activityPath);
        }

        public IEnumerable<Game> GetRecentPlaytime(IEnumerable<Game> recentGames, PlayNextSettings settings)
        {
            if (_activityPath == null)
            {
                _logger.Info("Game Activity extension not loaded. Returning empty list.");
                return Array.Empty<Game>();
            }

            var recentPlaytime = recentGames.Select(x =>
            {
                var game = x.GetCopy();
                game.Playtime = _recentActivities.FirstOrDefault(activity => activity.Id == game.Id)?
                    .Items?.Where(session => session.DateSession > _dateTimeProvider.GetNow().AddDays(-settings.RecentDays))
                    .Sum(session => session.ElapsedSeconds) ?? 0;
                return game;
            });

            _logger.Debug($"Games with playtime: {recentPlaytime.Count()}");
            return recentPlaytime;
        }

        private async Task<Activity> DeserializeActivityFile(string file)
        {
            try
            {
                using (var reader = new StreamReader(file))
                {
                    var json = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<Activity>(json);
                }
            }
            catch { return null; }
        }
    }
}