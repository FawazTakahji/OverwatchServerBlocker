using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using OverwatchServerBlocker.Core.Services;
using WindowsFirewallHelper;
using WindowsFirewallHelper.Addresses;

namespace OverwatchServerBlocker.Windows.Extensions;

public static class FirewallExtensions
{
    public static FirewallRuleDirection ToFirewallRuleDirection(this FirewallDirection direction)
    {
        return direction == FirewallDirection.Inbound
            ? FirewallRuleDirection.Inbound
            : FirewallRuleDirection.Outbound;
    }

    public static FirewallRuleAction ToFirewallRuleAction(this FirewallAction action)
    {
        return action == FirewallAction.Allow
            ? FirewallRuleAction.Allow
            : FirewallRuleAction.Block;
    }

    public static FirewallRuleProtocol ToFirewallRuleProtocol(this FirewallProtocol protocol)
    {
        if (protocol == FirewallProtocol.Any)
        {
            return FirewallRuleProtocol.Any;
        }
        if (protocol == FirewallProtocol.GRE)
        {
            return FirewallRuleProtocol.GRE;
        }
        if (protocol == FirewallProtocol.HOPOPT)
        {
            return FirewallRuleProtocol.HOPOPT;
        }
        if (protocol == FirewallProtocol.ICMPv4)
        {
            return FirewallRuleProtocol.ICMPv4;
        }
        if (protocol == FirewallProtocol.ICMPv6)
        {
            return FirewallRuleProtocol.ICMPv6;
        }
        if (protocol == FirewallProtocol.IGMP)
        {
            return FirewallRuleProtocol.IGMP;
        }
        if (protocol == FirewallProtocol.IPv6)
        {
            return FirewallRuleProtocol.IPv6;
        }
        if (protocol == FirewallProtocol.IPv6Frag)
        {
            return FirewallRuleProtocol.IPv6Frag;
        }
        if (protocol == FirewallProtocol.IPv6NoNxt)
        {
            return FirewallRuleProtocol.IPv6NoNxt;
        }
        if (protocol == FirewallProtocol.IPv6Opts)
        {
            return FirewallRuleProtocol.IPv6Opts;
        }
        if (protocol == FirewallProtocol.IPv6Route)
        {
            return FirewallRuleProtocol.IPv6Route;
        }
        if (protocol == FirewallProtocol.L2TP)
        {
            return FirewallRuleProtocol.L2TP;
        }
        if (protocol == FirewallProtocol.PGM)
        {
            return FirewallRuleProtocol.PGM;
        }
        if (protocol == FirewallProtocol.TCP)
        {
            return FirewallRuleProtocol.TCP;
        }
        if (protocol == FirewallProtocol.UDP)
        {
            return FirewallRuleProtocol.UDP;
        }
        if (protocol == FirewallProtocol.VRRP)
        {
            return FirewallRuleProtocol.VRRP;
        }

        // This should never happen
        throw new ArgumentException("Unknown protocol");
    }

    public static string[]? ToStrings(this IAddress[] addresses)
    {
        if (addresses.Length == 1 && addresses.First().ToString() == "*")
        {
            return null;
        }

        List<string> networks = [];
        foreach (IAddress address in addresses)
        {
            string addressString = address.ToString();
            if (address is NetworkAddress && IPNetwork2.TryParse(addressString, out IPNetwork2 ipNetwork))
            {
                networks.Add(ipNetwork.ToString());
            }
            else
            {
                networks.Add(addressString);
            }
        }

        return networks.ToArray();
    }

    public static string PortsToString(this ushort[] ports)
    {
        var portStrings = ports
            .Distinct()
            .OrderBy(port => port)
            .Select((port, index) => new {PortNumber = port, GroupId = port - index})
            .GroupBy(pair => pair.GroupId)
            .Select(
                groups => groups.Count() >= 3
                    ? groups.First().PortNumber + "-" + groups.Last().PortNumber
                    : string.Join(",", groups.Select(pair => pair.PortNumber.ToString("")).ToArray())
            )
            .ToArray();

        return string.Join(",", portStrings);
    }

    public static FirewallDirection ToWasDirection(this FirewallRuleDirection direction)
    {
        return direction == FirewallRuleDirection.Inbound
            ? FirewallDirection.Inbound
            : FirewallDirection.Outbound;
    }

    public static FirewallAction ToWasAction(this FirewallRuleAction action)
    {
        return action == FirewallRuleAction.Allow
            ? FirewallAction.Allow
            : FirewallAction.Block;
    }

    public static FirewallProtocol ToWasProtocol(this FirewallRuleProtocol protocol)
    {
        return protocol switch
        {
            FirewallRuleProtocol.Any => FirewallProtocol.Any,
            FirewallRuleProtocol.GRE => FirewallProtocol.GRE,
            FirewallRuleProtocol.HOPOPT => FirewallProtocol.HOPOPT,
            FirewallRuleProtocol.ICMPv4 => FirewallProtocol.ICMPv4,
            FirewallRuleProtocol.ICMPv6 => FirewallProtocol.ICMPv6,
            FirewallRuleProtocol.IGMP => FirewallProtocol.IGMP,
            FirewallRuleProtocol.IPv6 => FirewallProtocol.IPv6,
            FirewallRuleProtocol.IPv6Frag => FirewallProtocol.IPv6Frag,
            FirewallRuleProtocol.IPv6NoNxt => FirewallProtocol.IPv6NoNxt,
            FirewallRuleProtocol.IPv6Opts => FirewallProtocol.IPv6Opts,
            FirewallRuleProtocol.IPv6Route => FirewallProtocol.IPv6Route,
            FirewallRuleProtocol.L2TP => FirewallProtocol.L2TP,
            FirewallRuleProtocol.PGM => FirewallProtocol.PGM,
            FirewallRuleProtocol.TCP => FirewallProtocol.TCP,
            FirewallRuleProtocol.UDP => FirewallProtocol.UDP,
            FirewallRuleProtocol.VRRP => FirewallProtocol.VRRP,
            // This should never happen
            _ => throw new ArgumentException("Unknown protocol")
        };
    }

    public static IAddress[] ToAddresses(this string[]? networks)
    {
        if (networks is null)
        {
            return [new NetworkAddress(IPAddress.Any)];
        }

        List<IAddress> addresses = [];
        foreach (string network in networks)
        {
            addresses.Add(new Address(network));
        }

        return addresses.ToArray();
    }

    public static ushort[] StringToPorts(this string? str)
    {
        if (string.IsNullOrEmpty(str?.Trim()))
        {
            return [];
        }

        return str.Trim().Split(',')
            .SelectMany(port =>
                {
                    var portParts = port.Trim().Split('-');

                    if (portParts.Length == 2 &&
                        ushort.TryParse(portParts[0].Trim(), out var start) &&
                        ushort.TryParse(portParts[1].Trim(), out var end))
                    {
                        return Enumerable.Range(start, end - start + 1).Select(p => (ushort)p);
                    }

                    if (portParts.Length == 1 && ushort.TryParse(port.Trim(), out var portNumber))
                    {
                        return [portNumber];
                    }

                    return [];
                }
            )
            .Distinct()
            .OrderBy(port => port)
            .ToArray();
    }
}