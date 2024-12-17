using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public interface IGameData
{
    string PlayerName { get; }
}

public class GameHistory : IGameData
{
    public string PlayerName { get; set; }
    public int Score { get; set; }
}

public class GameStatistics<T> where T : IGameData
{
    private List<T> statistics = new();
    private readonly string filePath;
    private readonly string fileFormat;
    private readonly string gamesPlayedPath;

    public GameStatistics(string format = "json")
    {
        fileFormat = format.ToLower();
        filePath = $"gamehistory.{fileFormat}";
        gamesPlayedPath = "gamesplayed.json";
        LoadStatistics();
    }

      // KRAV #:
      // 1: Generics
      // 2: Konceptet används genom en generisk klass 
      // 3: Generics används för att kunna ha olika filformat. Filformaten används för att spara och läsa statistik


    private Dictionary<string, int> LoadGamesPlayed()
    {
        if (!File.Exists(gamesPlayedPath))
            return new Dictionary<string, int>();

        string json = File.ReadAllText(gamesPlayedPath);
        return JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new Dictionary<string, int>();
    }

    private void SaveGamesPlayed(Dictionary<string, int> gamesPlayed)
    {
        string json = JsonSerializer.Serialize(gamesPlayed, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(gamesPlayedPath, json);
    }

    private void LoadStatistics()
    {
        if (!File.Exists(filePath))
            return;

        string data = File.ReadAllText(filePath);
        statistics = fileFormat switch
        {
            "json" => JsonSerializer.Deserialize<List<T>>(data) ?? new List<T>(),
            "csv" => ParseCsv(data),
            _ => new List<T>()
        };
    }

    private void SaveStatistics()
    {
        string data = fileFormat switch
        {
            "json" => JsonSerializer.Serialize(statistics, new JsonSerializerOptions { WriteIndented = true }),
            "csv" => ToCsv(),
            _ => string.Empty
        };

        File.WriteAllText(filePath, data);
    }

    private List<T> ParseCsv(string csvData)
    {
        var results = new List<T>();
        var lines = csvData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines.Skip(1)) 
        {
            var values = line.Split(',');
            if (values.Length >= 2 && typeof(T) == typeof(GameHistory))
            {
                results.Add((T)(IGameData)new GameHistory 
                { 
                    PlayerName = values[0], 
                    Score = int.Parse(values[1]) 
                });
            }
        }
        
        return results;
    }

    private string ToCsv()
    {
        if (!statistics.Any())
            return "PlayerName,Score";

        return "PlayerName,Score\n" + string.Join("\n", 
            statistics
                .Where(s => s is GameHistory)  
                .Select(s => $"{s.PlayerName},{((GameHistory)(object)s).Score}")); 
    }

    public void AddData(T data)
    {
        if (typeof(T) == typeof(GameHistory))
        {
            statistics.Add(data);
            
            statistics = statistics
                .OrderByDescending(s => ((GameHistory)(object)s).Score)
                .Take(3)
                .AsEnumerable()  
                .ToList();  
            
            SaveStatistics();

            var gamesPlayed = LoadGamesPlayed();
            string playerName = data.PlayerName;
            if (!gamesPlayed.ContainsKey(playerName))
                gamesPlayed[playerName] = 0;
            gamesPlayed[playerName]++;
            SaveGamesPlayed(gamesPlayed);
        }
    }

    // KRAV #:
    // 1: LINQ
    // 2: LINQ används genom implementationen OrderByDescending samt COUNT
    // 3: Vi använder LINQ för att skapa en high score lista, där vi tar fram topp 3 samt antalet gånger en spelare har spelat. 
    //Detta blir menginsfullt efetrsom att koden blir mer simpel än om vi inte hade använt LINQ
    public void ShowStatistics()
    {
        if (typeof(T) == typeof(GameHistory))
        {
            Console.WriteLine($"\nTOP 3 SCORES OF ALL TIME (Players & Computer)");
            Console.WriteLine("----------------------------------------");
            int rank = 1;
            foreach (var score in statistics)
            {
                var history = (GameHistory)(object)score;
                Console.WriteLine($"{rank++}. {history.PlayerName}: {history.Score} points");
            }

            Console.WriteLine("\nEnter player name to see how many games they have played:");
            string playerName = Console.ReadLine();
            int gamesPlayed = GetGamesPlayed(playerName);
            if (gamesPlayed > 0)
            {
                Console.WriteLine($"{playerName} has played {gamesPlayed} games");
            }
            else
            {
                Console.WriteLine($"No games found for player: {playerName}");
            }
        }
    }

    public int GetGamesPlayed(string playerName)
    {
        var gamesPlayed = LoadGamesPlayed();
        return gamesPlayed.TryGetValue(playerName, out int count) ? count : 0;
    }
}