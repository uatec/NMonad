using System;
using System.Collections.Generic;
using System.Linq;
using NMonad.Win32Interop;

namespace NMonad
{
    public class WindowList : List<Window>
    {
        public new void Add(Window w)
        {
            if ( this.Select(x => x.Handle).Contains(w.Handle)) throw new Exception("Window has been detected as 'new' already and may not be readded.");
            base.Add(w);
        }

        public new void Clear()
        {
            throw new Exception("wat");
        }

        public new void Remove(Window w)
        {
            base.Remove(w);
        }
    }
}