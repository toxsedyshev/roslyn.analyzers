using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Linq;

namespace toxs.roslyn.analyzers.Extensions
{
    internal static class SyntaxNodeAnalysisContextExtensions
    {
        public static (bool success, T syntaxNode) TryGetSyntaxNode<T>(this SyntaxNodeAnalysisContext context) where T : SyntaxNode
        {
            if (!(context.Node is T node))
            {
                return (success: false, syntaxNode: null);
            }

            if (node.GetDiagnostics().Any(x => x.Severity == DiagnosticSeverity.Error))
            {
                return (success: false, syntaxNode: null);
            }

            return (success: true, syntaxNode: node);
        }
    }
}
