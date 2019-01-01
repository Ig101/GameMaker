using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ignitus
{
    public class ObjectEventArgs : EventArgs
    {
        object[] objects;

        public object[] Objects { get { return objects; } }

        public ObjectEventArgs(object[] objs)
        {
            this.objects = objs;
        }
    }
}
