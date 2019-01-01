using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public delegate void PressButtonAction(IgnitusGame game, Mode mode, HudElement element);
    public class ButtonElement : HudElement
    {
        string text;
        bool textAlign;
        Color textColor;
        Color selectedColor;
        Color pressedColor;
        string font;

        bool selected;
        bool pressed;
        bool way;
        PressButtonAction action;
        bool selectedChange;

        public PressButtonAction Action { get { return action; } }
        public bool Selected { get { return selected; } set { selected = value; } }
        public bool Pressed { get { return pressed; } set { pressed = value; } }
        public string Text { get { return text; } set { text = value; } }
        public bool TextAlign { get { return textAlign; } }
        public Color TextColor { get { return textColor; } }
        public Color SelectedColor { get { return selectedColor; } }
        public Color PressedColor { get { return pressedColor; } }
        public string Font { get { return font; } set { font = value; } }

        public ButtonElement (string name, int x, int y, int width, int height, string text, string font, bool align,
            Color textColor, Color selectedColor, Color pressedColor, PressButtonAction action, bool ignoreAnimation, bool ignoreBackAnimation):
            base (name,x,y,width,height,false, ignoreAnimation, ignoreBackAnimation)
        {
            selected = false;
            pressed = false;
            this.action = action;
            this.text = text;
            this.textAlign = align;
            this.textColor = textColor;
            this.selectedColor = selectedColor;
            this.pressedColor = pressedColor;
            this.font = font;
        }

        public override void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            if (selected == true && selectedChange==false &&!pressed)
            {
                game.PlaySound("click", 0.4f, MathHelper.PiOver2);
            }
            if (selectedChange == false)
            {
                selected = false;
            }
            selectedChange = false;
            if (!state.LeftButtonState && !state.KeysState[0] &&
                !prevState.LeftButtonState && !prevState.KeysState[0])
            {
                pressed = false;
            }
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            if(selected==false&&!pressed)
            {
                selectedChange = true;
                game.PlaySound("click", 0.4f, MathHelper.PiOver2);
            }
            selected = true;
            selectedChange = true;
            Point correctedMousePos = base.TransformPointToElementCoords(state.MousePosition);
            if(base.CheckMousePositionInElement(state.MousePosition) &&
                state.LeftButtonState && !prevState.LeftButtonState ||
                state.KeysState[0] && !prevState.KeysState[0] && mode.KeyboardUse)
            {
                way = state.LeftButtonState && !prevState.LeftButtonState;
                pressed = true;
            }
            if(!state.LeftButtonState && prevState.LeftButtonState && way ||
                !state.KeysState[0] && prevState.KeysState[0] && !way)
            {
                if (pressed && (base.CheckMousePositionInElement(state.MousePosition) || !way))
                {
                    action(game, mode,this);
                }
                pressed = false;
            }
            ArrowsMechanics(mode, state);
        }

        public override void Draw(IgnitusGame game, Matrix animation, Microsoft.Xna.Framework.Color fonColor, float milliseconds)
        {
            Color color;
            if(pressed)
            {
                color = pressedColor;
            }
            else if (selected)
            {
                color = selectedColor;
            }
            else
            {
                color = textColor;
            }
            color.R = (byte)(color.R*(float)fonColor.R / 255);
            color.G = (byte)(color.G * (float)fonColor.G / 255);
            color.B = (byte)(color.B * (float)fonColor.B / 255);
            color.A = (byte)(color.A * (float)fonColor.A / 255);
            game.DrawString(font, text, textAlign, new Point(X, Y), Width, color);
        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor)
        {
            
        }

        public override string ToString()
        {
            return "ButtonElement";
        }
    }
}
