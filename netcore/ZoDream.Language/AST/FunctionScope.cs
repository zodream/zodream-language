using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Language.AST
{
    public class FunctionScope
    {
        public bool IsPublic { get; set; }

        public bool IsStatic { get; set; }

        public string Name { get; set; } = string.Empty;

        public Dictionary<string, PropertyScope> ArgumentItems = new();

        public List<Scope> BodyItems = new();


    }
}
