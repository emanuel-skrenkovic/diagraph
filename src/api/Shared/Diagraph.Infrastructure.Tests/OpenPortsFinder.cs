using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.NetworkInformation;

namespace Diagraph.Infrastructure.Tests;

internal static class OpenPortsFinder
{
    // The value is unused, only need the key, but C# does not have a 
    // ConcurrentHashSet implementation.
    private static readonly ConcurrentDictionary<int, byte> Ports = new();

    internal static bool TryGetPort(Range range, out int freePort)
    {
        int start = range.Start.Value;
        int end   = range.End.Value;
        
        for (int port = start; port < end; port++)
        {
            if (CheckPort(port))
            {
                freePort = port;
                return true;
            }
        }

        freePort = 0;
        return false;
    }

    private static bool CheckPort(int port)
    {
        if (Ports.ContainsKey(port))           return false;
        
        IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
        TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();
        
        if (tcpConnInfoArray.Select(i => i.LocalEndPoint.Port).Contains(port)) return false; 
        if (!Ports.TryAdd(port, Byte.MinValue)) return false; // TODO: redundant?

        return true;
    }
}