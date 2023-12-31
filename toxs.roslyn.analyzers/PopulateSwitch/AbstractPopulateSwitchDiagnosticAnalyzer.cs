﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using toxs.roslyn.analyzers.Extensions;

namespace toxs.roslyn.analyzers.PopulateSwitch
{
    public abstract class AbstractPopulateSwitchDiagnosticAnalyzer<TSwitchOperation, TSwitchSyntax> :
        DiagnosticAnalyzer
        where TSwitchOperation : IOperation
        where TSwitchSyntax : SyntaxNode
    {
        protected static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.TitleSwitchOnEnumMustHandleAllCases), Resources.ResourceManager, typeof(Resources));
        protected static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.MessageSwitchOnEnumMustHandleAllCases), Resources.ResourceManager, typeof(Resources));

        public abstract DiagnosticDescriptor Rule { get; }

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        #region Interface methods

        protected abstract OperationKind OperationKind { get; }

        protected abstract ICollection<string> GetMissingMembers(TSwitchOperation operation);
        protected abstract bool HasDefaultCase(TSwitchOperation operation);

        protected virtual Location GetDiagnosticLocation(TSwitchSyntax switchBlock)
            => switchBlock.GetLocation();

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterOperationAction(AnalyzeOperation, OperationKind);
        }

        private void AnalyzeOperation(OperationAnalysisContext context)
        {
            var switchOperation = (TSwitchOperation)context.Operation;
            if (!(switchOperation.Syntax is TSwitchSyntax switchBlock))
                return;

            var tree = switchBlock.SyntaxTree;

            if (SwitchIsIncomplete(switchOperation, out var missingCases, out var missingDefaultCase) &&
                !tree.OverlapsHiddenPosition(switchBlock.Span, context.CancellationToken))
            {
                Debug.Assert(missingCases || missingDefaultCase);
                var properties = ImmutableDictionary<string, string>.Empty
                    .Add(PopulateSwitchStatementHelpers.MissingCases, missingCases.ToString())
                    .Add(PopulateSwitchStatementHelpers.MissingDefaultCase, missingDefaultCase.ToString());

#pragma warning disable CS8620 // Mismatch in nullability of 'properties' parameter and argument types - Parameter type for 'properties' has been updated to 'ImmutableDictionary<string, string?>?' in newer version of Microsoft.CodeAnalysis (3.7.x).
                var diagnostic = Diagnostic.Create(
                    Rule,
                    GetDiagnosticLocation(switchBlock),
                    properties: properties,
                    additionalLocations: new[] { switchBlock.GetLocation() });
#pragma warning restore CS8620

                context.ReportDiagnostic(diagnostic);
            }
        }

        #endregion

        protected abstract ITypeSymbol GetParamType(TSwitchOperation switchOperation);

        private bool SwitchIsIncomplete(
            TSwitchOperation operation,
            out bool missingCases, out bool missingDefaultCase)
        {
            var paramType = GetParamType(operation);

            var hasDefaultCase = HasDefaultCase(operation);
            var missingMembers = GetMissingMembers(operation);

            if (paramType.SpecialType == SpecialType.System_Boolean)
            {
                if (missingMembers.Count == 2)
                {
                    missingCases = true;
                    missingDefaultCase = !hasDefaultCase;
                }
                else if (missingMembers.Count == 1)
                {
                    missingCases = !hasDefaultCase;
                    missingDefaultCase = !hasDefaultCase;
                }
                else
                {
                    missingCases = false;
                    missingDefaultCase = false;
                }
            }
            else
            {
                missingCases = paramType.TypeKind == TypeKind.Enum
                    ? missingMembers.Count > 0
                    : false;
                missingDefaultCase = !hasDefaultCase;
            }

            // The switch is incomplete if we're missing any cases or we're missing a default case.
            return missingDefaultCase || missingCases;
        }
    }
}