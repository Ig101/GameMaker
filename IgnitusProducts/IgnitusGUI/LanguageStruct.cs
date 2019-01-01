using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus.IgnitusGUI
{
    public class StringStruct
    {
        string id;
        string text;

        public string Id { get { return id; } set { id = value; } }
        public string Text { get { return text; } set { text = value; } }

        public StringStruct (string id, string text)
        {
            this.id = id;
            this.text = text;
        }
    }

    public class LanguageStruct
    {
        string name;
        List<StringStruct> strings;

        public string Name { get { return name; } set { name = value; } }
        public List<StringStruct> Strings { get { return strings; } set { strings = value; } }

        public LanguageStruct(string name)
        {
            this.name = name;
            this.strings = new List<StringStruct>();
        }

        public void LoadStringStructs(string filePath)
        {
            if (File.Exists(filePath + name + ".mrc"))
            {
                strings.Clear();
                byte[] bytesLangs = Magic.Restore(filePath + name + ".mrc");
                string langs = Encoding.UTF8.GetString(bytesLangs);
                string[] phrases = langs.Split(new char[] { '\n' });
                for (int i = 0; i < phrases.Length; i++)
                {
                    string[] content = phrases[i].Split(new char[] { '@' });
                    if (content.Length >= 2)
                    {
                        strings.Add(new StringStruct(content[0], content[1]));
                    }
                }
            }
        }

        public void SaveStringStructs(string filePath)
        {
            string file = "";
            for(int i  =0; i<strings.Count;i++)
            {
                file += strings[i].Id + "@" + strings[i].Text + (i != strings.Count - 1 ? "\n" : "");
            }
            byte[] bytes = Encoding.UTF8.GetBytes(file);
            Magic.Act(filePath + name + ".mrc", bytes);
        }
    }
}
