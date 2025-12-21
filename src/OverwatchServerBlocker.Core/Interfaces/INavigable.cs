namespace OverwatchServerBlocker.Core.Interfaces;

public interface INavigable
{
    public string Route { get; }

    public void OnNavigatingAway()
    {

    }
}