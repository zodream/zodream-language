using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Language.AST
{
    public class PackageScope
    {

        public Dictionary<string, PackageScope> StructItems = new();
    }
}
