﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Cotorra.Core.Utils
{ 
    public class CotorraMemoryStream : MemoryStream
    {
        public CotorraMemoryStream()
        {
            // We always want to close streams by default to
            // force the developer to make the conscious decision
            // to disable it.  Then, they're more apt to remember
            // to re-enable it.  The last thing you want is to
            // enable memory leaks by default.  ;-)
            AllowClose = true;
        }

        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
}
