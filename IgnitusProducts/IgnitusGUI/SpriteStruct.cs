using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus.IgnitusGUI
{
    public class SpriteStruct
    {
        string name;
        string path;

        public string Name { get { return name; } set { name = value; } }
        public string Path { get { return path; } set { path = value; } }

        public SpriteStruct (string name, string path)
        {
            this.name = name;
            this.path = path;
        }

        public SpriteInfo LoadWholeSprite()
        {
            if (path != null)
            {
                return new SpriteInfo(name, path);
            }
            else
            {
                return new SpriteInfo(name);
            }
        }
    }
}
