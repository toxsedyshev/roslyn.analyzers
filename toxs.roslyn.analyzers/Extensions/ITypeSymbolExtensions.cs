using Microsoft.CodeAnalysis;

namespace toxs.roslyn.analyzers.Extensions
{
    internal static partial class ITypeSymbolExtensions
    {
        public static bool IsNullable(this ITypeSymbol symbol)
            => symbol?.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

        public static bool IsNullable(
            this ITypeSymbol symbol,
            out ITypeSymbol underlyingType)
        {
            if (IsNullable(symbol))
            {
                underlyingType = ((INamedTypeSymbol)symbol).TypeArguments[0];
                return true;
            }

            underlyingType = null;
            return false;
        }
    }
}
