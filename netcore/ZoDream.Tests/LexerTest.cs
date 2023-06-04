using ZoDream.Language.Analyzers;
using ZoDream.Language.Analyzers.LexicalAnalyzers;
using ZoDream.Language.Utils;

namespace ZoDream.Tests
{
    [TestClass]
    public class LexerTest
    {
        [TestMethod]
        public void TestNextToken()
        {
            var analyzer = new Lexer("http.z", """
                                struct Http {
                    Router
                    a int
                    _ b int
                    : c int
                }
                """
                , false);
            Assert.AreEqual(analyzer.NextToken().Type, TokenType.ReservedKeywords);
        }

        [TestMethod]
        public void TestNextToken2()
        {
            var analyzer = new Lexer("http.z", """
                a int
                _ b int
                : c int
                ()
                    a=1
                    if a==1
                        a++
                    for 1..2,5
                        return i
                """
                , true);
            var items = new List<Token>();
            while (true)
            {
                var token = analyzer.NextToken();
                items.Add(token);
                if (token.Type == TokenType.Eof)
                {
                    break;
                }
            }
            Assert.AreEqual(items.Count, 46);
        }

        [TestMethod]
        public void TestStudly()
        {
            Assert.AreEqual(Str.Studly("zo_dR_ea_m", false), "zoDREaM");
        }
    }
}