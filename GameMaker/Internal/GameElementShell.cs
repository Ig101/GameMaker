using Ignitus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameMaker
{
    public abstract class GameElementShell: HudElement
    {
        const int coff = 500;
        //ForStart
        float animationProgress;
        //AndEnd
        bool screenshotNeed;
        bool screenshotDone;
        bool screenshotShow;
        string targetMode;
        Texture2D fon;
        Texture2D sampleFon;
        string fonName;
        //ForButton
        float buttonAnimationProgress;
        bool buttonDirection;

        public string FonName { get { return fonName; } }

        public GameElementShell (string fonName)
            :base("game",int.MinValue/2,int.MinValue/2,int.MaxValue,int.MaxValue,false,false,false)
        {
            animationProgress = 1;
            screenshotShow = true;
            this.fonName = fonName;
        }

        public override void PassiveUpdate(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            if (!screenshotShow)
            {
                UpdateAll(this, state, prevState, milliseconds);
            }
            if (screenshotDone) game.GoToMode(targetMode);
        }

        public override void Update(IgnitusGame game, Mode mode, ControlsState state, ControlsState prevState, float milliseconds)
        {
            if (!screenshotShow)
            {
                UpdateButtons(this, state, prevState, milliseconds);
            }
        }

        public override void Draw(IgnitusGame game, Matrix animation, Color fonColor, float milliseconds)
        {
            if (animationProgress < 2)
            {
                bool b = animationProgress < 1;
                animationProgress += milliseconds / coff;
                if (animationProgress >= 1 && b)
                {
                    buttonDirection = true;
                    screenshotShow = false;
                }
                if (animationProgress > 2) animationProgress = 2;
            }
            if (buttonDirection)
            {
                if (buttonAnimationProgress < 1)
                {
                    buttonAnimationProgress += milliseconds / coff;
                    if (buttonAnimationProgress > 1) buttonAnimationProgress = 1;
                }
            }
            else
            {
                if (buttonAnimationProgress > 0)
                {
                    buttonAnimationProgress -= milliseconds / coff;
                    if (buttonAnimationProgress < 0) buttonAnimationProgress = 0;
                }
            }
            //
            game.EndDrawing();
            //
            if (screenshotShow && fon!=null)
            {
                game.BeginDrawingNoScale(SpriteSortMode.Immediate, null);
                game.DrawSprite(fon,
                    new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height),
                    new Rectangle(0, 0, fon.Width, fon.Height)
                    , UpdateFonColor(fonColor), 0, Vector2.Zero, SpriteEffects.None, 0);
                game.EndDrawing();
            }
            else
            {
                DrawAll(game, UpdateFonColor(fonColor));
            }
            //
            if (animation == new Matrix())
            {
                game.BeginDrawing(SpriteSortMode.Immediate, null);
            }
            else
            {
                game.BeginDrawing(SpriteSortMode.Immediate, null, animation);
            }
        }

        public override void DrawPreActionsUpdate(IgnitusGame game, Color fonColor)
        {
            if (screenshotNeed && !screenshotDone && (buttonAnimationProgress<=0 || targetMode == "game_mode_context"))
            {
                if(fon!=null && fon!=sampleFon)
                {
                    fon.Dispose();
                }
                RenderTarget2D target;
                target = new RenderTarget2D(game.GraphicsDevice,
                    game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height, false, SurfaceFormat.Rgba64,
                    DepthFormat.Depth24Stencil8);
                game.GraphicsDevice.SetRenderTarget(target);
                game.GraphicsDevice.Clear(Color.Black);
                DrawAll(game, Color.White);
                game.GraphicsDevice.SetRenderTarget(null);
                fon = target;
                screenshotDone = true;
                screenshotShow = true;
            }
        }

        protected abstract void DrawAll(IgnitusGame game, Color fonColor);

        protected abstract void UpdateAll(GameElementShell gameElement, ControlsState state, ControlsState prevState, float milliseconds);

        protected abstract void UpdateButtons(GameElementShell gameElement, ControlsState state, ControlsState prevState, float milliseconds);

        protected abstract void FirstTimeUpdate(GameElementShell gameElement);

        Color UpdateFonColor (Color fonColor)
        {
            float stage = Math.Abs(animationProgress - 1);
            return new Color((byte)(fonColor.R * stage), (byte)(fonColor.G * stage), (byte)(fonColor.B * stage));
        }

        public float UpdateButtonCoff ()
        {
            return buttonAnimationProgress;
        }

        public void PrepareForFirstUse()
        {
            animationProgress = 0;
            PrepareTo();
            buttonDirection = false;
            FirstTimeUpdate(this);
        }

        public void PrepareTo()
        {
            screenshotDone = false;
            screenshotNeed = false;
            screenshotShow = true;
            buttonDirection = true;
        }

        public void PrepareFrom(IgnitusGame game, string modeName)
        {
            screenshotNeed = true;
            targetMode = modeName;
            buttonDirection = false;
        }

        protected void EndGameAndResult(Game1Shell game, int score)
        {
            game.SetScore(score);
            PrepareFrom(game, "game_mode_result");
        }

        public void PickSampleFon()
        {
            if(this.fon != null && this.fon!=this.sampleFon)
            {
                this.fon.Dispose();
            }
            fon = sampleFon;
        }

        public void SetFon(Texture2D fon)
        {
            this.fon = fon;
            this.sampleFon = fon;
        }
    }
}
