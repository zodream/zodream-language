using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;

namespace ZoDream.Language.Analyzers.LexicalAnalyzers
{
    /// <summary>
    /// 基于完整语法的分析器
    /// </summary>
    public class FullLexicalAnalyzer : BaseLexicalAnalyzer
    {
        public FullLexicalAnalyzer(string fileName, TextReader reader)
            : base(fileName, reader)
        {
        }

        public FullLexicalAnalyzer(SymbolDocumentInfo document, TextReader reader)
            : base(document, reader)
        {
        }


        protected override Token GetToken()
        {
            int next;
            while (true)
            {
                var codeInt = ReadChar();
                if (codeInt == -1)
                {
                    return CreateToken(TokenType.Eof, string.Empty, 0, 0);
                }
                if (IsNewLine(codeInt))
                {
                    return CreateToken(TokenType.NewLine, 0, 0);
                }
                var code = (char)codeInt;
                if (IsWhiteSpace(code))
                {
                    continue;
                }
                if (code is '\'' or '"')
                {
                    return GetStringToken(code);
                }
                if (code is >= '0' and <= '9')
                {
                    return GetNumericToken(code);
                }
                if (IsSeparator(code))
                {
                    return CreateToken(TokenType.Separator, code.ToString(), 0, 1);
                }
                if (IsBracketOpen(codeInt) || IsBracketClose(codeInt))
                {
                    return CreateToken(TokenType.Bracket, code.ToString(), 0, 1);
                }
                if (code == '.')
                {
                    next = ReadChar();
                    if (next == '.')
                    {
                        return CreateToken(TokenType.DotDot, 1, 1);
                    }
                    MoveBackChar();
                    return CreateToken(TokenType.Dot, 0, 1);
                }
                if (code == ':')
                {
                    return CreateToken(TokenType.ReservedKeywords, "static", 0, 1);
                }
                if (code == '/')
                {
                    next = ReadChar();
                    if (next < 0)
                    {
                        return CreateToken(TokenType.InvalidChar, code.ToString(), 0, 0);
                    }
                    if (next == '=')
                    {
                        return CreateToken(TokenType.CompoundOperator, "/=", 2, 0);
                    }
                    if (next == '*')
                    {
                        return GetCommentBlockToken();
                    }
                    if (next == '/')
                    {
                        return GetCommentToken();
                    }
                    MoveBackChar();
                    continue;
                }
                if (code == '_')
                {
                    next = ReadChar();
                    if (next < 0)
                    {
                        return CreateToken(TokenType.InvalidChar, code.ToString(), 0, 0);
                    }
                    if (IsWhiteSpace(next))
                    {
                        return CreateToken("private", 0, 0);
                    }
                    MoveBackChar();
                    return GetNameToken(code);
                }
                if (code == '@')
                {
                    return CreateToken("import", 0, 0);
                }
                if (IsCombinationSymbol(code))
                {
                    return GetCombinationSymbol(code);
                }
                if (!IsWhiteSpace(code))
                {
                    return GetNameToken(code);
                }
            }
        }

        private Token GetNameToken(char code)
        {
            var sb = new StringBuilder();
            sb.Append(code);
            var begin = CreatePosition();
            while (true)
            {
                var codeInt = ReadChar();
                if (codeInt < 0)
                {
                    break;
                }
                if (IsNewLine(codeInt))
                {
                    MoveBackChar();
                    break;
                }
                var c = (char)codeInt;
                if (IsWhiteSpace(c))
                {
                    MoveBackChar();
                    break;
                }
                if (!(IsNumeric(codeInt) || IsAlphabet(codeInt)) || codeInt > 127)
                {
                    MoveBackChar();
                    break;
                }
                sb.Append(c);
            }
            return CreateToken(sb.ToString(), begin);
        }

        private Token GetCombinationSymbol(char code)
        {
            var sb = new StringBuilder();
            sb.Append(code);
            var begin = CreatePosition();
            while (true)
            {
                var codeInt = ReadChar();
                if (codeInt < 0)
                {
                    break;
                }
                var c = (char)codeInt;
                if (IsWhiteSpace(c))
                {
                    break;
                }
                if (!IsCombinationSymbol(c))
                {
                    MoveBackChar();
                    break;
                }
                sb.Append(c);
            }
            var text = sb.ToString();
            if (text == "=>" || text == "=<")
            {
                text = text.Substring(1, 1) + text[..1];
            }
            if (IsCompoundOperator(text))
            {
                return new Token(TokenType.CompoundOperator, text, begin, CreatePosition());
            }
            if (IsOperator(text))
            {
                return new Token(TokenType.Operator, text, begin, CreatePosition());
            }
            if (IsDeterminer(text))
            {
                return new Token(TokenType.Determiner, text, begin, CreatePosition());
            }
            switch (text)
            {
                case "//":
                    return GetCommentToken();
                case "/*":
                    return GetCommentBlockToken();
                case "!":
                    return new Token(TokenType.ReservedKeywords, text, begin, CreatePosition());
                case "=":
                    return new Token(TokenType.ReservedKeywords, text, begin, CreatePosition());
                default:
                    break;
            }
            return new Token(TokenType.InvalidChar, text, begin, CreatePosition());
        }



        /// <summary>
        /// 单行注释
        /// </summary>
        /// <returns></returns>
        private Token GetCommentToken()
        {
            var sb = new StringBuilder();
            var begin = CreatePosition();
            while (true)
            {
                var codeInt = ReadChar();
                if (codeInt < 0)
                {
                    break;
                }
                if (IsNewLine(codeInt))
                {
                    MoveBackChar();
                    break;
                }
                sb.Append((char)codeInt);
            }
            return new Token(TokenType.Comment, sb.ToString(), begin, CreatePosition());
        }
        /// <summary>
        /// 多行注释
        /// </summary>
        /// <returns></returns>
        private Token GetCommentBlockToken()
        {
            var sb = new StringBuilder();
            var begin = CreatePosition();
            var foundStar = false;
            while (true)
            {
                var codeInt = ReadChar();
                if (codeInt < 0)
                {
                    break;
                }
                if (codeInt == '/' && foundStar)
                {
                    break;
                }
                if (foundStar)
                {
                    sb.Append('*');
                }
                if (codeInt == '*')
                {
                    foundStar = true;
                    continue;
                }
                foundStar = false;
                sb.Append((char)codeInt);
            }
            return new Token(TokenType.Comment, sb.ToString(), begin, CreatePosition());
        }


        private Token GetNumericToken(char code)
        {
            var isHex = 0; // 是否是十六进制, 0 未判断 1 是小数 2 是进制
            var sb = new StringBuilder();
            var begin = CreatePosition();
            sb.Append(code);
            while (true)
            {
                var codeInt = ReadChar();
                if (codeInt < 0)
                {
                    break;
                }
                if (codeInt == '.')
                {
                    var next = ReadChar();
                    if (next == '.')
                    {
                        NextTokenQueue.Enqueue(CreateToken(TokenType.DotDot, 1, 1));
                        return CreateToken(TokenType.String, sb.ToString(), 2, -1);
                    }
                    isHex = 1;
                    sb.Append((char)codeInt);
                    if (next < 0)
                    {
                        break;
                    }
                    codeInt = next;
                }
                if (isHex == 0)
                {
                    if (codeInt != 'X' && codeInt != 'x' && !IsHexNumeric(codeInt))
                    {
                        MoveBackChar();
                        break;
                    }
                    if (!IsNumeric(codeInt))
                    {
                        isHex = 2;
                    }
                }
                else if (isHex == 1)
                {
                    if (!IsNumeric(codeInt))
                    {
                        MoveBackChar();
                        break;
                    }
                }
                else if (isHex == 2 && !IsHexNumeric(codeInt))
                {
                    MoveBackChar();
                    break;
                }
                sb.Append((char)codeInt);
            }
            return new Token(TokenType.String, sb.ToString(), begin, CreatePosition());
        }

        private Token GetStringToken(char end)
        {
            var reverseCount = 0;
            var sb = new StringBuilder();
            var begin = CreatePosition();
            while (true)
            {
                var codeInt = ReadChar();
                if (codeInt < 0)
                {
                    break;
                }
                if (codeInt == end && reverseCount % 2 == 0)
                {
                    break;
                }
                if (codeInt == '\\')
                {
                    reverseCount++;
                    if (reverseCount == 2)
                    {
                        sb.Append((char)codeInt);
                        reverseCount = 0;
                    }
                    continue;
                }
                reverseCount = 0;
                sb.Append((char)codeInt);
            }
            return new Token(TokenType.String, sb.ToString(), begin, CreatePosition());
        }



    }
}
