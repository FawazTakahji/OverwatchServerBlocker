using System;
using System.Collections.Generic;
using System.Linq;
using OverwatchServerBlocker.Core.Services;
using OverwatchServerBlocker.Windows.Extensions;
using WindowsFirewallHelper;
using WindowsFirewallHelper.Collections;
using WindowsFirewallHelper.FirewallRules;
using IFirewallRule = OverwatchServerBlocker.Core.Services.IFirewallRule;

namespace OverwatchServerBlocker.Windows.Services;

public class FirewallManager : IFirewallManager
{
    public IFirewallRule[] GetRules()
    {
        IFirewallWASRulesCollection<FirewallWASRule> rules = FirewallWAS.Instance.Rules;
        List<IFirewallRule> firewallRules = [];
        firewallRules.AddRange(rules.Select(wasRule => new FirewallRule(wasRule)));

        return firewallRules.ToArray();
    }

    public IFirewallRule AddRule(IFirewallRule rule)
    {
        FirewallWASRule wasRule = new(
            rule.Name,
            rule.Action.ToWasAction(),
            rule.Direction.ToWasDirection(),
            FirewallProfiles.Domain | FirewallProfiles.Public | FirewallProfiles.Private)
        {
            RemoteAddresses = rule.RemoteAddresses.ToAddresses(),
            Protocol = rule.Protocol.ToWasProtocol(),
            IsEnable = rule.IsEnabled
        };
        if (rule.ApplicationPath != null)
        {
            wasRule.ApplicationName = rule.ApplicationPath;
        }
        if (rule.Description != null)
        {
            wasRule.Description = rule.Description;
        }
        if (rule.RemotePorts != null)
        {
            wasRule.RemotePorts = rule.RemotePorts.StringToPorts();
        }

        FirewallWAS.Instance.Rules.Add(wasRule);

        return new FirewallRule(wasRule);
    }

    public void RemoveRule(IFirewallRule rule)
    {
        if (rule is not FirewallRule firewallRule)
        {
            throw new NotImplementedException("This type of rule is not supported.");
        }

        FirewallWAS.Instance.Rules.Remove(firewallRule.Rule);
    }
}

public class FirewallRule : IFirewallRule
{
    public readonly FirewallWASRule Rule;

    public FirewallRule(FirewallWASRule rule)
    {
        Rule = rule;
    }

    public bool IsEnabled
    {
        get => Rule.IsEnable;
        set => Rule.IsEnable = value;
    }

    public string? ApplicationPath
    {
        get => Rule.ApplicationName;
        set => Rule.ApplicationName = value;
    }

    public string? Description
    {
        get => Rule.Description;
        set => Rule.Description = value;
    }

    public string Name
    {
        get => Rule.Name;
        set => Rule.Name = value;
    }

    public FirewallRuleAction Action
    {
        get => Rule.Action.ToFirewallRuleAction();
        set => Rule.Action = value.ToWasAction();
    }

    public FirewallRuleDirection Direction
    {
        get => Rule.Direction.ToFirewallRuleDirection();
        set => Rule.Direction = value.ToWasDirection();
    }

    public FirewallRuleProtocol Protocol
    {
        get => Rule.Protocol.ToFirewallRuleProtocol();
        set => Rule.Protocol = value.ToWasProtocol();
    }

    public string[]? RemoteAddresses
    {
        get => Rule.RemoteAddresses.ToStrings();
        set => Rule.RemoteAddresses = value.ToAddresses();
    }

    public string? RemotePorts
    {
        get => Rule.RemotePorts.PortsToString();
        set => Rule.RemotePorts = value.StringToPorts();
    }
}