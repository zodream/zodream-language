using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Text;
using ZoDream.Language.Utils;

namespace ZoDream.Language.Analyzers.LexicalAnalyzers
{
    public abstract class BaseLexicalAnalyzer: ILexicalAnalyzer
    {
        public BaseLexicalAnalyzer(string fileName, TextReader reader)
            : this(Expression.SymbolDocument(fileName), reader)
        {
        }

        public BaseLexicalAnalyzer(SymbolDocumentInfo document, TextReader reader)
        {
            Document = document;
            Reader = reader;
            CurrentToken = CreateToken(TokenType.None, 0, 0);
        }

        protected readonly SymbolDocumentInfo Document;
        protected readonly TextReader Reader;
        /// <summary>
        /// 读取多了需要，排个队列
        /// </summary>
        protected readonly Queue<Token> NextTokenQueue = new();
        /// <summary>
        /// 上一次获取到的Token
        /// </summary>
        protected Token CurrentToken;
        private int _lineIndex = 0;
        private int _columnIndex = 0;
        private int _charIndex = -1;
        // 上一个字符
        private int _lastChar = -1;
        // 当前的字符
        private int _currentChar = -1;
        // 指示下一次只获取当前的
        private bool _moveNextStop = false;

        public Token NextToken()
        {
            Token token;
            if (NextTokenQueue.Count > 0)
            {
                token = NextTokenQueue.Dequeue();
            } else
            {
                token = GetToken();
            }
            CurrentToken = token;
            return token;
        }


        protected abstract Token GetToken();

        public void MoveBackChar()
        {
            _moveNextStop = true;
        }

        protected int ReadChar()
        {
            if (_moveNextStop)
            {
                _moveNextStop = false;
                return _currentChar;
            }
            _lastChar = _currentChar;
            _currentChar = Reader.Read();
            if (_currentChar == -1)
            {
                return _currentChar;
            }
            _charIndex++;
            if (_currentChar == '\n' && _lastChar == '\r')
            {
                return ReadChar();
            }
            if (IsNewLine(_currentChar))
            {
                _lineIndex++;
                _columnIndex = 0;
            }
            else
            {
                _columnIndex++;
            }
            return _currentChar;
        }


        protected bool IsNewLine(int code)
        {
            return code == '\r' || code == '\n';
        }

        protected bool IsOperator(string value)
        {
            return value switch
            {
                "+" or "-" or "*" or "/" or "~" or "^" or "<<" or ">>" or "&"
                or "|" or "%" => true,
                _ => false
            };
        }

        protected bool IsDeterminer(string value)
        {
            return value switch
            {
                "==" or "!=" or "&&" or "||" or ">" or "<" or "<=" or ">=" => true,
                _ => false
            };
        }

        protected bool IsCompoundOperator(string value)
        {
            return value switch
            {
                "++" or "--" or "+=" or "-=" or "*=" or "/=" or "%=" or "^=" or "~=" => true,
                _ => false
            };
        }

        protected bool IsCombinationSymbol(char code)
        {
            return code switch
            {
                '!' or '&' or '=' or '%' or '*' or '+' or '/' or '-' or '.'
                or '<' or '>' or '?' or '^' or '~' => true,
                _ => false
            };
        }

        protected bool IsNumeric(int code)
        {
            return code >= '0' && code <= '9';
        }

        protected bool IsHexNumeric(int code)
        {
            return IsNumeric(code) || (code >= 'a' && code <= 'f') ||
                    (code >= 'A' && code <= 'F');
        }

        /// <summary>
        /// 是否是大写字母
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected bool IsUpperAlphabet(int code)
        {
            return code >= 'A' && code <= 'Z';
        }

        /// <summary>
        /// 是否是小写字母
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected bool IsLowerAlphabet(int code)
        {
            return code >= 'a' && code <= 'z';
        }

        protected bool IsBracketOpen(int code)
        {
            return code switch
            {
                '(' or '{' or '[' => true,
                _ => false
            };
        }

        protected bool IsBracketClose(int code)
        {
            return code switch
            {
                ')' or '}' or ']' => true,
                _ => false
            };
        }

        /// <summary>
        /// 是否是字母
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected bool IsAlphabet(int code)
        {
            return IsUpperAlphabet(code) || IsLowerAlphabet(code);
        }

        /// <summary>
        /// 也包括换行
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        protected bool IsWhiteSpace(char code)
        {
            return char.IsWhiteSpace(code);
        }

        protected bool IsWhiteSpace(int code)
        {
            return IsWhiteSpace((char)code);
        }

        protected bool IsReservedKeywords(string text)
        {
            return text switch
            {
                "!" or "=" or "return" or "if" or "import" or "struct" or "for" or "break" or "switch" or "fn"
                or "public" or "static" or "private" or "mut" or "const" or "var" or "as" => true,
                _ => false
            };
        }

        protected bool IsSeparator(int code)
        {
            return code is ',' or ';';
        }

        protected bool IsDataType(string text)
        {
            return text switch
            {
                "int" or "bool" or "float" or "double" or "string" or "object" or "map" or "interface" => true,
                _ => false
            };
        }

        protected Token CreateToken(string value, int beginOffset, int endOffset = 0)
        {
            var type = TokenType.Identifier;
            if (IsReservedKeywords(value))
            {
                type = TokenType.ReservedKeywords;
            }
            else if (IsDataType(value))
            {
                type |= TokenType.DataType;
            }
            return CreateToken(type, value, beginOffset, endOffset);
        }

        protected Token CreateToken(string value, Position begin, Position end)
        {
            var type = TokenType.Identifier;
            if (IsReservedKeywords(value))
            {
                type = TokenType.ReservedKeywords;
            }
            else if (IsDataType(value))
            {
                type |= TokenType.DataType;
            }
            return new Token(type, value, begin, end);
        }

        protected Token CreateToken(string value, Position begin)
        {
            return CreateToken(value, begin, CreatePosition());
        }


        protected Token CreateToken(TokenType type, string value, int beginOffset, int endOffset = 0)
        {
            return new Token(type, value,
                new Position(Document, _lineIndex, _columnIndex - beginOffset, _charIndex - beginOffset),
                new Position(Document, _lineIndex, _columnIndex + endOffset, _charIndex + endOffset));
        }

        protected Token CreateToken(TokenType type, int beginOffset, int endOffset = 0)
        {
            var value = type.ToString();
            var attribute = CustomAttributeExtensions.GetCustomAttributeValue<TokenNameAttribute, string>(
                typeof(TokenType),
                x => string.IsNullOrEmpty(x.Name) ? value : x.Name, value);
            return CreateToken(type, attribute ?? value, beginOffset, endOffset);
        }

        /// <summary>
        /// 往前偏移
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        protected Position CreatePosition(int offset)
        {
            return CreatePosition(_lineIndex, _columnIndex -
                offset, _charIndex - offset);
        }

        protected Position CreatePosition(int line, int column, int index)
        {
            return new Position(Document, line, column, index);
        }

        protected Position CreatePosition()
        {
            return CreatePosition(_moveNextStop ? 1 : 0);
        }
    }
}
