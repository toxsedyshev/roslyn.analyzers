﻿using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis;
using System.Threading;
using System;

namespace toxs.roslyn.analyzers.Extensions
{
    internal static class SyntaxTreeExtensions
    {
        public static bool OverlapsHiddenPosition(this SyntaxTree tree, TextSpan span, CancellationToken cancellationToken)
        {
            if (tree == null)
            {
                return false;
            }

            var text = tree.GetText(cancellationToken);

            return text.OverlapsHiddenPosition(span, (position, cancellationToken2) =>
            {
                // implements the ASP.NET IsHidden rule
                var lineVisibility = tree.GetLineVisibility(position, cancellationToken2);
                return lineVisibility == LineVisibility.Hidden || lineVisibility == LineVisibility.BeforeFirstLineDirective;
            },
            cancellationToken);
        }
        public static bool OverlapsHiddenPosition(
            this SourceText text, TextSpan span, Func<int, CancellationToken, bool> isPositionHidden, CancellationToken cancellationToken)
        {
            var result = TryOverlapsHiddenPosition(text, span, isPositionHidden, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            return result;
        }

        /// <summary>
        /// Same as OverlapsHiddenPosition but doesn't throw on cancellation.  Instead, returns false
        /// in that case.
        /// </summary>
        public static bool TryOverlapsHiddenPosition(
            this SourceText text, TextSpan span, Func<int, CancellationToken, bool> isPositionHidden,
            CancellationToken cancellationToken)
        {
            var startLineNumber = text.Lines.IndexOf(span.Start);
            var endLineNumber = text.Lines.IndexOf(span.End);

            // NOTE(cyrusn): It's safe to examine the start of a line because you can't have a line
            // with both a pp directive and code on it.  so, for example, if a node crosses a region
            // then it must be the case that the start of some line from the start of the node to
            // the end is hidden.  i.e.:
#if false
'           class C
'           {
'#line hidden
'           }
'#line default
#endif
            // The start of the line with the } on it is hidden, and thus the node overlaps a hidden
            // region.

            for (var lineNumber = startLineNumber; lineNumber <= endLineNumber; lineNumber++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var linePosition = text.Lines[lineNumber].Start;
                var isHidden = isPositionHidden(linePosition, cancellationToken);
                if (isHidden)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool AreOnSameLine(this SourceText text, SyntaxToken token1, SyntaxToken token2)
            => token1.RawKind != 0 &&
               token2.RawKind != 0 &&
               text.AreOnSameLine(token1.Span.End, token2.SpanStart);

        public static bool AreOnSameLine(this SourceText text, int pos1, int pos2)
            => text.Lines.IndexOf(pos1) == text.Lines.IndexOf(pos2);
    }
}