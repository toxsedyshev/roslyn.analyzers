using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace toxs.roslyn.analyzers.Extensions
{
    internal static class SyntaxTriviaExtensions
    {
        public static bool IsWhitespaceOrEndOfLineTrivia(this SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.WhitespaceTrivia)
                || trivia.IsKind(SyntaxKind.EndOfLineTrivia);
        }
    }
}