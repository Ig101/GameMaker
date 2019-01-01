using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public enum Animation { Stand, Move, Attack, Spell1, Spell2, Spell3, Spell4, Spell5 }
    public class Sprite
    {
        Animation animation;
        Animation standAnimation;
        float frame;
        bool direction;
        SpriteInfo info;
        float[] frameSpeed;
        Color color;

        #region properties
        public Color Color { get { return color; } set {color=value;} }
        public SpriteInfo Info { get { return info; } }
        public Animation StandAnimation
        {
            get { return standAnimation; }
            set
            {
                bool b = false;
                if (animation == standAnimation)
                {
                    b = true;
                }
                standAnimation = (byte)standAnimation > info.MaxAnimation ? standAnimation : value;
                if (b) Animation = standAnimation;
            }
        }
        public Animation Animation
        {
            get { return animation; }
            set
            {
                if ((byte)animation < info.MaxAnimation)
                {
                    if (animation != value && value != standAnimation)
                    {
                        Frame = 0;
                    }
                    animation = (byte)animation > info.MaxAnimation ? animation : value;
                    if (frame >= info.MaxFrame[(int)animation])
                    {
                        Frame = 0;
                    }
                }
            }
        }
        public float Frame
        {
            get { return frame; }
            set
            {
                if ((int)value >= info.MaxFrame[(byte)animation])
                {
                    if (animation == standAnimation)
                    {
                        frame = value - info.MaxFrame[(byte)animation];
                    }
                    else
                    {
                        this.animation = standAnimation;
                        frame = 0;
                    }
                }
                else if ((int)value < 0)
                {
                    if (animation == standAnimation)
                    {
                        frame = value + info.MaxFrame[(byte)animation];
                    }
                    else
                    {
                        this.animation = standAnimation;
                        frame = info.MaxFrame[(byte)animation];
                    }
                }
                else
                {
                    frame = value;
                }
            }
        }
        public float[] FrameSpeed { get { return frameSpeed; } set { frameSpeed = value; } }
        public bool Direction { get { return direction; } set { direction = value; } }
        #endregion

        //ObjectConstructor
        public Sprite(SpriteInfo info, Color color)
        {
            this.color = color;
            this.animation = Animation.Stand;
            this.frame = 0;
            this.frameSpeed = new float[info.FrameSpeed.Length];
            info.FrameSpeed.CopyTo(this.FrameSpeed, 0);
            this.direction = true;
            this.standAnimation = Animation.Stand;
            this.info = info;
        }

        public void Update(float milliseconds, float speedMod)
        {
            if (info.MaxFrame[(int)animation] > 0)
            {
                Frame += milliseconds / 16f * frameSpeed[(int)animation] * speedMod * (direction ? 1 : -1);
            }
        }
    }
}
