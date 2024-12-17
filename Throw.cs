using System;
using System.Drawing;
using System.Numerics;
using System.Linq;

// KRAV #:
// 1: Bridge Pattern
// 2: Mönstret separerar abstraktionen (olika kastkrafter: Weak/Strong) från implementationen (kasttekniker: Curve/Straight/Spin)
// 3: Möjliggör oberoende variation av kraft och teknik, vilket ger flexibel kombination av olika kasttyper och strategier

// KRAV #:
// 1: Strategy Pattern
// 2: Vi har skapat olika kasttekniker (Curve/Straight/Spin) som alla följer samma interface (IStrategy), 
//    där varje teknik har sitt eget sätt att beräkna träffar och poäng
// 3: För att spelaren ska kunna välja olika kast under spelets gång

public interface IThrow
{
    IStrategy Strategy { get; }
    string Name { get; }
    string Description { get; }
    int Number { get; }
    int CalculateThrowPower();
    (bool hit, string result) CalculateHit();  
}

public interface IStrategy
{
    string Name { get; }
    int Number { get; }
    (bool hit, string result) ExecuteThrow(int power, double accuracy);  
    int CalculateScore(BowlingLane lane);
}

public class WeakPower : IThrow
{
    public IStrategy Strategy { get; }
    public string Name => "Weak";
    public string Description => "Less power but more control";
    public int Number => 10;
    private const double BASE_ACCURACY = 0.7;  
    public WeakPower(IStrategy strategy)
    {
        Strategy = strategy;
    }

    public int CalculateThrowPower()
    {
        return 40 + new Random().Next(-5, 6);  
    }

    public (bool hit, string result) CalculateHit()
    {
        int power = CalculateThrowPower();
        double accuracy = Math.Min(0.95, BASE_ACCURACY + 0.2);  
        return Strategy.ExecuteThrow(power, accuracy);  
    }
}

public class StrongPower : IThrow
{
    public IStrategy Strategy { get; }
    public string Name => "Strong";
    public string Description => "More power but less control";
    public int Number => 100;
    private const double BASE_ACCURACY = 0.5;  
    public StrongPower(IStrategy strategy)
    {
        Strategy = strategy;
    }

    public int CalculateThrowPower()
    {
        return 80 + new Random().Next(-20, 21);  
    }

    public (bool hit, string result) CalculateHit()
    {
        int power = CalculateThrowPower();
        double accuracy = Math.Max(0.3, BASE_ACCURACY - 0.1); 
        return Strategy.ExecuteThrow(power, accuracy);  
    }
}

public class CurveStrategy : IStrategy
{
    public string Name => "Curve Ball";
    public int Number => 25;

    public (bool hit, string result) ExecuteThrow(int power, double accuracy)
    {
        accuracy = power switch
        {
            < 40 => accuracy * 0.8,  
            < 70 => accuracy * 1.2,  
            _ => accuracy * 0.9     
        };
        
        Random random = new Random();
        bool willHit = random.NextDouble() <= accuracy;
        return (willHit, willHit ? "Controlled curve throw!" : "\u001b[91mThe throw curved too much!\u001b[0m");
    }

    public int CalculateScore(BowlingLane lane)
    {
        var pins = lane.GetPins();
        return 10 - pins.Count;
    }
}

public class StraightStrategy : IStrategy
{
    public string Name => "Straight Shot";
    public int Number => 50;

    public (bool hit, string result) ExecuteThrow(int power, double accuracy)
    {
        accuracy = Math.Min(0.9, power / 100.0);  
        Random random = new Random();
        bool willHit = random.NextDouble() <= accuracy;
        return (willHit, willHit ? "Powerful throw!" : "\u001b[91mThe throw was too uncontrolled!\u001b[0m");
    }

    public int CalculateScore(BowlingLane lane)
    {
        var pins = lane.GetPins();
        return 10 - pins.Count;
    }
}

public class SpinStrategy : IStrategy
{
    public string Name => "Spin Shot";
    public int Number => 75;

    public (bool hit, string result) ExecuteThrow(int power, double accuracy)
    {
        accuracy = power switch
        {
            < 30 => 0.3,  
            < 60 => 0.7,  
            _ => 0.4     
        };
        
        Random random = new Random();
        bool willHit = random.NextDouble() <= accuracy;
        return (willHit, willHit ? "Controlled spin throw!" : "\u001b[91mThe throw spinned too much!\u001b[0m");
    }

    public int CalculateScore(BowlingLane lane)
    {
        var pins = lane.GetPins();
        return 10 - pins.Count;
    }

}

public class ThrowHandler
{
    public int PerformThrow(string name, IThrow power, BowlingLane lane)
    {
        var (hit, result) = power.CalculateHit();
        
        if (!hit)
        {
            Console.WriteLine(result);
            return 0;
        }

        Console.WriteLine(result);
        int pinsDown = lane.MakeThrow(power.Number, power.Strategy.Number);
        int score = power.Strategy.CalculateScore(lane);
        
        Console.WriteLine($"Pins down: {pinsDown}");
        Console.WriteLine($"Score: {score}");
        
        return score;
    }
}