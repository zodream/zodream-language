using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Language.Analyzers
{



    public enum TokenType
    {
        /// <summary>Not defined token</summary>
		None,
        /// <summary>End of file</summary>
        Eof,

        /// <summary>Invalid char</summary>
        InvalidChar,
        /// <summary>Invalid string</summary>
        InvalidString,
        /// <summary>Invalid string opening</summary>
        InvalidStringOpening,
        /// <summary>Invalid comment</summary>
        InvalidComment,

        /// <summary>NewLine</summary>
        [TokenName("\\n")]
        NewLine,
        /// <summary>Space</summary>
        Whitespace,
        Comment,
        CommentBlock,
        String,
        Number,
        /// <summary>Identifier</summary>
        Identifier,
        /// <summary>
        /// 定义数据类型
        /// </summary>
        DataType,
        /// <summary>
        /// 运算符 + - * / = 
        /// </summary>
        Operator,
        /// <summary>
        /// 复合运算符 ++ -- += -= *= /= ^= ~=
        /// </summary>
        CompoundOperator,
        /// <summary>
        /// 分隔符 ,;
        /// </summary>
        Separator,
        /// <summary>
        /// 判断符 == != && ||
        /// </summary>
        Determiner,
        /// <summary>
        /// 内部保留关键词
        /// </summary>
        ReservedKeywords,

        /// <summary>
        /// 成对出现的括号 (){}[]
        /// </summary>
        Bracket,
        /// <summary>
        /// 
        /// </summary>
        [TokenName(".")]
        Dot,
        [TokenName("..")]
        DotDot,
    }
}
