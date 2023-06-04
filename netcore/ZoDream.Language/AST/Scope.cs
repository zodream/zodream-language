using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ZoDream.Language.AST
{
    public class Scope
    {

        private readonly Scope? Parent;

        private Dictionary<string, Expression>? Variables = null;

        private List<Expression> BlockItems = new();

        public Expression ExpressionBlock =>  null;

    }
}
