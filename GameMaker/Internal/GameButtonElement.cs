using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ignitus;
using Microsoft.Xna.Framework;

namespace GameMaker
{
    class GameButtonElement: SpriteButtonElement
    {
        GameElementShell gameElement;

        public GameButtonElement(string name, int x, int y, int width, int height, Color color, Color selectedColor, Color pressedColor,
            string spriteName, string selectedSpriteName, string pressedSpriteName, Rectangle source, PressButtonAction action, GameElementShell gameElement)
            :base(name,x,y,width,height,"","",color,selectedColor,pressedColor,color,spriteName,selectedSpriteName,pressedSpriteName,
                 source,action,false,false)
        {
            this.gameElement = gameElement;
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            float coff = 0;
            if (gameElement == null && game.GetTempMode().Name == "game")
            {
                coff = 1;
            }
            if (gameElement != null)
            {
                coff = gameElement.UpdateButtonCoff();
            }
            if (coff>=0.2f)
                base.Update(game, mode, state, prevState, milliseconds);
        }

        public override void Draw(IgnitusGame game, Matrix animation, Color fonColor, float milliseconds)
        {
            float coff = 0;
            if(gameElement == null && game.GetTempMode().Name == "game")
            {
                coff = 1;
            }
            if(gameElement!=null)
            { 
                coff = gameElement.UpdateButtonCoff();
            }
            if (coff > 0)
            {
                base.Draw(game, animation, new Color((byte)(fonColor.R * coff), (byte)(fonColor.G * coff), (byte)(fonColor.B * coff), (byte)(fonColor.A * coff)),milliseconds);
            }
        }
    }
}
