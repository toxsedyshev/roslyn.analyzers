using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace toxs.roslyn.analyzers.Extensions
{
    internal static class BooleanExtensions
    {
        public static Dictionary<long, string> CreateBooleanOptions()
            => new Dictionary<long, string>
            {
                { 0, "false" },
                { 1, "true" },
            };
    }
}
