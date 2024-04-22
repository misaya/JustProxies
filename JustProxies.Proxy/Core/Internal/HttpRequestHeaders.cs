using System.Net.Http.Headers;

namespace JustProxies.Proxy.Core.Internal;

public class HttpRequestHeaders : HttpHeaders
{
    public bool IsExist(string headerString)
    {
        var key = headerString;
        if (headerString.Contains(':'))
        {
            key = headerString.Split(':')[0];
            var value = headerString.Split(':')[1];
            return this.Any(p => p.Key == key && p.Value.Contains(value));
        }

        return Contains(key);
    }

    public void Add(string headerLine)
    {
        var headerSplit = headerLine.Split([':'], 2);
        if (headerSplit.Length == 2)
        {
            if (headerSplit[1].Contains(';')) TryAddWithoutValidation(headerSplit[0], headerSplit[1].Split(';'));

            TryAddWithoutValidation(headerSplit[0], headerSplit[1]);
        }
    }

    public bool HasKeys()
    {
        return this.Any();
    }
}