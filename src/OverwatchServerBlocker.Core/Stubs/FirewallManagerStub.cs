using OverwatchServerBlocker.Core.Services;

namespace OverwatchServerBlocker.Core.Stubs;

public class FirewallManagerStub : IFirewallManager
{
    public IFirewallRule[] GetRules()
    {
        throw new NotImplementedException("Current OS is not supported");
    }

    public IFirewallRule AddRule(IFirewallRule rule)
    {
        throw new NotImplementedException("Current OS is not supported");
    }

    public void RemoveRule(IFirewallRule rule)
    {
        throw new NotImplementedException("Current OS is not supported");
    }
}