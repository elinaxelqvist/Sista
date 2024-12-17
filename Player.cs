public class Player
{
    private readonly ThrowHandler throwHandler = new();
    public string Name { get; }
    public IThrow PowerType { get; private set; }

    public Player(string playerName, IThrow powerType)
    {
        Name = playerName;
        PowerType = powerType;
    }

    public void UpdateThrowSettings(IThrow powerType)
    {
        PowerType = powerType;
    }

    public int PerformThrow(BowlingLane lane)
    {
        return throwHandler.PerformThrow(Name, PowerType, lane);
    }
}