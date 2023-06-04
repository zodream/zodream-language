using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Language.AST
{
    public class PropertyScope: VariableScope
    {

        public bool IsPublic { get; set; }

        public bool IsStatic { get; set; }

    }
}
