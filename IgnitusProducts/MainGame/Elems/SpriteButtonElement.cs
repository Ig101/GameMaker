using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public class SpriteButtonElement : ButtonElement
    {
        string spriteName;
        string selectedSpriteName;
        string pressedSpriteName;
        Color spriteColor;
        Color spriteSelectedColor;
        Color spritePressedColor;
        Rectangle source;

        public Rectangle Source { get { return source; } }
        public Color SpriteSelectedColor { get { return spriteSelectedColor; } }
        public Color SpritePressedColor { get { return spritePressedColor; } }
        public string SpriteName { get { return spriteName; } set { spriteName = value; } }
        public Color SpriteColor { get { return spriteColor; } }
        public string SelectedSpriteName { get { return selectedSpriteName; } }
        public string PressedSpriteName { get { return pressedSpriteName; } }
        
        public SpriteButtonElement(string name, int x, int y, int width, int height, string text, string font,
            Color textColor, Color selectedColor, Color pressedColor, 
            Color spriteColor, string spriteName, string selectedSpriteName, string pressedSpriteName, Rectangle source, 
            PressButtonAction action, bool ignoreAnimation, bool ignoreBackAnimation):
            base (name, x,y,width, height, text, font, false, textColor, selectedColor, pressedColor, action, ignoreAnimation, ignoreBackAnimation)
        {
            Vector4 colorChanges = spriteColor.ToVector4() / textColor.ToVector4();
            spriteSelectedColor = new Color((byte)(colorChanges.X * selectedColor.R), (byte)(colorChanges.Y * selectedColor.G),
                (byte)(colorChanges.Z * selectedColor.B), (byte)(colorChanges.W * selectedColor.A));
            spritePressedColor = new Color((byte)(colorChanges.X * pressedColor.R), (byte)(colorChanges.Y * pressedColor.G),
                (byte)(colorChanges.Z * pressedColor.B), (byte)(colorChanges.W * pressedColor.A));
            this.spriteColor = spriteColor;
            this.spriteName = spriteName;
            this.source = source;
            this.pressedSpriteName = pressedSpriteName;
            this.selectedSpriteName = selectedSpriteName;
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
                color = spritePressedColor;
            }
            else if (Selected)
            {
                color = spriteSelectedColor;
            }
            else
            {
                color = spriteColor;
            }
            color.R = (byte)(color.R * (float)fonColor.R / 255f);
            color.G = (byte)(color.G * (float)fonColor.G / 255f);
            color.B = (byte)(color.B * fonColor.B / 255f);
            color.A = (byte)(color.A * fonColor.A / 255f);
            string thisSpriteName = spriteName;
            if (Selected && selectedSpriteName!=null)
            {
                thisSpriteName = selectedSpriteName;
            }
            if(Pressed && pressedSpriteName!=null)
            {
                thisSpriteName = pressedSpriteName;
            }
            game.DrawSprite(thisSpriteName, new Rectangle(X, Y, Width, Height), source, color, 0, Vector2.Zero, SpriteEffects.None, 0);
            if (base.Text != "")
            {
                game.DrawString(Font, Text, TextAlign, new Point(X, Y + (Height-game.CalculateStringHeight(Text,Font))/2), Width, color);
            }
        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor)
        {
            
        }

        public override string ToString()
        {
            return "SpriteButtonElement";
        }
    }
}
