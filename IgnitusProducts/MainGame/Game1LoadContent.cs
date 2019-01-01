using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ignitus
{

    public struct TextureToLoad
    {
        string path;
        string name;
        string mode;

        public string Path { get { return path; } }
        public string Name { get { return name; } }
        public string Mode { get { return mode; } }

        public TextureToLoad (string name, string path, string mode)
        {
            this.name=name;
            this.path=path;
            this.mode=mode;
        }
    }

    public abstract partial class IgnitusGame : Game
    {
        protected List<string> languages = new List<string>();
        protected List<TextureToLoad> texturesLoadingList = new List<TextureToLoad>();
        protected Hashtable content = new Hashtable();
        protected Hashtable natives = new Hashtable();
        protected Hashtable strings = new Hashtable();
        protected Hashtable modes = new Hashtable();
        protected Hashtable sprites = new Hashtable();
        protected Hashtable sounds = new Hashtable();
        public Hashtable ContentTex { get { return content; } }
        public Hashtable Natives { get { return natives; } }
        public Hashtable Strings { get { return strings; } }
        public Hashtable Modes { get { return modes; } }
        public Hashtable Sprites { get { return sprites; } }
        public Hashtable Sounds { get { return sounds; } }

        //private Thread loadingThread;

        //Thread LoadingThread { get { return loadingThread; } set { loadingThread = value; } }

        //Methods
        protected void LoadSounds()
        {
            string filePath = Environment.CurrentDirectory + @"\data\sounds.mrc";
            if (File.Exists(filePath))
            {
                this.sounds.Clear();
                byte[] bytes = Magic.Restore(filePath);
                string info = Encoding.UTF8.GetString(bytes);
                string[] textures = info.Split(new char[] { '\n' });
                for (int i = 0; i < textures.Length; i++)
                {
                    string[] parts = textures[i].Split(new char[] { ';' });
                    if(parts[0] == "sound")
                    {
                        sounds.Add(parts[1], Content.Load<Song>(parts[2]));
                    }
                    else
                    {
                        sounds.Add(parts[1],Content.Load<SoundEffect>(parts[2]));
                    }
                }
            }
        }

        protected void LoadTextures()
        {
            string filePath = Environment.CurrentDirectory + @"\data\textures.mrc";
            if (File.Exists(filePath))
            {
                byte[] bytes = Magic.Restore(filePath);
                string info = Encoding.UTF8.GetString(bytes);
                string[] textures = info.Split(new char[] { '\n' });
                for(int i = 0; i<textures.Length;i++)
                {
                    string[] parts = textures[i].Split(new char[] { ';' });
                    if (parts[2] == "font")
                    {
                        content.Add(parts[0], Content.Load<SpriteFont>(parts[1]));
                    }
                    else
                    {
                        texturesLoadingList.Add(new TextureToLoad(parts[0], parts[1], parts[2]));
                    }
                }
            }
        }

        protected void LoadSprites()
        {
            string filePath = Environment.CurrentDirectory + @"\data\sprites.mrc";
            if (File.Exists(filePath))
            {
                this.sprites.Clear();
                byte[] bytes = Magic.Restore(filePath);
                string info = Encoding.UTF8.GetString(bytes);
                string[] sprites = info.Split(new char[] { '\n' });
                for(int  i =0 ; i<sprites.Length;i++)
                {
                    string[] parts = sprites[i].Split(new char[]{';'});
                    if(parts.Length>1)
                    {
                        this.sprites.Add(parts[0], new SpriteInfo(parts[0], Environment.CurrentDirectory + @"\Content\" + parts[1].Split(new char[] { '\r', '\0' })[0] + ".mrc"));
                    }
                    else
                    {
                        this.sprites.Add(parts[0].Split(new char[] { '\r', '\0' })[0], new SpriteInfo(parts[0].Split(new char[] { '\r', '\0' })[0]));
                    }
                }
            }
        }

        protected void LoadNatives()
        {
            string filePath = Environment.CurrentDirectory + @"\data\natives.mrc";
            if (File.Exists(filePath))
            {
                this.natives.Clear();
                byte[] bytes = Magic.Restore(filePath);
                string info = Encoding.UTF8.GetString(bytes);
                string[] natives = info.Split(new char[] { '\n' });
                for (int i = 0; i < natives.Length; i++)
                {
                    string[] parts = natives[i].Split(new char[] { ';' });
                    string[] parametres = new string[parts.Length - 2];
                    parts.CopyTo(parametres, 2);
                    this.natives.Add(parts[1],LoadNativeParser(parts[0], parts[1], parametres));
                }
            }
        }

        protected Mode LoadMode (string strings)
        {
            Type main = Type.GetType("Ignitus.MenuActions");
            string[] parts = strings.Split(new char[] { ';' });
            HudElement[] hudElements = new HudElement[parts.Length - 7];
            for (int j = 0; j < hudElements.Length; j++)
            {
                string[] element = parts[j + 6].Split(new char[] { '@' });
                string[] parametres = new string[element.Length - 2];
                element.CopyTo(parametres, 2);
                hudElements[j] = LoadModeElementParser(element[0], element[1], parametres);
            }
            MethodInfo info1 = main.GetMethod(parts[4]);
            return new Mode((Mode)this.modes[parts[1]], hudElements, float.Parse(parts[2]), parts[0],
                (Mode.GetAnimationTransformation)Mode.GetAnimationTransformation.CreateDelegate(main, main.GetMethod(parts[3])),
                (Mode.ActionOnOpening)Mode.ActionOnOpening.CreateDelegate(main, main.GetMethod(parts[4])),
                parts[5] == "1");
        }

        protected void LoadModes()
        {
            string filePath = Environment.CurrentDirectory + @"\data\modes.mrc";
            if (File.Exists(filePath))
            {
                this.modes.Clear();
                byte[] bytes = Magic.Restore(filePath);
                string info = Encoding.UTF8.GetString(bytes);
                string[] modes = info.Split(new char[] { '\n' });
                for (int i = 0; i < modes.Length; i++)
                {
                    string[] parts = modes[i].Split(new char[] { ';' });
                    this.modes.Add(parts[0], LoadMode(modes[i]));
                }
            }
        }

        protected abstract void LoadProfile();

        protected abstract void SaveProfile();

        protected abstract object LoadNativeParser(string type, string name, string[] parametres);

        protected virtual HudElement LoadModeElementParser(string type, string name, string[] parametres)
        {
            HudElement neededElement = null;
            Type main = Type.GetType("Ignitus.MenuActions");
            switch(type)
            {
                case "AnyKeyElement":
                    neededElement = new AnyKeyElement(name,
                        (PressButtonAction)PressButtonAction.CreateDelegate(main, main.GetMethod(parametres[0])));
                    break;
                case "BorderElement":
                    neededElement = new BorderElement(name,int.Parse(parametres[0]),int.Parse(parametres[1]),int.Parse(parametres[2]),
                    int.Parse(parametres[3]),parametres[4],
                    new Color(int.Parse(parametres[5]),int.Parse(parametres[6]),int.Parse(parametres[7]),int.Parse(parametres[8])),
                    float.Parse(parametres[9]),parametres[10]=="1",parametres[11]=="1");
                    break;
                case "ButtonElement":
                    neededElement = new ButtonElement(name, int.Parse(parametres[0]), int.Parse(parametres[1]), int.Parse(parametres[2]),
                        int.Parse(parametres[3]), parametres[4], parametres[5], parametres[6] == "1",
                        new Color(int.Parse(parametres[7]), int.Parse(parametres[8]), int.Parse(parametres[9]), int.Parse(parametres[10])),
                        new Color(int.Parse(parametres[11]), int.Parse(parametres[12]), int.Parse(parametres[13]), int.Parse(parametres[14])),
                        new Color(int.Parse(parametres[15]), int.Parse(parametres[16]), int.Parse(parametres[17]), int.Parse(parametres[18])),
                        (PressButtonAction)PressButtonAction.CreateDelegate(main, main.GetMethod(parametres[19])), parametres[20] == "1", parametres[21] == "1");
                    break;
                case "CheckBoxElement":
                    neededElement = new CheckBoxElement(name, int.Parse(parametres[0]), int.Parse(parametres[1]), int.Parse(parametres[2]),
                        int.Parse(parametres[3]), parametres[4], parametres[5], parametres[6] == "1", parametres[7],
                        new Color(int.Parse(parametres[8]), int.Parse(parametres[9]), int.Parse(parametres[10]), int.Parse(parametres[11])),
                        new Color(int.Parse(parametres[12]), int.Parse(parametres[13]), int.Parse(parametres[14]), int.Parse(parametres[15])),
                        new Color(int.Parse(parametres[16]), int.Parse(parametres[17]), int.Parse(parametres[18]), int.Parse(parametres[19])),
                        parametres[20] == "1", parametres[21] == "1", (PressButtonAction)PressButtonAction.CreateDelegate(main, main.GetMethod(parametres[22])),
                        parametres[23] == "1", parametres[24] == "1");
                    break;
                case "ContextMenuElement":
                    neededElement = new ContextMenuElement(parametres[0]);
                    break;
                case "LabelElement":
                    neededElement = new LabelElement(name, int.Parse(parametres[0]), int.Parse(parametres[1]), int.Parse(parametres[2]),
                        parametres[3], parametres[4] == "1", parametres[5] == "1",
                        new Color(int.Parse(parametres[6]), int.Parse(parametres[7]), int.Parse(parametres[8]), int.Parse(parametres[9])),
                        parametres[10], parametres[11] == "1", parametres[12] == "1");
                    break;
                case "LoadingWheelElement":
                    neededElement = new LoadingWheelElement(name, int.Parse(parametres[0]),int.Parse(parametres[1]),int.Parse(parametres[2]),
                    int.Parse(parametres[3]),parametres[4],
                    new Color(int.Parse(parametres[5]),int.Parse(parametres[6]),int.Parse(parametres[7]),int.Parse(parametres[8])),
                    float.Parse(parametres[9]),parametres[10]=="1",parametres[11]=="1");
                    break;
                case "SlideElement":
                    neededElement = new SlideElement(name, int.Parse(parametres[0]), int.Parse(parametres[1]), int.Parse(parametres[2]),
                        int.Parse(parametres[3]), parametres[4],
                        new Color(int.Parse(parametres[5]), int.Parse(parametres[6]), int.Parse(parametres[7]), int.Parse(parametres[8])),
                        new Color(int.Parse(parametres[9]), int.Parse(parametres[10]), int.Parse(parametres[11]), int.Parse(parametres[12])),
                        new Color(int.Parse(parametres[13]), int.Parse(parametres[14]), int.Parse(parametres[15]), int.Parse(parametres[16])),
                        (PressButtonAction)PressButtonAction.CreateDelegate(main, main.GetMethod(parametres[17])), int.Parse(parametres[18]),
                        parametres[19] == "1", parametres[20] == "1", float.Parse(parametres[21]), parametres[22] == "1", parametres[23] == "1");
                    break;
                case "SpriteButtonElement":
                    neededElement = new SpriteButtonElement(name, int.Parse(parametres[0]), int.Parse(parametres[1]), int.Parse(parametres[2]),
                        int.Parse(parametres[3]), parametres[4], parametres[5],
                        new Color(int.Parse(parametres[6]), int.Parse(parametres[7]), int.Parse(parametres[8]), int.Parse(parametres[9])),
                        new Color(int.Parse(parametres[10]), int.Parse(parametres[11]), int.Parse(parametres[12]), int.Parse(parametres[13])),
                        new Color(int.Parse(parametres[14]), int.Parse(parametres[15]), int.Parse(parametres[16]), int.Parse(parametres[17])),
                        new Color(int.Parse(parametres[18]), int.Parse(parametres[19]), int.Parse(parametres[20]), int.Parse(parametres[21])),
                        parametres[22],parametres[23]=="null"?null:parametres[23],parametres[24]=="null"?null:parametres[24],
                        new Rectangle(int.Parse(parametres[25]),int.Parse(parametres[26]),int.Parse(parametres[27]),int.Parse(parametres[28])),
                        (PressButtonAction)PressButtonAction.CreateDelegate(main, main.GetMethod(parametres[29])), parametres[30] == "1", parametres[31] == "1");
                    break;
                case "SpriteElement":
                    neededElement = new SpriteElement(name, int.Parse(parametres[0]), int.Parse(parametres[1]), int.Parse(parametres[2]),
                        int.Parse(parametres[3]), parametres[4],
                        new Color(int.Parse(parametres[5]), int.Parse(parametres[6]), int.Parse(parametres[7]), int.Parse(parametres[8])),
                        new Rectangle(int.Parse(parametres[9]),int.Parse(parametres[10]),int.Parse(parametres[11]),int.Parse(parametres[12])),
                        parametres[13] == "1", parametres[14] == "1");
                    break;
                case "SpriteElementFull":
                    neededElement = new SpriteElement(name, int.Parse(parametres[0]), int.Parse(parametres[1]), int.Parse(parametres[2]),
                        int.Parse(parametres[3]), parametres[4],
                        new Color(int.Parse(parametres[5]), int.Parse(parametres[6]), int.Parse(parametres[7]), int.Parse(parametres[8])),
                        new Rectangle(int.Parse(parametres[9]), int.Parse(parametres[10]), int.Parse(parametres[11]), int.Parse(parametres[12])),
                        float.Parse(parametres[13]),new Vector2(float.Parse(parametres[14]),float.Parse(parametres[15])),
                        parametres[16]=="1"?SpriteEffects.FlipVertically:parametres[16]=="2"?SpriteEffects.FlipHorizontally:SpriteEffects.None,
                        parametres[17] == "1", parametres[18] == "1");
                    break;
                case "TimerElement":
                    neededElement = new TimerElement(name, (PressButtonAction)PressButtonAction.CreateDelegate(main, main.GetMethod(parametres[0])),
                        float.Parse(parametres[1]), parametres[2] == "1", parametres[3]=="1");
                    break;
            }
            return neededElement;
        }

        protected void LoadLanguages()
        {
            #region Languages
            if (File.Exists(Environment.CurrentDirectory + "\\langs\\" + languageName + ".mrc"))
            {
                strings.Clear();
                byte[] bytesLangs = Magic.Restore(Environment.CurrentDirectory + "\\langs\\" + languageName + ".mrc");
                string langs = Encoding.UTF8.GetString(bytesLangs);
                string[] phrases = langs.Split(new char[] { '\n' });
                for (int i = 0; i < phrases.Length; i++)
                {
                    string[] content = phrases[i].Split(new char[] { '@' });
                    if (content.Length >= 2)
                    {
                        strings.Add(content[0], content[1]);
                    }
                }
            }
            #endregion
        }

        protected void LoadMainInformation()
        {
            LoadProfile();
            LoadModes();
        }

        protected void LoadMainContent()
        {
            LoadLanguages();
            LoadTextures();
            LoadSprites();
            LoadNatives();
            LoadSounds();
        }

        public string Id2Str(string id)
        {
            return strings[id] != null ? (string)strings[id] : id;
        }

        public int CalculateStringLength(string s, string font)
        {
            SpriteFont spriteFont = (SpriteFont)content[font];
            if(spriteFont!=null)
            {
                return (int)(spriteFont.MeasureString(s).X) + 8;
            }
            else
            {
                return -1;
            }
        }

        public int CalculateStringHeight(string s, string font)
        {
            if (font != null)
            {
                SpriteFont spriteFont = (SpriteFont)content[font];
                if (spriteFont != null)
                {
                    return (int)(spriteFont.MeasureString(s).Y);
                }
                else
                {
                    return -1;
                }
            }
            return -1;
        }

        public void LoadTexturePack (string name)
        {
            for (int i = 0; i < texturesLoadingList.Count; i++)
            {
                if (texturesLoadingList[i].Mode.Split(new char[] { '\r' })[0].Split(new char[] { '\0' })[0] == name)
                {
                    LoadTexture(texturesLoadingList[i]);
                }
            }
        }

        void LoadTexture (TextureToLoad texture)
        {
            if (File.Exists(Environment.CurrentDirectory + "\\Content\\" + texture.Path + ".xnb"))
            {
                string textureName = texture.Name;
                string texturePath = texture.Path;
              /*  if(textureName == "grass" && (DateTime.Now.Month == 1 && DateTime.Now.Day==1))
                {
                    texturePath = "tiles\\snow";
                }*/
                bool b = false;
                foreach (string str in content.Keys)
                {
                    if (str == textureName)
                    {
                        b = true;
                        break;
                    }
                }
                if(!b) content.Add(textureName, Content.Load<Texture2D>(texturePath));
            }
        }

        void AsynchronicLoadingAction(object obj)
        {
            LoadingWheelElement wheel = (LoadingWheelElement)obj;
            wheel.LoadingMethod(wheel.Objects);
            wheel.LoadCompleted();
            //GoToMode(wheel.TargetMode);
        }

        void LoadingAction ()
        {
            LoadingWheelElement wheel = (LoadingWheelElement)tempMode.Elements[tempMode.Elements.Length-1];
            wheel.PreLoadingMethod(wheel.Objects);
            Thread thread = new Thread(new ParameterizedThreadStart(AsynchronicLoadingAction));
            thread.Start(wheel);
        }
    }
}