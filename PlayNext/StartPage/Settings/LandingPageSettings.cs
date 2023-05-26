using System;
using System.Collections.Generic;
using Playnite.SDK;
using Playnite.SDK.Data;

namespace PlayNext.StartPage.Settings
{
    public class LandingPageSettings : ObservableObject
    {
        private bool fixGridSize = false;
        public bool FixGridSize { get => fixGridSize; set => SetValue(ref fixGridSize, value); }

        private double fixedGridHeight = 1080;
        public double FixedGridHeight { get => fixedGridHeight; set => SetValue(ref fixedGridHeight, value); }

        private double padding = 0;
        public double Padding { get => padding; set => SetValue(ref padding, value); }

        private bool horizontalShelveLabels = false;
        public bool HorizontalShelveLabels { get => horizontalShelveLabels; set => SetValue(ref horizontalShelveLabels, value); }

        private string startPage = ResourceProvider.GetString("LOCLibrary");
        public string StartPage { get => startPage; set => SetValue(ref startPage, value); }

        private bool showClock = true;
        public bool ShowClock { get => showClock; set => SetValue(ref showClock, value); }

        private bool showDetails = true;
        public bool ShowDetails { get => showDetails; set => SetValue(ref showDetails, value); }

        private bool showRecentAchievements = true;
        public bool ShowRecentAchievements { get => showRecentAchievements; set => SetValue(ref showRecentAchievements, value); }

        private bool showRecentGames = true;
        public bool ShowRecentGames { get => showRecentGames; set => SetValue(ref showRecentGames, value); }

        private bool showMostPlayedGames = true;
        public bool ShowMostPlayedGames { get => showMostPlayedGames; set => SetValue(ref showMostPlayedGames, value); }

        private bool showAddedGames = true;
        public bool ShowAddedGames { get => showAddedGames; set => SetValue(ref showAddedGames, value); }

        private bool showFavoriteGames = false;
        public bool ShowFavoriteGames { get => showFavoriteGames; set => SetValue(ref showFavoriteGames, value); }

        private bool showTitleOnCover = true;
        public bool ShowTitleOnCover { get => showTitleOnCover; set => SetValue(ref showTitleOnCover, value); }

        private int maxNumberRecentAchievements = 6;
        public int MaxNumberRecentAchievements { get => maxNumberRecentAchievements; set => SetValue(ref maxNumberRecentAchievements, value); }

        private int maxNumberRecentAchievementsPerGame = 3;
        public int MaxNumberRecentAchievementsPerGame { get => maxNumberRecentAchievementsPerGame; set => SetValue(ref maxNumberRecentAchievementsPerGame, value); }

        private bool enableNotifications = true;
        public bool EnableNotifications { get => enableNotifications; set => SetValue(ref enableNotifications, value); }

        private bool enableGlobalProgressBar = false;
        public bool EnableGlobalProgressBar { get => enableGlobalProgressBar; set => SetValue(ref enableGlobalProgressBar, value); }

        private bool enableGameActivity = true;
        public bool EnableGameActivity { get => enableGameActivity; set => SetValue(ref enableGameActivity, value); }

        private bool minimizeNotificationsOnLaunch = false;
        public bool MinimizeNotificationsOnLaunch { get => minimizeNotificationsOnLaunch; set => SetValue(ref minimizeNotificationsOnLaunch, value); }

        private bool showNotificationButtons = true;
        public bool ShowNotificationButtons { get => showNotificationButtons; set => SetValue(ref showNotificationButtons, value); }

        private bool enableStartupOverride = false;
        public bool EnableStartupOverride { get => enableStartupOverride; set => SetValue(ref enableStartupOverride, value); }

        public bool switchWithLowPriority = false;
        public bool SwitchWithLowPriority { get => switchWithLowPriority; set => SetValue(ref switchWithLowPriority, value); }

        private bool keepInMemory = true;
        public bool KeepInMemory { get => keepInMemory; set => SetValue(ref keepInMemory, value); }

        private double blurAmount = 20;

        public double BlurAmount
        { get => blurAmount; set { SetValue(ref blurAmount, value); OnPropertyChanged(nameof(BlurAmountScaled)); } }

        private double coverAspectRatio = 0.71794;

        public double CoverAspectRatio
        { get => coverAspectRatio; set { SetValue(ref coverAspectRatio, value); } }

        private double maxCoverWidth = 140;

        public double MaxCoverWidth
        { get => maxCoverWidth; set { SetValue(ref maxCoverWidth, value); } }

        private int numberOfGames = 10;

        public int NumberOfGames
        { get => numberOfGames; set { SetValue(ref numberOfGames, value); } }

        [DontSerialize]
        public double BlurAmountScaled { get => Math.Round(blurAmount / renderScale); }

        private double animationDuration = 1;
        public double AnimationDuration { get => animationDuration; set => SetValue(ref animationDuration, value); }

        private int backgroundRefreshInterval = 0;
        public int BackgroundRefreshInterval { get => backgroundRefreshInterval; set => SetValue(ref backgroundRefreshInterval, value); }

        private double backgroundGameInfoOpacity = 0.11;
        public double BackgroundGameInfoOpacity { get => backgroundGameInfoOpacity; set => SetValue(ref backgroundGameInfoOpacity, value); }

        private double overlayOpacity = 0.1;
        public double OverlayOpacity { get => overlayOpacity; set => SetValue(ref overlayOpacity, value); }

        private double noiseOpacity = 0.15;
        public double NoiseOpacity { get => noiseOpacity; set => SetValue(ref noiseOpacity, value); }

        private double renderScale = 0.05;

        public double RenderScale
        { get => renderScale; set { SetValue(ref renderScale, value); OnPropertyChanged(nameof(BlurAmountScaled)); } }

        private bool moveToTopOfList = false;
        public bool MoveToTopOfList { get => moveToTopOfList; set => SetValue(ref moveToTopOfList, value); }

        private string backgroundImagePath = null;
        public string BackgroundImagePath { get => backgroundImagePath; set => SetValue(ref backgroundImagePath, value); }

        private Uri backgroundImageUri = null;
        public Uri BackgroundImageUri { get => backgroundImageUri; set => SetValue(ref backgroundImageUri, value); }

        private Guid? lastRandomBackgroundId = null;
        public Guid? LastRandomBackgroundId { get => lastRandomBackgroundId; set => SetValue(ref lastRandomBackgroundId, value); }

        private bool skipGamesInPreviousShelves = false;
        public bool SkipGamesInPreviousShelves { get => skipGamesInPreviousShelves; set => SetValue(ref skipGamesInPreviousShelves, value); }

        private bool skipGamesInPreviousMostPlayed = false;
        public bool SkipGamesInPreviousMostPlayed { get => skipGamesInPreviousMostPlayed; set => SetValue(ref skipGamesInPreviousMostPlayed, value); }

        private bool enableTagCreation = false;
        public bool EnableTagCreation { get => enableTagCreation; set => enableTagCreation = value; }

        private bool downScaleCovers = true;
        public bool DownScaleCovers { get => downScaleCovers; set => SetValue(ref downScaleCovers, value); }

        private bool lockLayout = false;
        public bool LockLayout { get => lockLayout; set => SetValue(ref lockLayout, value); }

        private bool showCurrentlyPlayedBackground = true;
        public bool ShowCurrentlyPlayedBackground { get => showCurrentlyPlayedBackground; set => SetValue(ref showCurrentlyPlayedBackground, value); }

        private bool disableBlurForCurrentlyPlayed = true;
        public bool DisableBlurForCurrentlyPlayed { get => disableBlurForCurrentlyPlayed; set => SetValue(ref disableBlurForCurrentlyPlayed, value); }

        private bool enableGlobalBackground = false;
        public bool EnableGlobalBackground { get => enableGlobalBackground; set => SetValue(ref enableGlobalBackground, value); }

        private double globalBackgroundOpacity = 0.8;
        public double GlobalBackgroundOpacity { get => globalBackgroundOpacity; set => SetValue(ref globalBackgroundOpacity, value); }

        private Guid ignoreTagId = Guid.Empty;

        public Guid IgnoreTagId
        {
            get => ignoreTagId;
            set => ignoreTagId = value;
        }

        private Guid ignoreMostPlayedTagId = Guid.Empty;

        public Guid IgnoreMostPlayedTagId
        {
            get => ignoreMostPlayedTagId;
            set => ignoreMostPlayedTagId = value;
        }

        private bool showHiddenInMostPlayed = false;
        public bool ShowHiddenInMostPlayed { get => showHiddenInMostPlayed; set => SetValue(ref showHiddenInMostPlayed, value); }
    }
}