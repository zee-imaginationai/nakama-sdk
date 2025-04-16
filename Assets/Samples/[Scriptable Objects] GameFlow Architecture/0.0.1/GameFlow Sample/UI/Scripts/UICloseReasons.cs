namespace ProjectCore.UI
{
    public enum UICloseReasons
    {
        Home,
        Game, //same for retry or restart whatever
        SkipLevel,
        Revive,
        ResumeGame,
        FullScreenPlacement,
        ResumeAny,
        DailyLogin,
        /// <summary>
        /// special case should be handled with great care
        /// </summary>
        ShowFullScreenPlacement,
    }
}