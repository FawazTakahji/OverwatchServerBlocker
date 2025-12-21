namespace OverwatchServerBlocker.Core.Services;

public interface IFirewallManager
{
    public IFirewallRule[] GetRules();
    public IFirewallRule AddRule(IFirewallRule rule);
    public void RemoveRule(IFirewallRule rule);
}

public class FirewallRule : IFirewallRule
{
    public bool IsEnabled { get; set; }
    public string? ApplicationPath { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; }
    public FirewallRuleAction Action { get; set; }
    public FirewallRuleDirection Direction { get; set; }
    public FirewallRuleProtocol Protocol { get; set; } = FirewallRuleProtocol.Any;
    public string[]? RemoteAddresses { get; set; }
    public string? RemotePorts { get; set; }

    public FirewallRule(string name, FirewallRuleAction action, FirewallRuleDirection direction)
    {
        IsEnabled = true;
        Name = name;
        Action = action;
        Direction = direction;
    }
}

public interface IFirewallRule
{
    public bool IsEnabled { get; set; }
    public string? ApplicationPath { get; set; }
    public string? Description { get; set; }
    public string Name { get; set; }
    public FirewallRuleAction Action { get; set; }
    public FirewallRuleDirection Direction { get; set; }
    public FirewallRuleProtocol Protocol { get; set; }
    public string[]? RemoteAddresses { get; set; }
    public string? RemotePorts { get; set; }
}

public enum FirewallRuleAction
{
    Allow,
    Block
}

public enum FirewallRuleDirection
{
    Inbound,
    Outbound
}

public enum FirewallRuleProtocol
{
    Any,
    GRE,
    HOPOPT,
    ICMPv4,
    ICMPv6,
    IGMP,
    IPv6,
    IPv6Frag,
    IPv6NoNxt,
    IPv6Opts,
    IPv6Route,
    L2TP,
    PGM,
    TCP,
    UDP,
    VRRP
}