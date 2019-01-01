using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public class CheckBoxElement : ButtonElement
    {
        bool check;
        bool checkerColor;
        bool windowColor;
        string checkerIcon;

        public bool CheckerColor { get { return checkerColor; } }
        public bool WindowColor { get { return windowColor; } }
        public bool Check { get { return check; } set { check = value; } }
        public string CheckerIcon { get { return checkerIcon; } }

        public CheckBoxElement (string name, int x, int y, int width, int height, string text, string font, bool align, string checkerIcon,
            Color textColor, Color selectedColor, Color pressedColor, bool windowColor, bool checkerColor,
            PressButtonAction action, bool ignoreAnimation, bool ignoreBackAnimation):
            base (name,x,y,width,height,text,font,align,textColor,selectedColor,pressedColor,action,ignoreAnimation,ignoreBackAnimation)
        {
            this.check = false;
            this.checkerIcon = checkerIcon;
            this.windowColor = windowColor;
            this.checkerColor = checkerColor;
        }

        public override void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            base.PassiveUpdate(game, mode, state, prevState, milliseconds);
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            base.Update(game, mode, state, prevState, milliseconds);
        }

        public override void Draw(IgnitusGame game, Matrix animation, Microsoft.Xna.Framework.Color fonColor, float milliseconds)
        {

            Color color;
            if (Pressed)
            {
                color = PressedColor;
            }
            else if (Selected)
            {
                color = SelectedColor;
            }
            else
            {
                color = TextColor;
            }
            color.R = (byte)(color.R * (float)fonColor.R / 255);
            color.G = (byte)(color.G * (float)fonColor.G / 255);
            color.B = (byte)(color.B * (float)fonColor.B / 255);
            color.A = (byte)(color.A * (float)fonColor.A / 255);
            int side = Height;
            game.DrawSprite(checkerIcon, new Rectangle(X+side/2, Y+side/2, side, side), windowColor?color:fonColor, 0, 
                Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            if(check)
            {
                game.DrawSprite(checkerIcon + "_checked", new Rectangle(X+side/2, Y+side/2, side, side), checkerColor?color:fonColor, 0,
                    Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            }
            if(Text!=null) game.DrawString(Font, Text, TextAlign, new Point(X+side+10, Y), Width, color);
        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor)
        {
        
        }

        public override string ToString()
        {
            return "CheckBoxElement";
        }
    }
}
