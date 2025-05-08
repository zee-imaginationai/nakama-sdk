using System;
using ProjectCore.Hud;
using ProjectCore.Variables;
using UnityEngine;

public class GameHudSideBar : GameHudBar
{
    [SerializeField] private SideBarDirection Direction;
    [SerializeField] private GameObject SubmitButton;
    [SerializeField] private GameObject ThemeText;
    [SerializeField] private DBBool IntroFtueShown;
    public GameObject VerticalOverlay;
    public override void Show()
    {

        switch (Direction)
        {
            case SideBarDirection.LEFT:
                AnimateX(GameHudConfig.LeftSideBarOffScreenPosition, 0);
                break;
            case SideBarDirection.RIGHT:
                AnimateX(GameHudConfig.RightSideBarOffScreenPosition, 0);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
       /* if (CharacterData.CharacterAttireData.GetAttireEquipItems() >= 3 && IntroFtueShown.GetValue()==true)
            SubmitButton.SetActive(true);*/
        ThemeText.SetActive(true);
    }

    public override void Hide()
    {
        switch (Direction)
        {
            case SideBarDirection.LEFT:
                AnimateX(0, GameHudConfig.LeftSideBarOffScreenPosition);

                break;
            case SideBarDirection.RIGHT:
                AnimateX(46, GameHudConfig.RightSideBarOffScreenPosition);

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        SubmitButton.SetActive(false);
        ThemeText.SetActive(false);

    }

    private enum SideBarDirection
    {
        LEFT, RIGHT
    }
}

