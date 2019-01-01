using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Ignitus
{
    public class LoadingWheelElement : HudElement
    {
        public delegate void LoadingMethodDelegate(object[] objects);

        string spriteName;
        Color color;
        float angle;
        float angleSpeed;
        float time;
        int maxTime;
        bool skippable;
        float alpha;
        bool loaded;

        LoadingMethodDelegate preLoadingMethod;
        LoadingMethodDelegate loadingMethod;
        string targetMode;
        object[] objects;

        public bool Loaded { get { return loaded; } set { loaded = value; } }
        public float Alpha { get { return alpha; } set { alpha = value; } }
        public float Time { get { return time; } set { time = value; } }
        public int MaxTime { get { return maxTime; } set { maxTime = value; } }
        public bool Skippable { get { return skippable; } set { skippable = value; } }
        public string TargetMode { get { return targetMode; } set { targetMode = value; } }
        public LoadingMethodDelegate LoadingMethod { get { return loadingMethod; } set { loadingMethod = value; } }
        public object[] Objects { get { return objects; } set { objects = value; } }
        public LoadingMethodDelegate PreLoadingMethod { get { return preLoadingMethod; } set { preLoadingMethod = value; } }
        public float AngleSpeed { get { return angleSpeed; } set { angleSpeed = value; } }
        public string SpriteName { get { return spriteName; } set { spriteName = value; } }
        public Color Color { get { return color; } set { color = value; } }

        public LoadingWheelElement (string name, int x, int y, int width, int height, string spriteName, int time, bool skippable,
            Color color, float angleSpeed, LoadingMethodDelegate preLoadingMethod, LoadingMethodDelegate loadingMethod, bool ignoreAnimation, bool ignoreBackAnimation):
            base(name,x,y,width,height,false, ignoreAnimation, ignoreBackAnimation)
        {
            this.alpha = 0;
            this.skippable = skippable;
            this.time = time;
            this.maxTime = time;
            this.targetMode = null;
            this.spriteName = spriteName;
            this.color = color;
            this.angle = 0;
            this.angleSpeed = angleSpeed;
            this.preLoadingMethod = preLoadingMethod;
            this.loadingMethod = loadingMethod;
        }

        public LoadingWheelElement (string name, int x, int y, int width, int height, string spriteName, Color color, float angleSpeed,
            bool ignoreAnimation, bool ignoreBackAnimation):
            base(name,x,y,width,height,false, ignoreAnimation, ignoreBackAnimation)
        {
            this.targetMode = null;
            this.spriteName = spriteName;
            this.color = color;
            this.angle = 0;
            this.angleSpeed = angleSpeed;
            this.preLoadingMethod = null;
            this.loadingMethod = null;
        }

        public override void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            angle += angleSpeed*milliseconds/1000;
            if (angle > MathHelper.TwoPi) angle -= MathHelper.TwoPi;
            time = Math.Max(0, time -= milliseconds);
            if(loaded && alpha<1)
            {
                alpha += milliseconds / 200;
            }
            if (skippable && loaded)
            {
                bool b = false;
                for (int i = 0; i < state.KeysState.Length; i++)
                {
                    b = b || state.KeysState[i];
                }
                if (b) time = 0;
            }
            if(CanEnd())
            {
                game.GoToMode(targetMode);
            }
            /*if(!play && preLoadingMethod!=null)
            {
                preLoadingMethod(game, texturePacks, targetMode);
                play = true;
            }*/
        }

        public void PrepareBeforeMode ()
        {
            alpha = 0;
            loaded = false;
            time = maxTime;
        }

        public void LoadCompleted ()
        {
            loaded = true;
        }

        public bool CanEnd()
        {
            return loaded && time <= 0;
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {

        }

        public override void Draw(IgnitusGame game, Matrix animation, Microsoft.Xna.Framework.Color fonColor, float milliseconds)
        {
            game.DrawSprite(spriteName, new Rectangle(X, Y, Width, Height), new Color(color.R * fonColor.R / 255,
                color.G * fonColor.G / 255, color.B * fonColor.B / 255, color.A * fonColor.A / 255 * alpha), angle, SpriteEffects.None, 0);
        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Microsoft.Xna.Framework.Color fonColor)
        {
            
        }

        public override string ToString()
        {
            return "LoadingWheelElement";
        }
    }
}
