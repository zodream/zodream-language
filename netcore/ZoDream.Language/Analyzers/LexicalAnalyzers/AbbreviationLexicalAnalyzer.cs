using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using ZoDream.Language.AST;
using ZoDream.Language.Utils;

namespace ZoDream.Language.Analyzers.LexicalAnalyzers
{
    /// <summary>
    /// 基于简写语法的分析器
    /// </summary>
    public class AbbreviationLexicalAnalyzer : BaseLexicalAnalyzer
    {
        public AbbreviationLexicalAnalyzer(string fileName, TextReader reader)
            : base(fileName, reader)
        {
        }

        public AbbreviationLexicalAnalyzer(SymbolDocumentInfo document, TextReader reader)
            : base(document, reader)
        {
            var position = CreatePosition(0, 0, 0);
            NextTokenQueue.Enqueue(CreateToken("struct", position, position));
            NextTokenQueue.Enqueue(new Token(TokenType.Identifier, GetStructName(Document), position, position));
            NextTokenQueue.Enqueue(new Token(TokenType.Bracket, "{",
                position, position));
        }
        
        /// <summary>
        /// 未关闭的 { 及所处行的值
        /// </summary>
        private readonly List<int> _notCloseBlock = new();
        private int _linePreviousHasBlock = -1;
        private int _footerToken = 1;

        protected override Token GetToken()
        {
            int next;
            while (true)
            {
                if (CurrentToken.Type == TokenType.NewLine)
                {
                    if (AutoAddBlockTag())
                    {
                        return NextTokenQueue.Dequeue();
                    }
                }
                var codeInt = ReadChar();
                if (codeInt == -1)
                {
                    if (_footerToken > 0)
                    {
                        CloseAllBlock();
                        return NextTokenQueue.Dequeue();
                    }
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
                _linePreviousHasBlock = codeInt == '{' || codeInt == '}' ? codeInt : -1;
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

        private bool AutoAddBlockTag()
        {
            var spaceCount = GetWhiteSpaceCount();
            var lastLineSpace = _notCloseBlock.Count < 1 ? 0 : _notCloseBlock.Last();
            if (spaceCount == lastLineSpace)
            {
                return false;
            }
            if (spaceCount > lastLineSpace)
            {
                if (_linePreviousHasBlock != '{')
                {
                    NextTokenQueue.Enqueue(CreateToken(TokenType.Bracket, "{", 0, 0));
                    _notCloseBlock.Add(spaceCount);
                    return true;
                }
            } else if (_notCloseBlock.Count > 0)
            {
                for (var i = _notCloseBlock.Count - 1; i >= 0; i--)
                {
                    if (_notCloseBlock[i] <= spaceCount)
                    {
                        break;
                    }
                    NextTokenQueue.Enqueue(CreateToken(TokenType.Bracket, "}", 0, 0));
                    _notCloseBlock.RemoveAt(i);
                }
                return NextTokenQueue.Count > 0;
            }
            return false;
        }

        private void CloseAllBlock()
        {
            for (var i = _notCloseBlock.Count; i >= 0; i--)
            {
                NextTokenQueue.Enqueue(CreateToken(TokenType.Bracket, "}", 0, 0));
            }
            _notCloseBlock.Clear();
            _footerToken--;
        }

        private int GetWhiteSpaceCount()
        {
            var total = 0;
            while (true)
            {
                var code = ReadChar();
                if (code < 0)
                {
                    return 0;
                }
                if (IsNewLine(code))
                {
                    MoveBackChar();
                    return 0;
                }
                if (!IsWhiteSpace(code))
                {
                    MoveBackChar();
                    break;
                }
                total += code == '\t' ? 2 : 1;
                
            }
            return total;
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
                    MoveBackChar();
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
                text = text.Substring(1,1) + text[..1];
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
                } else if (isHex == 1)
                {
                    if (!IsNumeric(codeInt))
                    {
                        MoveBackChar();
                        break;
                    }
                } else if (isHex == 2 && !IsHexNumeric(codeInt))
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

        

        /// <summary>
        /// 根据文件名自动获取 struct
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetStructName(string fileName)
        {
            return Str.Studly(Path.GetFileNameWithoutExtension(fileName));
        }

        public static string GetStructName(SymbolDocumentInfo doc)
        {
            return GetStructName(doc.FileName);
        }
    }
}
