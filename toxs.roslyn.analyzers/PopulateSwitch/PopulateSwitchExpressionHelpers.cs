﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using toxs.roslyn.analyzers.Extensions;

namespace toxs.roslyn.analyzers.PopulateSwitch
{
    internal static class PopulateSwitchExpressionHelpers
    {
        public static ICollection<ISymbol> GetMissingEnumMembers(ISwitchExpressionOperation operation)
        {
            var switchExpression = operation.Value;
            var switchExpressionType = switchExpression?.Type;

            // Check if the type of the expression is a nullable INamedTypeSymbol
            // if the type is both nullable and an INamedTypeSymbol extract the type argument from the nullable
            // and check if it is of enum type
            if (switchExpressionType != null)
                switchExpressionType = switchExpressionType.IsNullable(out var underlyingType) ? underlyingType : switchExpressionType;

            if (switchExpressionType?.TypeKind == TypeKind.Enum)
            {
                var enumMembers = new Dictionary<long, ISymbol>();
                if (PopulateSwitchStatementHelpers.TryGetAllEnumMembers(switchExpressionType, enumMembers))
                {
                    RemoveExistingEnumMembers(operation, enumMembers);
                    return enumMembers.Values;
                }
            }

            return Array.Empty<ISymbol>();
        }

        private static void RemoveExistingEnumMembers(
            ISwitchExpressionOperation operation, Dictionary<long, ISymbol> enumMembers)
        {
            foreach (var arm in operation.Arms)
            {
                RemoveIfConstantPatternHasValue(arm.Pattern, enumMembers);
                if (arm.Pattern is IBinaryPatternOperation binaryPattern)
                {
                    HandleBinaryPattern(binaryPattern, enumMembers);
                }
            }
        }

        private static void HandleBinaryPattern(IBinaryPatternOperation binaryPattern, Dictionary<long, ISymbol> enumMembers)
        {
            if (binaryPattern?.OperatorKind == BinaryOperatorKind.Or)
            {
                RemoveIfConstantPatternHasValue(binaryPattern.LeftPattern, enumMembers);
                RemoveIfConstantPatternHasValue(binaryPattern.RightPattern, enumMembers);

                HandleBinaryPattern(binaryPattern.LeftPattern as IBinaryPatternOperation, enumMembers);
                HandleBinaryPattern(binaryPattern.RightPattern as IBinaryPatternOperation, enumMembers);
            }
        }

        private static void RemoveIfConstantPatternHasValue(IOperation operation, Dictionary<long, ISymbol> enumMembers)
        {
            if (operation is IConstantPatternOperation o && o.Value?.ConstantValue.HasValue == true)
            {
                enumMembers.Remove(IntegerUtilities.ToInt64(o.Value.ConstantValue.Value));
            }
        }

        public static bool HasDefaultCase(ISwitchExpressionOperation operation)
            => operation.Arms.Any(a => IsDefault(a));

        public static bool IsDefault(ISwitchExpressionArmOperation arm)
        {
            if (arm.Pattern.Kind == OperationKind.DiscardPattern)
                return true;

            if (arm.Pattern is IDeclarationPatternOperation declarationPattern)
                return declarationPattern.MatchesNull;

            return false;
        }
    }
}