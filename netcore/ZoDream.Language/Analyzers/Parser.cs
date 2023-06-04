using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using ZoDream.Language.Analyzers.LexicalAnalyzers;
using ZoDream.Language.AST;

namespace ZoDream.Language.Analyzers
{
    public class Parser
    {

        public Expression ParseProgram(TextReader reader)
        {
            var parameters = new List<ParameterExpression>();
            var globalScope = new GlobalScope();

            return Expression.Lambda(globalScope.ExpressionBlock, parameters);
        }

        public Expression ParseChunk(ILexicalAnalyzer reader)
        {
            var parameters = new List<ParameterExpression>();
            var globalScope = new GlobalScope();

            return Expression.Lambda(globalScope.ExpressionBlock, parameters);
        }
    }
}
