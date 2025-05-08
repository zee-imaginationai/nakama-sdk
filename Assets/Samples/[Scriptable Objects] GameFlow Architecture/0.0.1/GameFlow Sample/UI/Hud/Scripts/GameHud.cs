using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ProjectCore.Hud
{
    public class GameHud : MonoBehaviour
    {
        [SerializeField] protected GameHudBar Header;
        [SerializeField] protected GameHudBar Footer;
        [SerializeField] protected GameHudBar RightGameHudBar;

        [SerializeField] protected List<GameHudBar> GameHudBars;
        
        virtual public void Show()
        {
            // if (Header != null)
            // {
            //     Header.Show();
            // }
            //
            // if (Footer!=null)
            // {
            //     Footer.Show();
            // }
            //
            // if (RightGameHudBar!=null)
            // {
            //     RightGameHudBar.Show();
            // }
            //Debug.LogError($"Showing HUD");

            for (int i = 0; i < GameHudBars.Count; i++)
            {
                var gameHudBar = GameHudBars[i];
                if (gameHudBar!=null)
                {
                    gameHudBar.Show();
                }
            }

        }

        virtual public void Hide()
        {
            // if (Header != null)
            // {
            //     Header.Hide();
            // }
            //
            // if (Footer!=null)
            // {
            //     Footer.Hide();
            // }
            //
            // if (RightGameHudBar!=null)
            // {
            //     RightGameHudBar.Hide();
            // }
            
            //Debug.LogError($"Hiding HUD");
            
            for (int i = 0; i < GameHudBars.Count; i++)
            {
                var gameHudBar = GameHudBars[i];
                if (gameHudBar!=null)
                {
                    gameHudBar.Hide();
                }
            }
        }
    }
}