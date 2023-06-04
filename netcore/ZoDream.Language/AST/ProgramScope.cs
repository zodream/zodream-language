using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Language.AST
{
    public class ProgramScope
    {
        public string EntryName = string.Empty;

        public Dictionary<string, PackageScope> PackageItems = new();


    }
}
