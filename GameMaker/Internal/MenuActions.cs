using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ignitus;

namespace GameMaker
{
    public static partial class MenuActions
    {
        public static void PauseGame(IgnitusGame game, Mode mode, HudElement element)
        {
            Mode gameMode = (Mode)game.Modes["game_mode"];
            if (gameMode.Elements.Length > 1)
            {
                ((GameElementShell)(gameMode).Elements[0]).PrepareFrom(game,"game_mode_context");
            }
            else
            {
                game.GoToMode("game_mode_context");
            }
        }

        public static void EndGame(IgnitusGame game, Mode mode, HudElement element)
        {
            game.GoToMode("main");
        }

        public static void StartGame(IgnitusGame game, Mode mode, HudElement element)
        {
            Mode gameMode = (Mode)game.Modes["game_mode"];
            if (gameMode.Elements.Length > 1)
            {
                GameElementShell gameElement = (GameElementShell)(gameMode).Elements[0];
                gameElement.PrepareForFirstUse();
            }
            //TODO
            game.GoToMode("game_mode");
        }

        public static void ContinueGame(IgnitusGame game, Mode mode, HudElement element)
        {
            Mode gameMode = (Mode)game.Modes["game_mode"];
            if (gameMode.Elements.Length > 1)
            {
                ((GameElementShell)(gameMode).Elements[0]).PrepareTo();
            }
            game.GoToMode("game_mode");
        }

        public static void Exit (IgnitusGame game, Mode mode, HudElement element)
        {
            game.SaveConfig();
            ((Game1Shell)game).EndGameActivities();
            game.Exit();
        }

        public static void SoundVolume (IgnitusGame game, Mode mode, HudElement element)
        {
            float pos = ((SlideElement)element).Position;
            game.SoundVolume = (int)(pos*100);
            game.Volume = (int)(pos * 100);
            
        }
    }
}
