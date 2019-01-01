using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameMaker;
using Ignitus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game
{
    class GameElementTemplate:GameElementShell
    {
        public GameElementTemplate(string fon)
            :base(fon)
        {
            //Put your base logic here
        }

        protected override void DrawAll(IgnitusGame game, Color fonColor)
        {
            //Put your drawing logic here
            game.BeginDrawing(SpriteSortMode.FrontToBack, null);
            game.DrawString("mediumFont", "Template", false, new Point(0, 500), 2560, fonColor);
            game.EndDrawing();
        }

        protected override void UpdateAll(GameElementShell gameElement, ControlsState state, ControlsState prevState, float milliseconds)
        {
            //Put your main update logic here
        }

        protected override void UpdateButtons(GameElementShell gameElement, ControlsState state, ControlsState prevState, float milliseconds)
        {
            //Put your buttons logic here (not keyboard keys)
        }

        protected override void FirstTimeUpdate(GameElementShell gameElement)
        {
            //Put your launch logic here
        }
    }
}
