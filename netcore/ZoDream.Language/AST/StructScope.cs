using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Language.AST
{
    public class StructScope
    {
        public Dictionary<string, PropertyScope> PropertyItems = new();
        public Dictionary<string, List<FunctionScope>> MethodItems = new();
    }
}
