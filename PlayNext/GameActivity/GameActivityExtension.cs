using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using PlayNext.GameActivity.Helpers;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace PlayNext.GameActivity
{
    public class GameActivityExtension
    {
        private readonly ILogger _logger = LogManager.GetLogger(nameof(GameActivityExtension));
        private readonly string _activityPath;
        private readonly SemaphoreSlim _readingFilesSemaphore = new SemaphoreSlim(1);
        private List<Activity> _recentActivities = new List<Activity>();
        private bool _initialized;

        public GameActivityExtension(string activityPath)
        {
            _activityPath = activityPath;
        }

        public static GameActivityExtension Create(IPlayniteAPI api)
        {
            var gameActivityPath = Directory.GetDirectories(api.Paths.ExtensionsDataPath, "GameActivity", SearchOption.AllDirectories).FirstOrDefault();
            if (!string.IsNullOrEmpty(gameActivityPath))
            {
                var gameActivity = new GameActivityExtension(gameActivityPath);
                return gameActivity;
            }
            else
            {
                return new GameActivityExtension(null);
            }
        }

        public event Action ActivityRefreshed;

        public void StartParsingActivity(IEnumerable<Game> recentGames)
        {
            if (string.IsNullOrEmpty(_activityPath) || !Directory.Exists(_activityPath))
            {
                return;
            }

            try
            {
                var files = Directory.GetFiles(_activityPath);
                var validFiles = files
                    .AsParallel()
                    .Where(path =>
                        Guid.TryParse(Path.GetFileNameWithoutExtension(path), out var id) &&
                        recentGames.Any(x => x.Id == id));
                var deserializedFiles = validFiles
                    .Select(DeserializeActivityFile);
                var withSessions = deserializedFiles
                    .Where(activity => (activity?.Items?.Count() ?? 0) > 0);

                _recentActivities = new List<Activity>(withSessions);
                _logger.Info($"{_recentActivities.Count} games with recent activity found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failure reading game activities files");
            }
            finally
            {
                OnActivityRefreshed();
            }
        }

        public IEnumerable<Game> GetRecentPlaytime(IEnumerable<Game> recentGames, PlayNextSettings settings)
        {
            if (_activityPath == null)
            {
                _logger.Info("Game Activity extension not loaded. Returning zeroed playtime.");
                return recentGames.Select(x =>
                {
                    var game = x.GetCopy();
                    game.Playtime = 0;
                    return game;
                });
            }

            var recentPlaytime = recentGames.Select(x =>
            {
                var game = x.GetCopy();
                game.Playtime = _recentActivities.FirstOrDefault(activity => activity.Id == game.Id)?
                    .Items?.Where(session => session.DateSession > DateTime.Now.AddDays(-settings.RecentDays))
                    .Sum(session => session.ElapsedSeconds) ?? 0;
                return game;
            });

            _logger.Debug($"Games with playtime: {recentPlaytime.Count()}");
            return recentPlaytime;
        }

        private Activity DeserializeActivityFile(string file)
        {
            try
            {
                var json = File.ReadAllText(file);
                return JsonConvert.DeserializeObject<Activity>(json);
            }
            catch { return null; }
        }

        protected virtual void OnActivityRefreshed()
        {
            ActivityRefreshed?.Invoke();
        }
    }
}