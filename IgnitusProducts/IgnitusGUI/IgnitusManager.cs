using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus.IgnitusGUI
{


    public class IgnitusManager
    {
        public delegate string GetStringByElement(HudElement elem);
        public delegate string GetStringByNative(object obj);

        List<ModeStruct> modes;
        List<SoundStruct> sounds;
        List<SpriteStruct> sprites;
        List<TextureStruct> textures;
        List<LanguageStruct> languages;
        List<NativeStruct> natives;

        string nativesPath;
        string modesPath;
        string soundsPath;
        string spritesPath;
        string texturesPath;
        string languagesPath;

        GetStringByElement modeElementMethod;
        GetStringByNative nativeMethod;

        public List<NativeStruct> Natives { get { return natives; } }
        public List<ModeStruct> Modes { get { return modes; } }
        public List<SoundStruct> Sounds { get { return sounds; } }
        public List<SpriteStruct> Sprites { get { return sprites; } }
        public List<TextureStruct> Textures { get { return textures; } }
        public List<LanguageStruct> Languages { get { return languages; } }
        public string NativesPath { get { return nativesPath; } set { nativesPath = value; } }
        public string ModesPath { get { return modesPath; } set { modesPath = value; } }
        public string SoundsPath { get { return soundsPath; } set { soundsPath = value; } }
        public string SpritesPath { get { return spritesPath; } set { spritesPath = value; } }
        public string TexturesPath { get { return texturesPath; } set { texturesPath = value; } }
        public string LanguagesPath { get { return languagesPath; } set { languagesPath = value; } }

        public IgnitusManager(GetStringByElement modeElementMethod, GetStringByNative nativeMethod)
        {
            modes = new List<ModeStruct>();
            sounds = new List<SoundStruct>();
            sprites = new List<SpriteStruct>();
            textures = new List<TextureStruct>();
            languages = new List<LanguageStruct>();
            natives = new List<NativeStruct>();
            this.modeElementMethod = modeElementMethod;
            this.nativeMethod = nativeMethod;
        }

        public void LoadAllData()
        {
            LoadLanguages();
            LoadTextures();
            LoadSprites();
            LoadSounds();
            LoadNatives();
            LoadModes();
        }

        public void SaveAllData()
        {
            SaveLanguages();
            SaveTextures();
            SaveSprites();
            SaveSounds();
            SaveNatives();
            SaveModes();
        }

        #region langs
        public void SaveLanguages(string path)
        {
            foreach (LanguageStruct str in languages)
            {
                str.SaveStringStructs(path);
            }
        }

        public void SaveLanguages()
        {
            SaveLanguages(languagesPath);
        }

        public void LoadLanguages(string path)
        {
            if (Directory.Exists(path))
            {
                languages.Clear();
                FileInfo[] files = new DirectoryInfo(path).GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i].Extension == ".mrc")
                    {
                        string name = files[i].Name.Substring(0, files[i].Name.Length - 4);
                        LanguageStruct str = new LanguageStruct(name);
                        str.LoadStringStructs(path);
                        languages.Add(str);
                    }
                }
            }
        }

        public void LoadLanguages()
        {
            this.LoadLanguages(languagesPath);
        }
        #endregion

        #region textures
        public void LoadTextures()
        {
            LoadTextures(texturesPath);
        }

        public void LoadTextures(string path)
        {
            if (File.Exists(path))
            {
                this.textures.Clear();
                byte[] bytes = Magic.Restore(path);
                string info = Encoding.UTF8.GetString(bytes);
                string[] textures = info.Split(new char[] { '\n' });
                for (int i = 0; i < textures.Length; i++)
                {
                    string[] parts = textures[i].Split(new char[] { ';' });
                    this.textures.Add(new TextureStruct(parts[0], parts[1], parts[2]));
                }
            }
        }

        public void SaveTextures()
        {
            SaveTextures(texturesPath);
        }

        public void SaveTextures(string path)
        {
            string file = "";
            for (int i = 0; i < textures.Count; i++)
            {
                file += textures[i].Id + ";" + textures[i].Path + ";" + textures[i].PackName + (i != textures.Count - 1 ? "\n" : "");
            }
            byte[] bytes = Encoding.UTF8.GetBytes(file);
            Magic.Act(path, bytes);
        }
        #endregion

        #region Sounds
        public void LoadSounds()
        {
            LoadSounds(soundsPath);
        }

        public void LoadSounds(string path)
        {
            if (File.Exists(path))
            {
                this.sounds.Clear();
                byte[] bytes = Magic.Restore(path);
                string info = Encoding.UTF8.GetString(bytes);
                string[] textures = info.Split(new char[] { '\n' });
                for (int i = 0; i < textures.Length; i++)
                {
                    string[] parts = textures[i].Split(new char[] { ';' });
                    sounds.Add(new SoundStruct(parts[0] == "sound", parts[1], parts[2]));
                }
            }
        }

        public void SaveSounds()
        {
            SaveSounds(soundsPath);
        }

        public void SaveSounds(string path)
        {
            string file = "";
            for (int i = 0; i < sounds.Count; i++)
            {
                file += (sounds[i].Sound?"sound":"effect") + ";" +sounds[i].Name + ";" + sounds[i].Path + (i != sounds.Count - 1 ? "\n" : "");
            }
            byte[] bytes = Encoding.UTF8.GetBytes(file);
            Magic.Act(path, bytes);
        }
        #endregion

        #region sprites
        public void LoadSprites()
        {
            LoadSprites(spritesPath);
        }

        public void LoadSprites(string path)
        {
            if (File.Exists(path))
            {
                this.sprites.Clear();
                byte[] bytes = Magic.Restore(path);
                string info = Encoding.UTF8.GetString(bytes);
                string[] textures = info.Split(new char[] { '\n' });
                for (int i = 0; i < textures.Length; i++)
                {
                    string[] parts = textures[i].Split(new char[] { ';' });
                    sprites.Add(new SpriteStruct(parts[0], parts.Length>1?parts[1]:null));
                }
            }
        }

        public void SaveSprites()
        {
            SaveSprites(spritesPath);
        }

        public void SaveSprites(string path)
        {
            string file = "";
            for (int i = 0; i < sprites.Count; i++)
            {
                file += sprites[i].Name + ";" + sprites[i].Path + (i != sprites.Count - 1 ? "\n" : "");
            }
            byte[] bytes = Encoding.UTF8.GetBytes(file);
            Magic.Act(path, bytes);
        }
        #endregion

        #region modes
        public void LoadModes ()
        {
            LoadModes(modesPath);
        }

        public void LoadModes(string path)
        {
            if (File.Exists(path))
            {
                this.sprites.Clear();
                byte[] bytes = Magic.Restore(path);
                string info = Encoding.UTF8.GetString(bytes);
                string[] modes = info.Split(new char[] { '\n' });
                for (int i = 0; i < modes.Length; i++)
                {
                    string[] parts = modes[i].Split(new char[] { ';' });
                    this.modes.Add(new ModeStruct(parts[0],modes[i], modeElementMethod));
                }
            }
        }

        public void SaveModes()
        {
            SaveModes(modesPath);
        }

        public void SaveModes(string path)
        {
            string file = "";
            for (int i = 0; i < modes.Count; i++)
            {
                file += modes[i].Strings + (i != modes.Count - 1 ? "\n" : "");
            }
            byte[] bytes = Encoding.UTF8.GetBytes(file);
            Magic.Act(path, bytes);
        }
        #endregion

        #region natives
        public void LoadNatives()
        {
            LoadNatives(nativesPath);
        }

        public void LoadNatives(string path)
        {
            if (File.Exists(path))
            {
                this.natives.Clear();
                byte[] bytes = Magic.Restore(path);
                string info = Encoding.UTF8.GetString(bytes);
                string[] natives = info.Split(new char[] { '\n' });
                for (int i = 0; i < natives.Length; i++)
                {
                    string[] parts = natives[i].Split(new char[] { ';' });
                    string[] parametres = new string[parts.Length - 2];
                    parts.CopyTo(parametres, 2);
                    this.natives.Add(new NativeStruct(parts[1], parts[0], parametres, nativeMethod));
                }
            }
        }

        public void SaveNatives()
        {
            SaveNatives(nativesPath);
        }
        public void SaveNatives(string path)
        {
            string file = "";
            for (int i = 0; i < natives.Count; i++)
            {
                file += natives[i].Type + ";" + natives[i].Name;
                for(int j = 0; j<natives[i].Parametres.Length;j++)
                {
                    file += ";" + natives[i].Parametres[j];
                }
                if (i != natives.Count)
                {
                    file += "\n";
                }
            }
            byte[] bytes = Encoding.UTF8.GetBytes(file);
            Magic.Act(path, bytes);
        }
        #endregion
    }
}
