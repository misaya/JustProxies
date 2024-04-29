using System.Runtime.InteropServices.JavaScript;
using System.Text.Json.Nodes;

namespace JustProxies.Common;

public static class JsonExt
{
    public static string ToJson(this object obj)
    {
        return System.Text.Json.JsonSerializer.Serialize(obj);
    }

    public static T? ToObject<T>(this string json)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(json);
    }

    public static bool IsJson(this string json)
    {
        try
        {
            JsonNode.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}