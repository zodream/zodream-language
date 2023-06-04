using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Language.Analyzers
{
    public class Token
    {
        public Token(TokenType type, string value, Position begin, Position end)
        {
            Type = type;
            Begin = begin;
            End = end;
            Value = value;
        }


        public TokenType Type { get; private set; }

        public string Value { get; private set; }

        public Position Begin { get; private set; }

        public Position End { get; private set; }

        public int Length => (int)(End.Index - Begin.Index);

        public override string ToString()
            => string.Format("[{0,4},{1,4} - {2,4},{3,4}] {4}='{5}'", Begin.Line, Begin.Column, End.Line, End.Column, Type, Value);

    }
}
