using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ignitus
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    
    public struct ControlsState
    {
        Point mousePosition;
        bool leftButtonState;
        bool middleButtonState;
        bool rightButtonState;
        int wheelState;
        bool[] keysState;

        public Point MousePosition { get { return mousePosition; } }
        public bool LeftButtonState { get { return leftButtonState; } }
        public bool MiddleButtonState { get { return middleButtonState; } }
        public bool RightButtonState { get { return rightButtonState; } }
        public int WheelState { get { return wheelState; } }
        public bool[] KeysState { get { return keysState; } }

        public ControlsState (Point mousePosition, bool leftButtonState, bool middleButtonState, bool rightButtonState,
            int wheelState, bool[] keysState)
        {
            this.mousePosition = mousePosition;
            this.leftButtonState = leftButtonState;
            this.middleButtonState = middleButtonState;
            this.rightButtonState = rightButtonState;
            this.wheelState = wheelState;
            this.keysState = keysState;
        }
    }

    public enum Resolution : int { R640x400, R640x480, R800x480, R800x600, R1024x768, R1280x720, R1280x768, R1366x768, RYours };
    public enum Directions {Right,Up,Left,Down};
    public abstract partial class IgnitusGame : Game
    {

        //Media
        protected float volumeCoff;
        protected float volumeChangeSpeed;

        //Cursor
        protected Vector2 cursorShift;
        protected int cursorSize;
        protected float cursorAngle;
        protected string cursorName;

        //Rectangle
        protected Point ingameScreenSize;

        //Another
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        protected Resolution resolution;
        protected Resolution winResolution;
        protected Resolution resolutionToChange;
        protected Resolution winResolutionToChange;
        protected bool fullscreenToChange;
        protected bool fullScreen;

        public bool Fullscreen { get { return fullScreen; } set { fullScreen = value; } }
        public bool FullscreenToChange { get { return fullscreenToChange; } set { fullscreenToChange = value; } }
        public Resolution ResolutionToChange { get { return resolutionToChange; } set { resolutionToChange = value; } }
        public Resolution WinResolutionToChange { get { return winResolutionToChange; } set { winResolutionToChange = value; } }
        public Resolution Resolution { get { return resolution; } set { resolution = value; } }
        public Resolution WinResolution { get { return winResolution; } set { winResolution = value; } }
        public string LanguageName { get { return languageName; } set { languageName = value; } }

        protected float sc;
        protected Matrix scale;
        protected Matrix scaleReverse;
        //
        protected bool misc;
        protected int volume = 50;
        protected int soundVolume = 50;

        public int Volume { get { return volume; } set { volume = value; } }
        public int SoundVolume { get { return soundVolume; } set { soundVolume = value; } }
        public bool Misc { get { return misc; } set { misc = value; } }
        public bool FullScreen { get { return fullScreen; } set { fullScreen = value; } }

        protected Keys[] keys;
        protected string languageName;
        
        public Random r = new Random();
        //
        int fps;
        float time;
        int fpsScreen;
        protected string fpsFont=null;
        //
        protected Mode tempMode;
        protected Mode targetMode;
        //
        protected ControlsState controlsPrevState;
        protected bool noMainAnimation;
        protected Rectangle screenBounds;
        public Rectangle ScreenBounds { get { return screenBounds; } }
        public List<string> Languages { get { return languages; } }
        //
        public IgnitusGame(Vector2 cursorShift, int cursorSize, float cursorAngle, Point ingameScreenSize)
        {
            this.cursorAngle = cursorAngle;
            this.cursorSize = cursorSize;
            this.cursorShift = cursorShift;
            this.ingameScreenSize = ingameScreenSize;
            this.cursorName = "cursor";
            
        }

        protected override void Initialize()
        {
            keys = new Keys[6];
            keys[0] = Keys.Enter;
            keys[1] = Keys.Escape;
            keys[2] = Keys.Left;
            keys[3] = Keys.Right;
            keys[4] = Keys.Up;
            keys[5] = Keys.Down;
            LoadConfig();
            //
            controlsPrevState = new ControlsState(new Point(0, 0), false, false, false, 0, new bool[keys.Length]);
            int resol = fullScreen ? (int)resolution : (int)winResolution;
            graphics.PreferredBackBufferWidth =
                resol <= 1 ? 640 :
                resol == 2 || resol == 3 ? 800 :
                resol == 4 ? 1024 :
                resol == 5 || resol == 6 ? 1280 :
                resol == 7 ? 1366 :
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight =
                resol == 0 ? 400 :
                resol == 1 || resol == 2 ? 480 :
                resol == 3 ? 600 :
                resol == 4 || resol == 6 || resol == 7 ? 768 :
                resol == 5 ? 720 :
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.ApplyChanges();
            //
            //if (fullScreen) ToggleFullScreen();
            ChangeScale();
            graphics.GraphicsDevice.SamplerStates[0] = new SamplerState()
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                Filter = TextureFilter.Anisotropic,
                MaxAnisotropy = 8
            };
            spriteBatch = new SpriteBatch(GraphicsDevice);
            MediaPlayer.IsRepeating = true;
            volumeCoff = 1;
            base.Initialize();
        }


        protected override void Update(GameTime gameTime)
        {
            if (fullScreen != graphics.IsFullScreen) ToggleFullScreen();
            if(IsActive)
            {
                MediaPlayer.Volume = this.soundVolume / 100f * volumeCoff;
                volumeCoff += (volumeChangeSpeed*gameTime.ElapsedGameTime.Milliseconds/1000);
                if(volumeCoff<=0)
                {
                    volumeCoff = 0;
                    if (MediaPlayer.State == MediaState.Playing) MediaPlayer.Stop();
                }
                if (volumeCoff > 1) volumeCoff = 1;
                MouseState mState = Mouse.GetState();
                KeyboardState kState = Keyboard.GetState();
                Vector2 mouseStateCorrected = new Vector2(mState.X - (GraphicsDevice.Viewport.Width - sc * ingameScreenSize.X) / 2,
                    mState.Y - 27 - (GraphicsDevice.Viewport.Height - sc * ingameScreenSize.Y) / 2);
                Vector2 mousePos = Vector2.Transform(
                    new Vector2(mouseStateCorrected.X, mouseStateCorrected.Y), scaleReverse);
                bool[] keysState = new bool[keys.Length];
                for(int i =0; i<keys.Length;i++)
                {
                    keysState[i] = kState.IsKeyDown(keys[i]);
                }
                ControlsState controlsState = new ControlsState(new Point((int)mousePos.X, (int)mousePos.Y),
                    mState.LeftButton == ButtonState.Pressed, mState.MiddleButton == ButtonState.Pressed,
                    mState.RightButton == ButtonState.Pressed, mState.ScrollWheelValue, keysState);
                //
                tempMode.ModeUpdate(gameTime.ElapsedGameTime.Milliseconds);
                if(tempMode.AnimationProgress>=1)
                {
                    tempMode.Update(this, controlsState, controlsPrevState, gameTime.ElapsedGameTime.Milliseconds);
                }
                if(tempMode.AnimationProgress<=0)
                {
                    ChangeModeToTarget();
                }
                //
                controlsPrevState = controlsState;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);
            List<Mode> drawableModes = new List<Mode>
            {
                tempMode
            };
            while (drawableModes[drawableModes.Count-1].Parent!=null)
            {
                drawableModes.Add(drawableModes[drawableModes.Count - 1].Parent);
            }
            tempMode.DrawPreActionsUpdate(this, Color.White);
          /*  Color shadingColor;
            if (targetMode != null && targetMode != tempMode.Parent && targetMode.Parent != tempMode)
            {
                shadingColor = new Color(tempMode.AnimationProgress*0.8f, 
                    tempMode.AnimationProgress*0.8f, tempMode.AnimationProgress*0.8f, 1);
            }
            else shadingColor = new Color(0.8f,0.8f,0.8f,1);*/
            bool noAnimation = false;
            bool[] noAnimations = new bool[drawableModes.Count];
            for (int i = 0; i < noAnimations.Length;i++ )
            {
                if (!noAnimation && (drawableModes[i] == targetMode || drawableModes[0].AnimationDirection))
                {
                    noAnimation = true;
                }
                noAnimations[i] = noAnimation ? noMainAnimation : false;
                if (targetMode!=null && tempMode.Parent == targetMode.Parent && i > 0)
                    noAnimations[i] = true;
            }
            for (int i = drawableModes.Count - 1; i >= 0; i--)
            {
                drawableModes[i].Draw(this, i == 0, Color.White, noAnimations[i],gameTime.ElapsedGameTime.Milliseconds);
            }
            if (misc && fpsFont!=null)
            {
                BeginDrawing(SpriteSortMode.Immediate, null);
                fps++;
                time += gameTime.ElapsedGameTime.Milliseconds;
                if (time > 1000)
                {
                    fpsScreen = fps;
                    fps = 0;
                    time = 0;
                }
                DrawString(fpsFont, fpsScreen.ToString(), true, new Point(ingameScreenSize.X-100, ingameScreenSize.Y-100), 1000, Color.White);
                EndDrawing();
            }
            if (tempMode.CursorEnabled && content[cursorName] != null)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(((Texture2D)content[cursorName]), 
                    new Vector2(Mouse.GetState().Position.X, Mouse.GetState().Position.Y),
                    new Rectangle(0, 0, cursorSize, cursorSize), Color.White,
                    cursorAngle, cursorShift, 0.5f, SpriteEffects.None, 0);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        #region ModesOperations
        void SetTempMode (Mode mode)
        {
            tempMode = mode;
            if (mode.Name == "loadingScreen")
            {
                LoadingAction();
            }
            tempMode.Action?.Invoke(this, mode);
        }

        void ChangeModeToTarget()
        {
            bool b = true;
            Mode checkedMode = tempMode;
            while(checkedMode.Parent!=null)
            {
                b = b && checkedMode.Parent != targetMode;
                checkedMode = checkedMode.Parent;
            }
            if (b)
            {
                targetMode.AnimationDirection = true;
            }
            SetTempMode(targetMode);
            targetMode = null;
        }

        public void GoToLoadingMode (object[] objects, LoadingWheelElement.LoadingMethodDelegate preLoadingMethod, 
            LoadingWheelElement.LoadingMethodDelegate loadingMethod, string targetMode)
        {
            Mode loadingMode = (Mode)modes["loadingScreen"];
            if (loadingMode == null) return;
            LoadingWheelElement wheel = (LoadingWheelElement)loadingMode.Elements[loadingMode.Elements.Length-1];
            if (wheel == null) return;
            wheel.PrepareBeforeMode();
            wheel.Objects = objects;
            wheel.PreLoadingMethod = preLoadingMethod;
            wheel.LoadingMethod = loadingMethod;
            wheel.TargetMode = targetMode;
            GoToMode(loadingMode);
        }

        public void GoToMode (Mode mode)
        {
            List<Mode> firstModes = new List<Mode>();
            Mode checkMode = tempMode;
            while(checkMode!=null)
            {
                firstModes.Add(checkMode);
                checkMode = checkMode.Parent;
            }
            noMainAnimation = false;
            checkMode = mode;
            while(checkMode!=null)
            {
                for(int i = 0; i<firstModes.Count;i++)
                {
                    if(firstModes[i] == checkMode)
                    {
                        noMainAnimation = true;
                        break;
                    }
                }
                if(noMainAnimation)
                {
                    break;
                }
                else
                {
                    checkMode = checkMode.Parent;
                }
            }
            if (mode.Parent == tempMode)
            {
                SetTempMode(mode);
                tempMode.AnimationDirection = true;
            }
            else
            {
                tempMode.AnimationDirection = false;
                targetMode = mode;
            }
        }

        public void GoToMode (string modeName)
        {
            GoToMode((Mode)modes[modeName]);
        }
        #endregion

        #region Config
        protected abstract void LoadConfigFromFile(StreamReader reader);

        protected abstract void LoadStartConfig();

        protected abstract bool SaveConfigCore(StreamWriter writer);

        public bool SaveConfig()
        {
            bool crucial = false;
            try
            {
                StreamWriter write = new StreamWriter(Environment.CurrentDirectory + "\\config.ini", false, Encoding.UTF8);
                crucial = SaveConfigCore(write);
                write.Close();
            }
            catch
            {
                
            }
            return crucial;
        }

        void LoadConfig()
        {
            LoadStartConfig();
            if (Directory.Exists(Environment.CurrentDirectory + "\\langs"))
            {
                FileInfo[] files = new DirectoryInfo(Environment.CurrentDirectory + "\\langs").GetFiles();
                int k = 0;
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Extension == ".mrc")
                    {
                        languages.Add(files[i].Name.Split(new char[] { '.' })[0]);
                        if (files[i].Name == "en.mrc") languageName = languages[k];
                        k++;
                    }
                }
            }
            if (File.Exists(Environment.CurrentDirectory + "//config.ini"))
            {
                StreamReader reader = new StreamReader(Environment.CurrentDirectory + "//config.ini", Encoding.UTF8);
                LoadConfigFromFile(reader);
                reader.Close();
            }
        }
        #endregion

        public void ChangeScale ()
        {
            bool ar;
            if (ar = GraphicsDevice.Viewport.AspectRatio >= (float)ingameScreenSize.X/ingameScreenSize.Y)
            {
                sc = GraphicsDevice.Viewport.Height / (float)ingameScreenSize.Y;
            }
            else
            {
                sc = GraphicsDevice.Viewport.Width / (float)ingameScreenSize.X;
            }
            scale = Matrix.CreateScale(sc, sc, 0) * (ar ?
                Matrix.CreateTranslation((GraphicsDevice.Viewport.Width - sc * ingameScreenSize.X) / 2, 0, 0) :
                Matrix.CreateTranslation(0, (GraphicsDevice.Viewport.Height - sc * ingameScreenSize.Y) / 2, 0));
            scaleReverse = Matrix.CreateScale(1 / sc, 1 / sc, 0);
            Point shift = new Point((int)(GraphicsDevice.Viewport.Width / sc - ingameScreenSize.X) / 2, (int)(GraphicsDevice.Viewport.Height / sc - ingameScreenSize.Y) / 2);
            screenBounds = new Rectangle(-shift.X, -shift.Y, ingameScreenSize.X + shift.X * 2, ingameScreenSize.Y + shift.Y * 2);
        }

        #region ResolutionOperations
        public void ChangeResolution()
        {
            int resol = (fullScreen ? (int)resolution : (int)winResolution)-1;
            graphics.PreferredBackBufferWidth =
                resol <= 0 ? 640 :
                resol == 1 || resol == 2 ? 800 :
                resol == 3 ? 1024 :
                resol == 4 || resol == 5 ? 1280 :
                resol == 6 ? 1366 :
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight =
                resol == -1 ? 400 :
                resol == 0 || resol == 1 ? 480 :
                resol == 2 ? 600 :
                resol == 3 || resol == 5 || resol == 6 ? 768 :
                resol == 4 ? 720 :
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.ApplyChanges();
            ChangeScale();
        }

        public void ToggleFullScreen()
        {
            int resol = graphics.IsFullScreen ? (int)winResolution : (int)resolution;
            graphics.PreferredBackBufferWidth =
                resol <= 1 ? 640 :
                resol == 2 || resol == 3 ? 800 :
                resol == 4 ? 1024 :
                resol == 5 || resol == 6 ? 1280 :
                resol == 7 ? 1366 :
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight =
                resol == 0 ? 400 :
                resol == 1 || resol == 2 ? 480 :
                resol == 3 ? 600 :
                resol == 4 || resol == 6 || resol == 7 ? 768 :
                resol == 5 ? 720 :
                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.ToggleFullScreen();
            ChangeScale();
            fullScreen = graphics.IsFullScreen;
        }
        #endregion

        #region MainDrawMethods

        public void BeginDrawingNoScale(SpriteSortMode sortMode, Effect effect)
        {
            spriteBatch.Begin(sortMode, null, null, null, null, effect);
        }

        public void BeginDrawing(SpriteSortMode sortMode, Effect effect)
        {
            spriteBatch.Begin(sortMode, null, null, null, null, effect, scale);
        }

        public void BeginDrawing(SpriteSortMode sortMode, Effect effect, Matrix matrix)
        {
            spriteBatch.Begin(sortMode, null, null, null, null, effect, scale*matrix);
        }

        public void BeginDrawing(SpriteSortMode sortMode, BlendState state, Effect effect)
        {
            spriteBatch.Begin(sortMode, state, null, null, null, effect, scale);
        }

        public void BeginDrawing(SpriteSortMode sortMode, BlendState state, Effect effect, Matrix matrix)
        {
            spriteBatch.Begin(sortMode, state, null, null, null, effect, scale * matrix);
        }

        public void EndDrawing()
        {
            spriteBatch.End();
        }

        public void DrawBorder (string textureName,float size, Rectangle place, Color color, float depth)
        {
            if (textureName != null)
            {
                Texture2D texture = (Texture2D)content[textureName];
                int width = (int)(place.Width / 2 / size);
                int height = (int)(place.Height / 2 / size);
                Rectangle source = new Rectangle(0, texture.Height - height, width, height);
                spriteBatch.Draw(texture, new Rectangle(place.X, place.Y, place.Width / 2, place.Height / 2),
                    source, color, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);
                spriteBatch.Draw(texture, new Rectangle(place.X, place.Y + place.Height / 2, place.Width / 2, place.Height / 2),
                    source, color, 0, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, new Rectangle(place.X + place.Width, place.Y + place.Height / 2, place.Width / 2, place.Height / 2),
                    source, color, MathHelper.Pi, Vector2.Zero, SpriteEffects.None, 0);
                spriteBatch.Draw(texture, new Rectangle(place.X + place.Width / 2, place.Y + place.Height / 2, place.Width / 2, place.Height / 2),
                    source, color, 0, Vector2.Zero, SpriteEffects.FlipHorizontally, 0);
            }
        }

        public void DrawSprite(string textureName, Rectangle place, Color color,
            float angle, SpriteEffects effects, float depth)
        {
            if (textureName != null)
            {
                Texture2D texture = (Texture2D)content[textureName];
                if (texture != null)
                {
                    spriteBatch.Draw(texture, place, new Rectangle(0, 0, texture.Width, texture.Height),
                        color, angle, new Vector2(texture.Width / 2, texture.Height / 2), effects, depth);
                }
            }
        }

        public void DrawSprite(string textureName, Rectangle place, Rectangle area, Color color, 
            float angle, Vector2 origin, SpriteEffects effects, float depth)
        {
            if (textureName != null && (Texture2D)content[textureName] != null)
            {
                spriteBatch.Draw((Texture2D)content[textureName], place, area, color, angle, origin, effects, depth);
            }
        }

        public void DrawSprite(Texture2D texture, Rectangle place, Color color,
            float angle, SpriteEffects effects, float depth)
        {
            if (texture != null)
            {
                spriteBatch.Draw(texture, place, new Rectangle(0, 0, texture.Width, texture.Height),
                    color, angle, new Vector2(texture.Width / 2, texture.Height / 2), effects, depth);
            }
        }

        public void DrawSprite(Texture2D texture, Rectangle place, Rectangle area, Color color,
            float angle, Vector2 origin, SpriteEffects effects, float depth)
        {
            if (texture != null)
            {
                spriteBatch.Draw(texture, place, area, color
                    , angle, origin, effects, depth);
            }
        }

        public void DrawSprite (Sprite sprite, int x, int y, float size, float angle, float depth, float alpha)
        {
	    Color color = sprite.Color;
            if(sprite.Info.Objects!=null)
            {
                Directions direction;
                if(angle>=0 && angle<MathHelper.PiOver2)
                {
                    direction = Directions.Right;
                }
                else if(angle>=MathHelper.PiOver2 && angle<MathHelper.Pi)
                {
                    direction = Directions.Up;
                }
                else if(angle<0 && angle>=-MathHelper.PiOver2)
                {
                    direction = Directions.Down;
                }
                else direction = Directions.Left;
                float spriteAngle;
                switch(direction)
                {
                    case Directions.Right:
                        spriteAngle = angle;
                        break;
                    case Directions.Up:
                        spriteAngle = MathHelper.Pi - angle;
                        break;
                    case Directions.Down:
                        spriteAngle = angle;
                        break;
                    default:
                        if(angle>0)
                        {
                            spriteAngle = MathHelper.PiOver2 - angle;
                        }
                        else
                        {
                            spriteAngle = MathHelper.Pi - angle;
                        }
                        break;
                }
                //spriteAngle = 0;
                bool mirror = (direction == Directions.Left || direction == Directions.Up);
                bool side = (direction == Directions.Right || direction == Directions.Up);
                for(int i = 0; i<sprite.Info.Objects.Length;i++)
                {
                    AnimationObject part = sprite.Info.Objects[i];
                    FrameInfo frame = part.Animations[(int)sprite.Animation].Frames[(int)sprite.Frame];
                    if (frame.Hiden)
                    {
                        PositionParametres parametres = part.RotationImpact ?
                            sprite.Info.GetPresolvedParametres((int)sprite.Animation, (int)sprite.Frame, i, sprite.Info.Objects,
                            spriteAngle, out Matrix matrix)
                            : frame.PresolvedParametres;
                        //
                        Texture2D texture = (Texture2D)content[part.TextureName];
                        spriteBatch.Draw(texture, parametres.Translation * new Vector2(mirror ? -size : size, size) + new Vector2(x, y),
                            new Rectangle(0, 0, texture.Width, texture.Height),
                            part.Colorisation ? new Color((byte)(color.R*alpha),(byte)(color.G*alpha),(byte)(color.B*alpha),(byte)(alpha*255))
                            : new Color(alpha,alpha,alpha,alpha), 
                            mirror ? -parametres.Rotation : parametres.Rotation,
                            new Vector2(texture.Width / 2, texture.Height / 2), parametres.Scale * size,
                            mirror ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                            depth - (side ? part.Depth : part.BackDepth) / 100);
                    }
                }
            }
            else
            {
                Texture2D texture;
                if ((texture = (Texture2D)content[sprite.Info.Name]) != null)
                {
                    int width = texture.Width;
                    int height = texture.Height;
                    spriteBatch.Draw(texture, new Vector2(x, y), new Rectangle(0, 0, width, height),
                     new Color((byte)(color.R * alpha), (byte)(color.G * alpha), (byte)(color.B * alpha), (byte)(alpha * 255))
                     , angle, new Vector2(width / 2, height/2),
                    size, SpriteEffects.None, depth);
                }
            }
        }

        public void DrawString(string strFont, string text, bool align, Point pos, int width, Color color)
        {
            SpriteFont font = strFont!=null?(SpriteFont)content[strFont]:null;
            if (font != null)
            {
                string[] strings = text.Split(new char[] { ' ' });
                List<string> newStrings = new List<string>();
                int height = (int)(font.MeasureString(text).Y);
                string temp = "";
                foreach (string str in strings)
                {
                    if ((font.MeasureString(temp + str)).X > width)//(temp.Length+str.Length+1>(rect.Width/(float)(size)))
                    {
                        newStrings.Add(temp);
                        temp = str;
                    }
                    else
                    {
                        temp += " " + str;
                    }
                }
                if (temp != "") newStrings.Add(temp);
                for (int i = 0; i < newStrings.Count; i++)
                {
                    if (align)
                    {
                        spriteBatch.DrawString(font, newStrings[i], new Vector2(pos.X, pos.Y + (int)(i * height)), color, 0, Vector2.Zero,
                            1, SpriteEffects.None, 1);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, newStrings[i],
                            new Vector2(pos.X + (width - font.MeasureString(newStrings[i]).X) / 2
                            , pos.Y + (int)(i * height)), color, 0, Vector2.Zero,
                            1, SpriteEffects.None, 1);
                    }
                }
            }
        }

        public void PlaySound (string soundName, float volume, float angle)
        {
            SoundEffect sound = (SoundEffect)sounds[soundName];
            if (sound != null)
            {
                SoundEffectInstance inst = sound.CreateInstance();
                inst.Volume = volume * (this.volume / 100f);
                inst.Pan = (float)Math.Cos(angle);
                inst.Play();
                inst.IsLooped = false;
            }
        }
        #endregion

        public Mode GetTempMode()
        {
            return tempMode;
        }

        public void PlaySong(string name)
        {
            volumeChangeSpeed = 0;
            volumeCoff = 1;
            Song song = (Song)sounds[name];
            if (song != null)
            {
                if (MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong != song)
                {
                    MediaPlayer.Play(song);
                }
            }
        }

        public void StopSong(bool regress)
        {
            if (regress)
            {
                volumeChangeSpeed = -2f;
            }
            else
            {
                MediaPlayer.Stop();
            }
        }
    }
}
