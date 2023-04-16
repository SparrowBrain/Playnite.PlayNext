using System;
using PlayNext.Model.Data;

namespace PlayNext.Settings.Old
{
    public class SettingsV0 : IMigratableSettings
    {
        private bool[] _releaseYearChoices = new bool[Enum.GetValues(typeof(ReleaseYearChoice)).Length];

        public float TotalPlaytimeSerialized { get; set; }

        public float RecentPlaytimeSerialized { get; set; }

        public float RecentOrderSerialized { get; set; }

        public float GenreSerialized { get; set; }

        public float FeatureSerialized { get; set; }

        public float DeveloperSerialized { get; set; }

        public float PublisherSerialized { get; set; }

        public float TagSerialized { get; set; }

        public float CriticScoreSerialized { get; set; }

        public float CommunityScoreSerialized { get; set; }

        public float ReleaseYearSerialized { get; set; }

        public int DesiredReleaseYear { get; set; }

        public ReleaseYearChoice ReleaseYearChoice
        {
            get
            {
                var choice = Array.IndexOf(_releaseYearChoices, true);
                if (choice == -1)
                {
                    choice = 0;
                }

                return (ReleaseYearChoice)choice;
            }
            set
            {
                var newValue = new bool[Enum.GetValues(typeof(ReleaseYearChoice)).Length];
                newValue[(int)value] = true;
                _releaseYearChoices = newValue;
            }
        }

        public int NumberOfTopGames { get; set; }

        public int RecentDays { get; set; }

        public int Version { get; set; }

        public virtual IVersionedSettings Migrate()
        {
            var settings = PlayNextSettings.Default;
            settings.TotalPlaytimeWeight = TotalPlaytimeSerialized;
            settings.RecentPlaytimeWeight = RecentPlaytimeSerialized;
            settings.RecentOrderWeight = RecentOrderSerialized;

            settings.GenreWeight = GenreSerialized;
            settings.FeatureWeight = FeatureSerialized;
            settings.DeveloperWeight = DeveloperSerialized;
            settings.PublisherWeight = PublisherSerialized;
            settings.TagWeight = TagSerialized;
            settings.CriticScoreWeight = CriticScoreSerialized;
            settings.CommunityScoreWeight = CommunityScoreSerialized;
            settings.ReleaseYearWeight = ReleaseYearSerialized;
            settings.ReleaseYearChoice = ReleaseYearChoice;
            settings.DesiredReleaseYear = DesiredReleaseYear;

            settings.RecentDays = RecentDays;
            settings.NumberOfTopGames = NumberOfTopGames;

            return settings;
        }
    }
}