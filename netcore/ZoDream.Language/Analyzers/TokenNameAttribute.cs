using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Language.Analyzers
{
    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class TokenNameAttribute : Attribute
    {
        public TokenNameAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; private set; }
    }
}
