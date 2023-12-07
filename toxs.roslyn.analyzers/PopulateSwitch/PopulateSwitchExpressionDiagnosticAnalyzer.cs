// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using toxs.roslyn.analyzers.Enum;

namespace toxs.roslyn.analyzers.PopulateSwitch
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PopulateSwitchExpressionDiagnosticAnalyzer
        : PopulateSwitchExpressionDiagnosticAnalyzer<SwitchExpressionSyntax>
    {
    }

    public abstract class PopulateSwitchExpressionDiagnosticAnalyzer<TSwitchSyntax> :
        AbstractPopulateSwitchDiagnosticAnalyzer<ISwitchExpressionOperation, TSwitchSyntax>
        where TSwitchSyntax : SyntaxNode
    {
        public const string DiagnosticId = EnumDiagnosticIdentifiers.SwitchOnEnumExpressionMustHandleAllCases;

        public override DiagnosticDescriptor Rule { get; } = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            DiagnosticCategories.Maintainability,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);


        protected sealed override OperationKind OperationKind => OperationKind.SwitchExpression;

        protected sealed override ICollection<ISymbol> GetMissingEnumMembers(ISwitchExpressionOperation operation)
            => PopulateSwitchExpressionHelpers.GetMissingEnumMembers(operation);

        protected sealed override bool HasDefaultCase(ISwitchExpressionOperation operation)
            => PopulateSwitchExpressionHelpers.HasDefaultCase(operation);
    }
}