using System;
using ZoDream.Language.Analyzers;
using ZoDream.Language.Analyzers.LexicalAnalyzers;

namespace ZoDream.Language
{
    public class ZoLang : IDisposable
    {


        public SymbolTable CreateEnvironment()
        {
            throw new NotImplementedException();
        }

        public T CreateEnvironment<T>()
            where T : SymbolTable
            => (T)Activator.CreateInstance(typeof(T), this);


        public void Compile(string folder)
        {
            var parser = new Parser();
            parser.ParseProgram(folder);
        }

        public void CompileFile(string fileName)
        {
            
        }

        public void CompileChunk(string content, string fileName)
        {
            var lex = new Lexer(fileName, content);
            var parser = new Parser();
            parser.ParseChunk(lex);
        }


        public void Dispose()
        {
            
        }

        ~ZoLang() 
        {
            Dispose();
        }
    }
}
