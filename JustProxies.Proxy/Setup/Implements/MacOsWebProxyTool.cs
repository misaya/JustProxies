using System.Diagnostics;
using JustProxies.Proxy.Setup.Core;

namespace JustProxies.Proxy.Setup.Implements;

public class MacOsWebProxyTool(string network) : IWebProxyToolImpl
{
    public WebProxyInfo GetWebProxy()
    {
        WebProxyInfo result = new();
        var psi = new ProcessStartInfo
        {
            FileName = "/usr/sbin/networksetup",
            Arguments = $"-getwebProxy {network}",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(psi);
        process?.WaitForExit();

        var output = process?.StandardOutput?.ReadToEnd();
        var lines = output?.Split('\n');

        if (lines == null) return null!;

        result.NetworkService = network;
        foreach (var line in lines)
        {
            if (line.StartsWith("Enabled:")) result.IsEnabled = line.EndsWith("Yes");

            if (line.StartsWith("Server:")) result.Host = line.Split(':')[1].Trim();

            if (line.StartsWith("Port:")) result.Port = line.Split(':')[1].Trim();
        }

        return result;
    }

    public bool SetWebProxy(string address, int port)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "/usr/sbin/networksetup",
            Arguments = $"-setwebProxy {network} {address} {port}",
            Verb = "runas",
            RedirectStandardOutput = false,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(psi);
        process?.WaitForExit();
        if (process != null && process.ExitCode != 0) return false;

        var info = GetWebProxy();
        return info.Host == address && info.Port == port.ToString();
    }

    public bool EnableProxy()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "/usr/sbin/networksetup",
            Arguments = $"-setwebproxystate {network} on",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(psi);
        process?.WaitForExit();

        if (process != null && process.ExitCode != 0) return false;

        var info = GetWebProxy();
        return info.IsEnabled;
    }

    public bool DisableProxy()
    {
        var psi = new ProcessStartInfo
        {
            FileName = "/usr/sbin/networksetup",
            Arguments = $"-setwebproxystate {network} off",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = Process.Start(psi);
        process?.WaitForExit();

        if (process != null && process.ExitCode != 0) return false;

        var info = GetWebProxy();
        return !info.IsEnabled;
    }
}