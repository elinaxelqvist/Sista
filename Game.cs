using System.Text.Json;
public class Game
{
    private readonly Score playerScore = new();
    private readonly Score computerScore = new();
    private readonly GameStatistics<GameHistory> gameStats;
    private BowlingLane lane;
    private readonly Player player;
    private readonly ComputerPlayer computerPlayer;

    public Game()
    {
        Console.WriteLine("Welcome to Bowling Game!");
        Console.Write("Enter your name: ");
        string playerName = Console.ReadLine() ?? "Player";

        lane = new BowlingLane();
        IStrategy defaultStrategy = new SpinStrategy();
        IThrow defaultPower = new WeakPower(defaultStrategy);
        player = new Player(playerName, defaultPower);
        computerPlayer = new ComputerPlayer("Computer", defaultPower);
        gameStats = new GameStatistics<GameHistory>();
    }

    public void PlayGame()
    {
        for (int round = 1; round <= 5; round++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nROUND {round}");
            Console.ResetColor();

            lane = new BowlingLane();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n=== {player.Name}'s Turn ===");
            Console.ResetColor();
            lane.Print();

            for (int i = 0; i < 2; i++)
            {
      
                Console.WriteLine("\nChoose your strategy:");
                Console.WriteLine("1. Curve Strategy ");
                Console.WriteLine("2. Straight Strategy");
                Console.WriteLine("3. Spin Strategy");
                Console.Write("Input (1-3): ");

                int strategyChoice;
                while (!int.TryParse(Console.ReadLine(), out strategyChoice) || strategyChoice < 1 || strategyChoice > 3)
                {
                    Console.WriteLine("Invalid input. Please enter 1, 2 or 3:");
                }

       
                Console.WriteLine("\nChoose your power:");
                Console.WriteLine("1. Weak");
                Console.WriteLine("2. Strong");
                Console.Write("Input (1-2): ");

                int powerChoice;
                while (!int.TryParse(Console.ReadLine(), out powerChoice) || powerChoice < 1 || powerChoice > 2)
                {
                    Console.WriteLine("Invalid input. Please enter 1 or 2:");
                }

                IStrategy strategy = strategyChoice switch
                {
                    1 => new CurveStrategy(),
                    2 => new  StraightStrategy(),
                    _ => new SpinStrategy()
                };

                IThrow power = powerChoice == 1
                    ? new WeakPower(strategy)
                    : new StrongPower(strategy);

                player.UpdateThrowSettings(power);
                int playerThrowScore = player.PerformThrow(lane);
                playerScore.AddPoints(playerThrowScore);

                if (lane.AllPinsDown())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"All pins are down!");
                    Console.ResetColor();
                    break;
                }
            }

            lane = new BowlingLane();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"\n=== {computerPlayer.Name}'s Turn ===");
            Console.ResetColor();
            Console.WriteLine();

            for (int i = 0; i < 2; i++)
            {
                Thread.Sleep(1000);
                int computerThrowScore = computerPlayer.PerformThrow(lane);
                computerScore.AddPoints(computerThrowScore);

                if (lane.AllPinsDown())
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"All pins are down!");
                    Console.ResetColor();
                    break;
                }
            }
        }

        Console.WriteLine("\nGame over! 5 rounds completed.");
        var (winner, playerTotal, computerTotal) = AnalyzeScores(playerScore, computerScore);

        ShowRoundHistory();

        if (winner == "Tie")
        {
            Console.WriteLine("\nIt's a tie!");
        }
        else
        {
            Console.WriteLine($"\n{winner} wins with {(winner == player.Name ? playerTotal : computerTotal)} points!");
            Console.WriteLine($"Winning margin: {Math.Abs(playerTotal - computerTotal)} points");
        }

    
        gameStats.AddData(new GameHistory
        {
            PlayerName = player.Name,
            Score = playerTotal,
        });

        gameStats.AddData(new GameHistory
        {
            PlayerName = computerPlayer.Name,
            Score = computerTotal,
        });

        Console.WriteLine("\nPress Enter to see statistics...");
        Console.ReadLine();
        Console.Clear();

        gameStats.ShowStatistics();
    }

    private (string winner, int playerTotal, int computerTotal) AnalyzeScores(Score playerScore, Score computerScore)
    {
        int playerTotal = playerScore.GetTotalScore();
        int computerTotal = computerScore.GetTotalScore();

        string winner = playerTotal > computerTotal
            ? player.Name
            : playerTotal < computerTotal
                ? "Computer"
                : "Tie";

        return (winner, playerTotal, computerTotal);
    }

    private void ShowRoundHistory()
    {
        Console.WriteLine($"\n{player.Name}'s throw history:");
        int throwNumber = 1;
        foreach (int score in playerScore)
        {
            Console.WriteLine($"Throw {throwNumber++}: {score} points");
        }

        Console.WriteLine($"\n{computerPlayer.Name}'s throw history:");
        throwNumber = 1;
        foreach (int score in computerScore)
        {
            Console.WriteLine($"Throw {throwNumber++}: {score} points");
        }
    }
}