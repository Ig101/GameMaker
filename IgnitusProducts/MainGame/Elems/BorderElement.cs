using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public class BorderElement:HudElement
    {
        string spriteName;
        Color color;
        float size;

        public string SpriteName { get { return spriteName; } }
        public Color Color { get { return color; } }
        public float Size { get { return size; } }

        public BorderElement (string name, int x, int y, int width, int height, string spriteName, 
            Color color, float size, bool ignoreAnimation, bool ignoreBackAnimation):
            base(name,x,y,width,height,true, ignoreAnimation, ignoreBackAnimation)
        {
            this.size = size;
            this.spriteName = spriteName;
            this.color = color;
        }

        public override void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            
        }

        public override void Draw(IgnitusGame game, Matrix animation, Microsoft.Xna.Framework.Color fonColor, float milliseconds)
        {
            game.DrawBorder(spriteName, size, new Rectangle(X, Y, Width, Height), new Color(fonColor.R*color.R/255,fonColor.G*color.G/255,
                fonColor.B*color.B/255,fonColor.A*color.A/255), 0);
        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor)
        {
            
        }

        public override string ToString()
        {
            return "BorderElement";
        }
    }
}
