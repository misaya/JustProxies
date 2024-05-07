namespace JustProxies.Common;

public static class NullOrEmptyExt
{
    public static bool IsNull<T>(this T obj)
    {
        return obj == null;
    }

    public static bool IsNotNull<T>(this T obj)
    {
        return obj != null;
    }

    public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.IsNull() || enumerable.Count() == 0;
    }

    public static bool IsNotEmpty<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.IsNotNull() && enumerable.Any();
    }
}