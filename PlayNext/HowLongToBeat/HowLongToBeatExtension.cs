using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PlayNext.HowLongToBeat.Data;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace PlayNext.HowLongToBeat
{
    public class HowLongToBeatExtension
    {
        private readonly ILogger _logger = LogManager.GetLogger(nameof(HowLongToBeatExtension));

        private readonly string _dataPath;

        public HowLongToBeatExtension(string dataPath)
        {
            _dataPath = dataPath;
        }

        public static HowLongToBeatExtension Create(string extensionsDataPath)
        {
            var dataPath = Directory.GetDirectories(extensionsDataPath, "HowLongToBeat", SearchOption.AllDirectories).FirstOrDefault();

            return !string.IsNullOrEmpty(dataPath)
                ? new HowLongToBeatExtension(dataPath)
                : new HowLongToBeatExtension(null);
        }

        public bool DoesDataExist()
        {
            return Directory.Exists(_dataPath);
        }

        public Dictionary<Guid, int> GetTimeToPlay(IEnumerable<Game> games)
        {
            try
            {
                if (!DoesDataExist())
                {
                    return new Dictionary<Guid, int>();
                }

                var files = Directory.GetFiles(_dataPath);
                var validFiles = files
                    .AsParallel()
                    .Where(path =>
                        Guid.TryParse(Path.GetFileNameWithoutExtension(path), out var id) &&
                        games.Any(x => x.Id == id));

                var hltbData = validFiles
                    .Select(DeserializeFile).Where(x => x != null)
                    .ToDictionary(x => x.Id, x =>
                        x.Items.Sum(i => (i.GameHltbData.MainStory + i.GameHltbData.MainExtra + i.GameHltbData.Completionist) / 3) / x.Items.Count);

                _logger.Info($"{hltbData.Count} games with how long to beat data found");
                return hltbData;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failure reading how long to beat files");
                return new Dictionary<Guid, int>();
            }
        }

        private HltbFile DeserializeFile(string file)
        {
            try
            {
                var json = File.ReadAllText(file);
                return JsonConvert.DeserializeObject<HltbFile>(json);
            }
            catch
            {
                _logger.Warn($"Cloud not parse HowLongToBeat file: {file}");
                return null;
            }
        }
    }
}