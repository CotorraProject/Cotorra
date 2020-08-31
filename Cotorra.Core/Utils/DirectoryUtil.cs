using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Cotorra.Core.Utils
{
    public static class DirectoryUtil
    {
        public static string AssemblyDirectory
        {
            get
            {
                
#if DEBUG
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
#else
                return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
#endif

            }
        }
    }
}
