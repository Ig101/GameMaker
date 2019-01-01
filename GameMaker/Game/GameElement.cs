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
    class GameElement:GameElementShell
    {
        
        public GameElement(string fon)
            :base(fon)
        {
        }

        protected override void DrawAll(IgnitusGame game, Color fonColor)
        {
            game.BeginDrawing(SpriteSortMode.FrontToBack, null);
            
            game.EndDrawing();
        }

        protected override void UpdateAll(GameElementShell gameElement, ControlsState state, ControlsState prevState, float milliseconds)
        {

        }

        protected override void UpdateButtons(GameElementShell gameElement, ControlsState state, ControlsState prevState, float milliseconds)
        {
            
        }

        protected override void FirstTimeUpdate(GameElementShell gameElement)
        {
            
        }

        void PlayerCommand (int keyIndex, bool state)
        {

        }
    }
}
