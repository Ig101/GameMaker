using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public class LabelElement : HudElement
    {
        string text;
        bool convert;
        bool textAlign;
        Color textColor;
        string font;

        public string Text { get { return text; } set { text = value; } }
        public bool Convert { get { return convert; } set { convert = value; } }
        public bool TextAlign { get { return textAlign; } set { textAlign = value; } }
        public Color TextColor { get { return textColor; } set { textColor = value; } }
        public string Font { get { return font; } set { font = value; } }

        public LabelElement (string name, int x, int y, int width, string text, bool convert, 
            bool align, Color color, string font, bool ignoreAnimation, bool ignoreBackAnimation)
            :base (name, x,y,width,0,true, ignoreAnimation, ignoreBackAnimation)
        {
            this.convert = convert;
            this.textAlign = align;
            this.textColor = color;
            this.font = font;
            this.text = text;
        }

        public override void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
        }

        public override void Draw(IgnitusGame game, Matrix animation, Microsoft.Xna.Framework.Color fonColor, float milliseconds)
        {
            game.DrawString(font, convert ? game.Id2Str(text) : text, textAlign,
                new Point(X, Y), Width, new Color(textColor.R * fonColor.R / 255, textColor.G * fonColor.G / 255,
                    textColor.B * fonColor.B / 255, textColor.A*fonColor.A/255));
        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor)
        {

        }

        public override string ToString()
        {
            return "LabelElement";
        }
    }
}
