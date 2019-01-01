using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus.IgnitusGUI
{
    public class SoundStruct
    {
        bool sound;
        string name;
        string path;

        public bool Sound { get { return sound; } set { sound = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Path { get { return path; } set { path = value; } }

        public SoundStruct(bool sound, string name, string path)
        {
            this.sound = sound;
            this.name = name;
            this.path = path;
        }
    }
}
