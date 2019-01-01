using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus.IgnitusGUI
{
    public class TextureStruct
    {
        string id;
        string path;
        string packName;

        public string Id { get { return id; } set { id = value; } }
        public string Path { get { return path; } set { path = value; } }
        public string PackName { get { return packName; } set { packName = value; } }

        public TextureStruct(string id, string path, string packName)
        {
            this.id = id;
            this.path = path;
            this.packName = packName;
        }
    }
}
