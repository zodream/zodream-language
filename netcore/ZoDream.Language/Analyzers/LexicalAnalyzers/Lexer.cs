using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using System.Xml.Linq;

namespace ZoDream.Language.Analyzers.LexicalAnalyzers
{
    public class Lexer: ILexicalAnalyzer
    {

        public Lexer(string fileName, TextReader reader)
        {
            _document = Expression.SymbolDocument(fileName);
            _reader = reader;
        }

        public Lexer(string fileName, string content)
            : this(fileName, new StringReader(content))
        {
        }

     

        public Lexer(string fileName, TextReader reader, bool isAbbreviation)
            : this(fileName, reader)
        {
            _analyzer = isAbbreviation ? new AbbreviationLexicalAnalyzer(_document, _reader) 
                : new FullLexicalAnalyzer(_document, _reader);
        }

        public Lexer(string fileName, string content, bool isAbbreviation)
         : this(fileName, new StringReader(content), isAbbreviation)
        {
        }

        private readonly SymbolDocumentInfo _document;
        private readonly TextReader _reader;
        private readonly ILexicalAnalyzer? _analyzer;

        public Token NextToken()
        {
            if (_analyzer is not null)
            {
                return _analyzer.NextToken();
            }
            return new Token(TokenType.None, string.Empty, 
                new Position(_document, 1, 1, 1), new Position(_document, 1, 1, 1));
        }
        
        /// <summary>
        /// 跳过多少个
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public Token JumpToken(int count = 1)
        {
            while (count -- > 0)
            {
                NextToken();
            }
            return NextToken();
        }
    }
}
