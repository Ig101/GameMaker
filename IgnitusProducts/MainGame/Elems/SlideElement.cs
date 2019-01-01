using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public class SlideElement : HudElement
    {
        string spriteName;
        Color color;
        Color selectedColor;
        Color pressedColor;

        bool selected;
        bool pressed;
        PressButtonAction action;
        float position;
        int side;
        bool selectedChange;
        bool windowColor;
        float untouchedPart;

        bool rotated;

        public float UntouchedPart { get { return untouchedPart; } set { untouchedPart = value; } }
        public bool WindowColor { get { return windowColor; } set { windowColor = value; } }
        public bool Rotated { get { return rotated; } set { rotated = value; } }
        public int Side { get { return side; } set { side = value; } }
        public float Position { get { return position; } set { position = value; } }
        public PressButtonAction Action { get { return action; } }
        public bool Selected { get { return selected; } set { selected = value; } }
        public bool Pressed { get { return pressed; } set { pressed = value; } }
        public string SpriteName { get { return spriteName; } set { spriteName = value; } }
        public Color TextColor { get { return color; } }
        public Color SelectedColor { get { return selectedColor; } }
        public Color PressedColor { get { return pressedColor; } }

        public SlideElement (string name, int x, int y, int width, int height, string spriteName,
            Color color, Color selectedColor, Color pressedColor, PressButtonAction action, int side, bool rotated, bool windowColor, float untouchedPart,
            bool ignoreAnimation, bool ignoreBackAnimation):
            base (name,x,y,width,height,false, ignoreAnimation, ignoreBackAnimation)
        {
            this.untouchedPart = untouchedPart;
            this.windowColor = windowColor;
            this.rotated = rotated;
            this.side = side;
            selected = false;
            pressed = false;
            this.action = action;
            this.spriteName = spriteName;
            this.color = color;
            this.selectedColor = selectedColor;
            this.pressedColor = pressedColor;
            position = 0;
        }

        public override void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            if (selected == true && selectedChange == false && !pressed)
            {
                game.PlaySound("click", 0.4f, MathHelper.PiOver2);
            }
            if (selectedChange == false)
            {
                selected = false;
            }
            selectedChange = false;
            if (!state.LeftButtonState)
            {
                pressed = false;
            }
            else if (pressed==true)
            {
                Point correctedMousePos = base.TransformPointToElementCoords(state.MousePosition);
                if (rotated)
                {
                    position = (float)(correctedMousePos.Y - Height * untouchedPart) / (Height * (1-untouchedPart*2));
                }
                else
                {
                    position = (float)(correctedMousePos.X - Width * untouchedPart) / (Width * (1-untouchedPart*2));
                }
                if (position > 1) position = 1;
                if (position < 0) position = 0;
                Action?.Invoke(game, mode, this);
            }
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            if (selected == false && !pressed)
            {
                selectedChange = true;
                game.PlaySound("click", 0.4f, MathHelper.PiOver2);
            }
            selected = true;
            selectedChange = true;
            if (base.CheckMousePositionInElement(state.MousePosition) && state.LeftButtonState)
            {
                pressed = true;
            }
            ArrowsMechanicsIgnore(mode, state);
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
                color = this.color;
            }
            color.R = (byte)(color.R*(float)fonColor.R / 255);
            color.G = (byte)(color.G * (float)fonColor.G / 255);
            color.B = (byte)(color.B * (float)fonColor.B / 255);
            color.A = (byte)(color.A * (float)fonColor.A / 255);
            if (rotated)
            {
                game.DrawSprite(spriteName, new Rectangle(X, Y, Width, Height/2), new Rectangle(0, 0, Width / Height * 64, 128),
                    windowColor ? color : fonColor, MathHelper.PiOver2,
                    new Vector2(0, (side - 128) / 2), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
                game.DrawSprite(spriteName, new Rectangle(X, Y + Height / 2, Width, Height/2), new Rectangle(0, 0, Width / Height * 64, 128),
                    windowColor ? color : fonColor, MathHelper.PiOver2,
                    new Vector2(0, (side - 128) / 2), Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally, 0);
                game.DrawSprite(spriteName + "_slide", new Rectangle(X + Width / 2, (int)(Y + (position * (1-untouchedPart*2) + untouchedPart) * Height), Width, Width),
                    color, 0,
                    Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            }
            else
            {
                game.DrawSprite(spriteName, new Rectangle(X, Y, Width / 2, Height), new Rectangle(0, 0, Width / Height * 64, 128),
                    windowColor?color:fonColor, 0,
                     new Vector2(0, (side - 128) / 2), Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
                game.DrawSprite(spriteName, new Rectangle(X + Width / 2, Y, Width / 2, Height), new Rectangle(0, 0, Width / Height * 64, 128),
                    windowColor ? color : fonColor, 0,
                    new Vector2(0, (side - 128) / 2), Microsoft.Xna.Framework.Graphics.SpriteEffects.FlipHorizontally, 0);
                game.DrawSprite(spriteName + "_slide", new Rectangle((int)(X + (position * (1-untouchedPart*2) + untouchedPart) * Width), Y + Height / 2, Height, Height),
                    color, 0,
                    Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            }
        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor)
        {
            
        }

        public override string ToString()
        {
            return "SlideElement";
        }
    }
}
