using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Language.AST
{
    public class VariableScope
    {
        public bool IsMut { get; set; }

        public bool IsConst { get; set; }
        public string Name { get; set; } = string.Empty;

        public int Type { get; set; }

        public bool IsOptional { get; set; }

        public object? DefaultValue { get; set; }

    }
}
