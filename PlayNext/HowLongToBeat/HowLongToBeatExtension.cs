using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PlayNext.HowLongToBeat.Data;
using Playnite.SDK;
using Playnite.SDK.Models;

namespace PlayNext.HowLongToBeat
{
    public class HowLongToBeatExtension : IHowLongToBeatExtension
    {
        private static Guid _extensionId = Guid.Parse("e08cd51f-9c9a-4ee3-a094-fde03b55492f");
        private readonly ILogger _logger = LogManager.GetLogger(nameof(HowLongToBeatExtension));

        private readonly string _dataPath;
        private Dictionary<Guid, int> _hltbData = new Dictionary<Guid, int>();

        public HowLongToBeatExtension(string dataPath)
        {
            _dataPath = dataPath;
        }

        public static HowLongToBeatExtension Create(string extensionsDataPath)
        {
            var dataPath = Path.Combine(extensionsDataPath, _extensionId.ToString(), "HowLongToBeat");
            return Directory.Exists(dataPath)
                ? new HowLongToBeatExtension(dataPath)
                : new HowLongToBeatExtension(null);
        }

        public bool DoesDataExist()
        {
            return Directory.Exists(_dataPath);
        }

        public async Task ParseFiles(IEnumerable<Game> games)
        {
            try
            {
                if (!DoesDataExist())
                {
                    return;
                }

                var files = Directory.GetFiles(_dataPath);
                var validFiles = files
                    .Where(path =>
                        Guid.TryParse(Path.GetFileNameWithoutExtension(path), out var id) &&
                        games.Any(x => x.Id == id));

                var datas = await Task.WhenAll(validFiles.Select(DeserializeFile));

                _hltbData = datas.Where(x => x != null)
                    .ToDictionary(x => x.Id, x =>
                        x.Items.Sum(i => AverageTimes(i.GameHltbData)) / x.Items.Count)
                    .Where(x => x.Value > 0)
                    .ToDictionary(x => x.Key, x => x.Value);

                _logger.Info($"{_hltbData.Count} games with HowLongToBeat data found");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failure reading HowLongToBeat files");
            }
        }

        private static int AverageTimes(GameHltbData data)
        {
            var nonZeroTimes = 0;
            if (data.MainStory > 0)
            {
                nonZeroTimes++;
            }

            if (data.MainExtra > 0)
            {
                nonZeroTimes++;
            }

            if (data.Completionist > 0)
            {
                nonZeroTimes++;
            }

            return nonZeroTimes > 0
                ? (data.MainStory + data.MainExtra + data.Completionist) / nonZeroTimes
                : 0;
        }

        public Dictionary<Guid, int> GetTimeToPlay()
        {
            return _hltbData;
        }

        private async Task<HltbFile> DeserializeFile(string file)
        {
            try
            {
                using (var reader = new StreamReader(file))
                {
                    var json = await reader.ReadToEndAsync();
                    return JsonConvert.DeserializeObject<HltbFile>(json);
                }
            }
            catch
            {
                _logger.Warn($"Cloud not parse HowLongToBeat file: {file}");
                return null;
            }
        }
    }
}