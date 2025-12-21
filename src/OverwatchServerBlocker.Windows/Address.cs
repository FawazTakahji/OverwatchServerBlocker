using WindowsFirewallHelper;

namespace OverwatchServerBlocker.Windows;

public class Address : IAddress
{
    private readonly string _address;

    public Address(string address)
    {
        _address = address;
    }

    public override string ToString()
    {
        return _address;
    }
}