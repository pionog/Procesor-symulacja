using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public ScoreTableManager scoreTableManager;

    public void ExitButton() {
        Application.Quit();
    }
    public void ScoreButton()
    {
        ScoreEntry[] scores = new ScoreEntry[]
        {
            new ScoreEntry(150, "Player1"),
            new ScoreEntry(200, "Player2"),
            new ScoreEntry(120, "Player3")
        };

        // Wczytaj dane do tabeli
        
        scoreTableManager.LoadScores(scores);
    }
}
