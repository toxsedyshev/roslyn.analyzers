namespace toxs.roslyn.analyzers.Extensions
{

    internal static class IntegerUtilities
    {
        public static ulong ToUInt64(object o)
            => o is ulong ? (ulong)o : unchecked((ulong)System.Convert.ToInt64(o));

        public static long ToInt64(object o)
            => o is ulong ul ? unchecked((long)ul) : System.Convert.ToInt64(o);
    }
}
