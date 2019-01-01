using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Ignitus;
using System.IO;
using System;
using System.Text;

namespace GameMaker
{

    public class Game1Shell : IgnitusGame
    {
        Color c_color = new Color(200, 200, 200, 200);
        Color c_selected_color = new Color(240, 240, 240, 255);
        Color c_pressed_color = new Color(120, 120, 120, 255);

        public delegate string SetProfileString(GameElementShell gameElement);
        public delegate void GetProfileString(GameElementShell gameElement, string str);

        SetProfileString methodForSaveProfile;
        GetProfileString methodForLoadProfile;
        int loadingTime;
        string[] texturesOnStart;
        HudElement[] loadingScreenElements;
        HudElement[] mainElements;
        GameElementShell gameElement;

        int score;
        int maxScore;
        string profileFilePath;

        public GameElementShell GameElement { get { return gameElement; } }


        public Game1Shell(int loadingTime, string[] texturesOnStart, HudElement[] loadingScreenElements, HudElement[] mainElements,
            string profileFilePath,
            SetProfileString methodForSaveProfile, GetProfileString methodForLoadProfile, GameElementShell gameElement)
            :base(new Vector2(-16, -16), 32, MathHelper.Pi + MathHelper.PiOver4, new Point(2560, 1600))
        {
            this.gameElement = gameElement;
            this.loadingTime = loadingTime;
            this.texturesOnStart = texturesOnStart;
            this.loadingScreenElements = loadingScreenElements;
            this.mainElements = mainElements;
            this.methodForSaveProfile = methodForSaveProfile;
            this.methodForLoadProfile = methodForLoadProfile;
            this.profileFilePath = profileFilePath;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
            HudElement[] tempLoadingElements;
            if (loadingScreenElements != null)
            {
                tempLoadingElements = new HudElement[loadingScreenElements.Length + 1];
                for (int i = 0; i < loadingScreenElements.Length; i++)
                {
                    tempLoadingElements[i] = loadingScreenElements[i];
                }
            }
            else
            {
                tempLoadingElements = new HudElement[1];
            }
            tempLoadingElements[tempLoadingElements.Length - 1] = 
                new LoadingWheelElement("loading", 2400, 1460, 200, 200, "loadingWheel", loadingTime, true, Color.White, 1,
                    Game1Shell.PreLoadingMethodBeforeStart, Game1Shell.PreLoadingMethodBeforeStart, false, false);
            modes.Add("loadingScreen", new Mode(null, tempLoadingElements, 5, "loadingScreen", Mode.BlackGlow, null, false));
            GoToLoadingMode(new object[] { this }, PreLoadingMethodBeforeStart, LoadingMethodBeforeStart, "main");
            //fullScreen = true;
            base.Initialize();
            keys = new Keys[7];
            keys[0] = Keys.Enter;
            keys[1] = Keys.Escape;
            keys[2] = Keys.Left;
            keys[3] = Keys.Right;
            keys[4] = Keys.Up;
            keys[5] = Keys.Down;
            keys[6] = Keys.F4;
        }


        protected override void Update(GameTime gameTime)
        {
            
            if(controlsPrevState.KeysState.Length>6 && controlsPrevState.KeysState[6] && !Keyboard.GetState().IsKeyDown(keys[6]))
            {
                ToggleFullScreen();
                SaveConfig();
                gameElement.PickSampleFon();
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected override void LoadConfigFromFile(StreamReader reader)
        {
            if (reader.EndOfStream)
            {
                return;
            }
            string[] line = reader.ReadLine().Split(new char[] { '=' });
            if (line.Length > 1 && int.TryParse(line[1], out int result))
            {
                resolution = (Resolution)result;
            }
            if (reader.EndOfStream)
            {
                return;
            }
            line = reader.ReadLine().Split(new char[] { '=' });
            if (line.Length > 1 && int.TryParse(line[1], out result))
            {
                winResolution = (Resolution)result;
            }
            if (reader.EndOfStream)
            {
                return;
            }
            line = reader.ReadLine().Split(new char[] { '=' });
            if (line.Length > 1 && int.TryParse(line[1], out result))
            {
                fullScreen = result == 1;
            }
            if (reader.EndOfStream)
            {
                return;
            }
            line = reader.ReadLine().Split(new char[] { '=' });
            if (line.Length > 1 && int.TryParse(line[1], out result))
            {
                volume = result;
                soundVolume = result;
            }
            if (reader.EndOfStream)
            {
                return;
            }
            line = reader.ReadLine().Split(new char[] { '=' });
            if (line.Length > 1)
            {
                languageName = line[1];
            }
        }

        protected override HudElement LoadModeElementParser(string type, string name, string[] parametres)
        {
            return base.LoadModeElementParser(type, name, parametres);
        }

        protected override bool SaveConfigCore(StreamWriter writer)
        {
            writer.WriteLine("resolution=" + ((int)resolution).ToString());
            writer.WriteLine("winResolution=" + ((int)winResolution).ToString());
            writer.WriteLine("fullscreen=" + (fullScreen?"1":"0"));
            writer.WriteLine("volume=" + volume.ToString());
            writer.WriteLine("language=" + languageName);
            return true;
        }

        protected override void LoadStartConfig()
        {
            winResolution = Resolution.R800x480;
            resolution = Resolution.RYours;
            volume = 50;
            soundVolume = 50;
            languageName = "eng";
            fullScreen = true;
        }

        protected override object LoadNativeParser(string type, string name, string[] parametres)
        {
            return null;
        }

        protected override void LoadProfile()
        {
            HudElement[] elements = new HudElement[gameElement != null ? 2 : 1];
            elements[0] = gameElement;
            elements[gameElement == null ? 0 : 1] = new GameButtonElement("pause", 50, 50, 150, 150, c_color, c_selected_color, c_pressed_color,
                "pause_button", null, null, new Rectangle(0, 0, 512, 512), MenuActions.PauseGame, gameElement);
            modes.Add("game_mode", new Mode(null, elements, 5, "game", Mode.BlackGlow, null, false));
            if (mainElements != null)
            {
                elements = new HudElement[mainElements.Length + 7];
                for (int i = 0; i < mainElements.Length; i++)
                {
                    elements[i] = mainElements[i];
                }
            }
            else
            {
                elements = new HudElement[7];
            }
            //DownloadAdditiveElements
            elements[elements.Length - 1] = new SpriteButtonElement("play", ingameScreenSize.X / 2 - 200, ingameScreenSize.Y / 2, 400, 400, "", "", c_color,
                c_selected_color,c_pressed_color,c_color, "play_button", null, null, new Rectangle(0, 0, 512, 512),
                MenuActions.StartGame, false, false);
            elements[elements.Length - 2] = new SpriteButtonElement("exit", 50, 50, 150, 150, null, null, c_color,
                c_selected_color, c_pressed_color, c_color, "exit_button", null, null, new Rectangle(0, 0, 512, 512),
                MenuActions.Exit, false, false);
            elements[elements.Length - 3] = new SlideElement("soundReg", 550, 75, 400, 50, "slider", c_color, c_selected_color, c_pressed_color,
                MenuActions.SoundVolume, 130, false, true, 0.13f, false, false);
            ((SlideElement)elements[elements.Length - 3]).Position = volume / 100f;
            elements[elements.Length - 4] = new LabelElement("fullscreen", 205, 1450, ingameScreenSize.X - 400, "fullscreen_message", true, false, c_color, "smallFont", false, false);
            elements[elements.Length - 5] = new LabelElement("score", 195, 570, ingameScreenSize.X - 400, Id2Str("score") + " " + score.ToString(), false, false, c_color, "mediumFont", false, false);
            elements[elements.Length - 6] = new LabelElement("max_score", 195, 670, ingameScreenSize.X - 400, Id2Str("top_result") + " " + maxScore.ToString(), false, false, c_color, "mediumFont", false, false);
            elements[elements.Length - 7] = new LabelElement("volume", 320, 80, 1000, "volume", true, true, c_color, "smallFont", false, false);
            //
            modes.Add("main", new Mode((Mode)modes["game_mode"], elements, 3.5f, "main", FromAbove, null, false));

            modes.Add("game_mode_context", new Mode((Mode)modes["game_mode"], new HudElement[]
            {
                new ButtonElement("continue",600,650,ingameScreenSize.X-1200,100,Id2Str("continue"),"largeFont",false,
                c_color,c_selected_color,c_pressed_color,MenuActions.ContinueGame,false,false),
                new ButtonElement("end",600,850,ingameScreenSize.X-1200,100,Id2Str("end"),"largeFont",false,
                c_color,c_selected_color,c_pressed_color,MenuActions.EndGame,false,false)
            }, 3.5f, "context", FromAbove, null, false));

            modes.Add("game_mode_result", new Mode((Mode)modes["game_mode"], new HudElement[]
            {
                new LabelElement("score", 20, 570, ingameScreenSize.X - 400, Id2Str("score") + " " + score.ToString(), false, false, c_color, "largeFont", false, false),
                new LabelElement("top_score", 200, 770, ingameScreenSize.X - 400, Id2Str("top_result") + " " + maxScore.ToString(), false, false, c_color, "largeFont", false, false),
                new LabelElement("any_key", 200, 1400, ingameScreenSize.X - 400, "any_key_message", true, false, c_color, "mediumFont", false, false),
                new AnyKeyElement("result",MenuActions.EndGame)
            }, 3.5f, "result", FromAbove, null, false));
            //LoadFile
            if(File.Exists(profileFilePath+@"\profile.mrc"))
            {
                byte[] bytes = Magic.Restore(profileFilePath + @"\profile.mrc");
                string str = Encoding.UTF8.GetString(bytes);
                string[] strs = str.Split(new char[] { '\n' });
                string masterString = strs[0];
                //Master
                if (int.TryParse(masterString, out int result))
                {
                    maxScore = result;
                }
                string otherString = str.Remove(0, masterString.Length + 1);
                //Other
                methodForLoadProfile?.Invoke(gameElement, otherString);
            }
        }

        protected override void SaveProfile()
        {
            //SaveFile
            string str = maxScore.ToString() + "\n" + (methodForSaveProfile!=null?methodForSaveProfile(gameElement):"");
            Magic.Act(profileFilePath + @"\profile.mrc",Encoding.UTF8.GetBytes(str));
        }

        public bool SetScore (int score)
        {
            this.score = score;
            if(maxScore<score)
            {
                maxScore = score;
            }
            Mode mode = (Mode)modes["main"];
            ((LabelElement)mode.Elements[mode.Elements.Length - 5]).Text = Id2Str("score") + " " + score.ToString();
            ((LabelElement)mode.Elements[mode.Elements.Length - 6]).Text = Id2Str("top_result") + " " + maxScore.ToString();
            mode = (Mode)modes["game_mode_result"];
            ((LabelElement)mode.Elements[0]).Text = Id2Str("score") + " " + score.ToString();
            ((LabelElement)mode.Elements[1]).Text = Id2Str("top_result") + " " + maxScore.ToString();
            return maxScore < score;
        }

        public void LoadEngineContent()
        {
            content.Add("play_button", Content.Load<Texture2D>("menu//Play_button"));
            content.Add("exit_button", Content.Load<Texture2D>("menu//Exit_button"));
            content.Add("pause_button", Content.Load<Texture2D>("menu//Pause_button"));
            content.Add("slider", Content.Load<Texture2D>("menu//Slider"));
            content.Add("slider_slide", Content.Load<Texture2D>("menu//Slider_slide"));
            content.Add("slider_arrow", Content.Load<Texture2D>("menu//Slider_arrow"));
        }

        #region loading
        void LoadLoadingContent()
        {
            if (File.Exists(Environment.CurrentDirectory + "\\Content\\" + "NewSpriteFont" + ".xnb"))
                content.Add("largeFont", Content.Load<SpriteFont>("NewSpriteFont"));
            if (File.Exists(Environment.CurrentDirectory + "\\Content\\" + "lesser" + ".xnb"))
                content.Add("mediumFont", Content.Load<SpriteFont>("lesser"));
            if (File.Exists(Environment.CurrentDirectory + "\\Content\\" + "small" + ".xnb"))
                content.Add("smallFont", Content.Load<SpriteFont>("small"));
            if (File.Exists(Environment.CurrentDirectory + "\\Content\\" + "smallest" + ".xnb"))
                content.Add("systemFont", Content.Load<SpriteFont>("smallest"));
            for (int i = 0; i < texturesLoadingList.Count; i++)
            {
                if (File.Exists(Environment.CurrentDirectory + "\\Content\\"+texturesOnStart[i]+".xnb"))
                {
                    content.Add(texturesOnStart[i], Content.Load<Texture2D>("loadInfo\\" + texturesOnStart[i]));
                }
            }
            if (File.Exists(Environment.CurrentDirectory + "\\Content\\main_cursor.xnb"))
            {
                content.Add("cursor", Content.Load<Texture2D>("main_cursor"));
            }
        }

        public static void PreLoadingMethodBeforeStart(object[] objs)
        {
            Game1Shell game = (Game1Shell)objs[0];
            game.Content.Unload();
            game.content.Clear();
            game.LoadLoadingContent();
        }

        public static void LoadingMethodBeforeStart(object[] objs)
        {
            Game1Shell game = (Game1Shell)objs[0];
            game.LoadEngineContent();
            game.LoadMainInformation();
            game.LoadMainContent();
            game.GameElement.SetFon((Texture2D)game.ContentTex[game.GameElement.FonName]);
        }
        #endregion

        public static MatrixColorCombo FromAbove(IgnitusGame game, float animationProgress, Color color)
        {
            return new MatrixColorCombo(Matrix.CreateTranslation(0, -game.GraphicsDevice.Viewport.Height* (1-animationProgress), 0), color);
        }

        public void EndGameActivities ()
        {
            SaveProfile();
        }
    }
}
