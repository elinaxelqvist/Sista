using System.Runtime.CompilerServices;
public class ComputerPlayer
{
    private readonly ThrowHandler throwHandler = new();
    private readonly Random random = new();
    public string Name { get; }
    public IThrow PowerType { get; private set; }

    public ComputerPlayer(string name, IThrow powerType)
    {
        Name = name;
        PowerType = powerType;
    }

    public void UpdateThrowSettings(IThrow powerType)
    {
        PowerType = powerType;
    }
    
    //Här har vi fått ideér av LLM för att avancera datorns spellogik.
    public int PerformThrow(BowlingLane lane)
    {
        var pins = lane.GetPins();
        
        int leftPins = pins.Count(p => p.X < 2);
        int rightPins = pins.Count(p => p.X > 1);
        int backPins = pins.Count(p => p.Y == 3);
        
        IStrategy strategy = (leftPins, rightPins, backPins) switch
        {
            var (l, r, b) when b >= 2 => new  StraightStrategy(),      
            var (l, r, _) when l > r => new CurveStrategy(),   
            var (l, r, _) when r > l => new SpinStrategy(),   
            _ => new SpinStrategy()                               
        };

    
        PowerType = pins.Count() <= 5 
            ? new WeakPower(strategy)    
            : new StrongPower(strategy); 

        return throwHandler.PerformThrow(Name, PowerType, lane);
    }
}
