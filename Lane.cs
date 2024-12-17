//Här har vi fått ideér av LLM för att skapa en spelplan med koordinater. 
//Vår första tanke var att spelaren skulle ha en pointer och att spelplanen inte skulle ha koordintaer. 
public class BowlingLane
{
    private List<Coordinate> pins;

    public BowlingLane()
    {
        ResetPins();
    }

    private void ResetPins()
    {
        pins = new List<Coordinate>
        {
            new Coordinate(0, 0), new Coordinate(0, 1), new Coordinate(0, 2), new Coordinate(0, 3),
            new Coordinate(1, 0), new Coordinate(1, 1), new Coordinate(1, 2),
            new Coordinate(2, 0), new Coordinate(2, 1),
            new Coordinate(3, 0)
        };
    }

    public void Print(Score score = null)
    {
        Console.WriteLine("\nBowling Lane:");

        Console.Write(" ");
        for (int j = 0; j < 4; j++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($" {j}");
            Console.ResetColor();
        }
        Console.WriteLine();

        for (int i = 0; i < 4; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{i} ");
            Console.ResetColor();

            Console.Write(new string(' ', i));

            int pinsInRow = 4 - i;

            for (int j = 0; j < pinsInRow; j++)
            {
                if (pins.Contains(new Coordinate(i, j)))
                    Console.Write("X ");
                else
                    Console.Write("O ");
            }
            Console.WriteLine();
        }
    }

    public int MakeThrow(int power, int direction)
    {
        int pinsDownCount = 0;
        int startColumn, endColumn;

        
        if (direction <= 25)      
        {
            startColumn = 0;
            endColumn = 1;  
        }
        else if (direction <= 75) 
        {
            startColumn = 1;
            endColumn = 2;  

            
            Random random = new Random();
            if (random.Next(1, 101) <= 50)  
            {
                startColumn = 0;
                endColumn = 3;  
            }
        }
        else                       
        {
            startColumn = 2;
            endColumn = 3;  
        }

        
        int[] rowsToAffect;
        if (power <= 40) 
        {
            rowsToAffect = new int[] { 2, 3 };  
        }
        else 
        {
            rowsToAffect = new int[] { 0, 1, 2, 3 }; 
        }

        Console.WriteLine($"Result:");
        foreach (int row in rowsToAffect)
        {
            for (int col = startColumn; col <= endColumn; col++)
            {
                Coordinate pin = new Coordinate(row, col);
                if (pins.Contains(pin))
                {
                    pins.Remove(pin);
                    pinsDownCount++;
                    Console.WriteLine($"Hit pin at column {col}, row {row}");
                }
            }
        }

        Print();
        return pinsDownCount;
    }

    public bool AllPinsDown()
    {
        return pins.Count == 0;
    }

    public List<Coordinate> GetPins()
    {
        return pins;
    }

    public struct Coordinate
    {
        public int X { get; }
        public int Y { get; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}