using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace toxs.roslyn.analyzers.Extensions
{
    internal static class SwitchLabelSyntaxExtensions
    {
        public static SwitchLabelSyntax GetDefaultSwitchLabel(this SyntaxList<SwitchLabelSyntax> switchLabels)
        {
            return switchLabels.FirstOrDefault(label => label.IsKind(SyntaxKind.DefaultSwitchLabel));
        }
    }
}
