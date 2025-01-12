using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour
{
    public ScoreTableManager scoreTableManager;
    public ScoreTableManager scoreTableManager2;

    public void ExitButton() {
        Application.Quit();
    }

    public void ScoreButton() {
        scoreTableManager.LoadScores();
    }

    public void SaveButton() {
        scoreTableManager2.LoadScores();
    }
}
