namespace ProjectCore.Hud
{
    public class GameHudHeader : GameHudBar
    {
        public override void Show()
        {
            AnimateY(GameHudConfig.HeaderOffScreenPosition, 0);
        }

        public override void Hide()
        {
            AnimateY(0, GameHudConfig.HeaderOffScreenPosition);
        }
    }
}