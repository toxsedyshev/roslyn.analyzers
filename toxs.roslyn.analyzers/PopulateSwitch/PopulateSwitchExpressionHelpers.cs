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
        public static ICollection<string> GetMissingMembers(ISwitchExpressionOperation operation)
        {
            var switchExpression = operation.Value;
            var switchExpressionType = switchExpression?.Type;

            // Check if the type of the expression is a nullable INamedTypeSymbol
            // if the type is both nullable and an INamedTypeSymbol extract the type argument from the nullable
            // and check if it is of enum type
            if (switchExpressionType != null)
            {
                switchExpressionType = switchExpressionType.IsNullable(out var underlyingType) ? underlyingType : switchExpressionType;
            }

            if (switchExpressionType?.TypeKind == TypeKind.Enum)
            {
                var enumMembers = new Dictionary<long, string>();
                if (PopulateSwitchStatementHelpers.TryGetAllEnumMembers(switchExpressionType, enumMembers))
                {
                    RemoveExistingEnumMembers(operation, enumMembers);
                    return enumMembers.Values;
                }
            }

            if (switchExpressionType?.SpecialType == SpecialType.System_Boolean)
            {
                var enumMembers = BooleanExtensions.CreateBooleanOptions();
                RemoveExistingEnumMembers(operation, enumMembers);
                return enumMembers.Values;
            }

            return Array.Empty<string>();
        }

        private static void RemoveExistingEnumMembers(
            ISwitchExpressionOperation operation, Dictionary<long, string> enumMembers)
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

        private static void HandleBinaryPattern(IBinaryPatternOperation binaryPattern, Dictionary<long, string> enumMembers)
        {
            if (binaryPattern?.OperatorKind == BinaryOperatorKind.Or)
            {
                RemoveIfConstantPatternHasValue(binaryPattern.LeftPattern, enumMembers);
                RemoveIfConstantPatternHasValue(binaryPattern.RightPattern, enumMembers);

                HandleBinaryPattern(binaryPattern.LeftPattern as IBinaryPatternOperation, enumMembers);
                HandleBinaryPattern(binaryPattern.RightPattern as IBinaryPatternOperation, enumMembers);
            }
        }

        private static void RemoveIfConstantPatternHasValue(IOperation operation, Dictionary<long, string> enumMembers)
        {
            if (operation is IConstantPatternOperation o && o.Value?.ConstantValue.HasValue == true)
            {
                enumMembers.Remove(IntegerUtilities.ToInt64(o.Value.ConstantValue.Value));
            }
        }

        public static bool HasDefaultCase(ISwitchExpressionOperation operation)
            => operation.Arms.Any(a => IsDefault(a));

        public static bool IsDefault(this ISwitchExpressionArmOperation arm)
            => arm.Pattern.IsDefault();

        public static bool IsDefault(this IPatternOperation pattern)
        {
            // _ => ...
            if (pattern is IDiscardPatternOperation)
            {
                return true;
            }

            // var v => ...
            if (pattern is IDeclarationPatternOperation declarationPattern)
            {
                return declarationPattern.MatchesNull;
            }

            if (pattern is IBinaryPatternOperation binaryPattern)
            {
                if (binaryPattern.OperatorKind == BinaryOperatorKind.Or)
                {
                    // x or _ => ...
                    return IsDefault(binaryPattern.LeftPattern) || IsDefault(binaryPattern.RightPattern);
                }

                if (binaryPattern.OperatorKind == BinaryOperatorKind.And)
                {
                    // _ and var x => ...
                    return IsDefault(binaryPattern.LeftPattern) && IsDefault(binaryPattern.RightPattern);
                };

                return false;
            }

            return false;
        }
    }
}