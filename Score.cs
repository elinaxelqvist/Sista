using System.Collections.Generic;
using System.Linq;

public class Score : IEnumerable<int>
{
    private List<int> throwScores = new List<int>();

    // KRAV #:
    // 1: Iterator
    // 2: Klassen implementerar IEnumerable<int> för att möjliggöra iteration över poäng. Instansieras i Game.cs för att visa poänghistorik: foreach (int score in playerScore)
    // 3: Iteratorn används för att kunna gå igenom antalet poäng som tillkommer under spelet 

    public IEnumerator<int> GetEnumerator()
    {
        return throwScores.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void AddPoints(int points)
    {
        throwScores.Add(points);
    }

    public int GetTotalScore() 
    {
        int total = 0;
        foreach (int score in throwScores)
        {
            total += score;
        }
        return total;
    }
}
