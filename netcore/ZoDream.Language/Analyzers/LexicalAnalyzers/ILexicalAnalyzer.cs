using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Language.Analyzers.LexicalAnalyzers
{
    public interface ILexicalAnalyzer
    {

        public Token NextToken();
    }
}
