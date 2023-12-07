namespace toxs.roslyn.analyzers.Enum
{
    public static class EnumDiagnosticIdentifiers
    {
        private const string EnumPrefix = "TENUM";

        public const string SwitchOnEnumMustHandleAllCases = EnumPrefix + "0001";
        public const string SwitchOnEnumExpressionMustHandleAllCases = EnumPrefix + "0011";
    }
}
