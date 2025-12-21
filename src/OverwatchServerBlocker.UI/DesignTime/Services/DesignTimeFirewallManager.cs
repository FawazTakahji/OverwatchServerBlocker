using System;
using OverwatchServerBlocker.Core.Services;

namespace OverwatchServerBlocker.UI.DesignTime.Services;

public class DesignTimeFirewallManager : IFirewallManager
{
    public IFirewallRule[] GetRules()
    {
        throw new NotImplementedException();
    }

    public IFirewallRule AddRule(IFirewallRule rule)
    {
        throw new NotImplementedException();
    }

    public void RemoveRule(IFirewallRule rule)
    {
        throw new NotImplementedException();
    }
}