using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus.IgnitusGUI
{
    public class NativeStruct
    {
        string[] parametres;
        string type;
        string name;
        IgnitusManager.GetStringByNative nativesMethod;

        public string[] Parametres { get { return parametres; } set { parametres = value; } }
        public string Type { get { return type; } set { type = value; } }
        public string Name { get { return name; } set { name = value; } }

        public NativeStruct (string name, string type, string[] parametres, IgnitusManager.GetStringByNative nativesMethod)
        {
            this.nativesMethod = nativesMethod;
            this.name = name;
            this.type = type;
            this.parametres = parametres;
        }

        public void UpdateStruct(object native)
        {
            string newString = nativesMethod(native);
            parametres = newString.Split(new char[] { ';' });
        }
    }
}
