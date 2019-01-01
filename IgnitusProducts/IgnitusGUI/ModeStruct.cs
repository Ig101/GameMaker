using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus.IgnitusGUI
{

    public class ModeStruct
    {
        string strings;
        string name;
        IgnitusManager.GetStringByElement elementMethod;

        public string Strings { get { return strings; } set { strings = value; } }
        public string Name { get { return name; } set { name = value; } }

        public ModeStruct(string strings,string name, IgnitusManager.GetStringByElement elementMethod)
        {
            this.elementMethod = elementMethod;
            this.strings = strings;
            this.name = name;
        }

        public void UpdateStruct (Mode mode)
        {
            //[идентификатор];[имя родительского режима];[скорость анимации];[имя];[название метода типа GetAnimationTransformation];[название метода типа ActionOnOpening];[1 если клавиатура используется, 0 если не используется];[элемент1];..;[элементN]
            string newString = mode.Name + ";" + mode.Parent.Name + ";" + mode.AniationSpeed.ToString() + ";"
                + mode.AnimationMatrix.ToString() + ";" + mode.Action.ToString() + ";" + (mode.KeyboardUse ? "1" : "0");
            for(int i = 0; i<mode.Elements.Length;i++)
            {
                newString += ";" + elementMethod(mode.Elements[i]);
            }
            this.strings = newString;
        }
    }
}
