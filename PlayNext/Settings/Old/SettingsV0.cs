using PlayNext.Model.Data;

namespace PlayNext.Settings.Old
{
    public class SettingsV0 : IMigratableSettings
    {
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

        public ReleaseYearChoice ReleaseYearChoice { get; set; }

        public int NumberOfTopGames { get; set; }

        public int RecentDays { get; set; }

        public int Version { get; set; }

        public virtual IVersionedSettings Migrate()
        {
            var settings = SettingsV1.Default;
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