using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ZoDream.Language.Analyzers
{
    public class Position
    {
        public Position(SymbolDocumentInfo document, int line, int column, long index)
        {
            Document = document;
            Line = line;
            Column = column;
            Index = index;
        }

        public string FileName => Document.FileName;
        /// <summary>
        /// 文件
        /// </summary>
        internal SymbolDocumentInfo Document { get; private set; }
        /// <summary>
        /// 行号，从1开始
        /// </summary>
        public int Line { get; private set; }
        /// <summary>
        /// 列，从1开始
        /// </summary>
        public int Column { get; private set; }
        /// <summary>在整个内容中的位置，从0开始</summary>
        public long Index { get; private set; }

        public override string ToString()
            => string.Format("({0}; {1}; {2})", Line, Column, Index);

    }
}
