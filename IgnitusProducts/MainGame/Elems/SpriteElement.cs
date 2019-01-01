using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public class SpriteElement : HudElement
    {
        string spriteName;
        Color color;
        Rectangle source;
        float angle;
        Vector2 origin;
        SpriteEffects effects;

        public string SpriteName { get { return spriteName; } set { spriteName = value; } }
        public Color Color { get { return color; } set { color = value; } }
        public Rectangle Source { get { return source; } }

        public SpriteElement (string name, int x, int y, int width, int height, string spriteName, 
            Color color, Rectangle source, float angle, Vector2 origin, SpriteEffects effects, bool ignoreAnimation, bool ignoreBackAnimation):
            base(name,x,y,width,height,true, ignoreAnimation, ignoreBackAnimation)
        {
            this.spriteName = spriteName;
            this.color = color;
            this.source = source;
            this.angle = angle;
            this.origin = origin;
            this.effects = effects;
        }

        public SpriteElement (string name, int x, int y, int width, int height, string spriteName,
            Color color, Rectangle source, bool ignoreAnimation, bool ignoreBackAnimation):
            base (name,x,y,width,height,true, ignoreAnimation, ignoreBackAnimation)
        {
            this.spriteName=spriteName;
            this.color=color;
            this.source=source;
            this.angle=0;
            this.origin = new Vector2(0,0);
            this.effects = SpriteEffects.None;
        }

        public override void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            
        }

        public override void Draw(IgnitusGame game, Matrix animation, Microsoft.Xna.Framework.Color fonColor, float milliseconds)
        {
            game.DrawSprite(spriteName, new Rectangle(X, Y, Width, Height), source, new Color(color.R*fonColor.R/255,
                color.G*fonColor.G/255,color.B*fonColor.B/255,color.A*fonColor.A/255), angle, origin, effects, 0);
        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor)
        {
            
        }

        public override string ToString()
        {
            return "SpriteElement";
        }
    }
}
