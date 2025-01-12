using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class ScoreTableManager : MonoBehaviour
{
    public Transform content; // Content w Scroll View
    public GameObject rowPrefab; // Prefab wiersza tabeli (RowScoreLabel)
    public int menuType;

    public LoadManager load = LoadManager.Instance;

    public Color normalColor = Color.white;   // Default color
    public Color hoverColor = Color.yellow;   // Color on hover
    public Color pressedColor = Color.green;  // Color when pressed

    public void LoadScores()
    {
        GameObject menuManager = GameObject.Find("MenuManager");

        Dictionary<string, KeyValuePair<int, string>> fileData = new Dictionary<string, KeyValuePair<int, string>>();

        if (menuManager != null)
        {
            MenuManager menuScript = menuManager.GetComponent<MenuManager>();

            if (menuScript != null)
            { 
                if(menuType == 0) fileData = menuScript.getPlayerScores();
                else fileData = menuScript.getPlayerSaves();
            }
        }

        else
        {
            Debug.LogWarning("Menu Manager GameObject not found!");
        }

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        int index = 1;

        foreach (KeyValuePair<string, KeyValuePair<int, string>> entry in fileData){
            string nickname = entry.Key; 
            int score = entry.Value.Key;
            string date = entry.Value.Value;

            AddRow(index, score, nickname, date);

            index++;
        }
    }

    private void OnRowClicked(string nickname, string date)
    {
        load.loadGame(nickname, date);
    }

    private void AddRow(int index, int score, string nickname, string date)
    {
        // Stw�rz nowy wiersz
        GameObject row = Instantiate(rowPrefab, content);

        // Znajd� pola w prefabie
        TMP_Text indexText = row.transform.Find("Index").GetComponent<TMP_Text>();
        TMP_Text scoreText = row.transform.Find("Score").GetComponent<TMP_Text>();
        TMP_Text nicknameText = row.transform.Find("Nickname").GetComponent<TMP_Text>();

        // Ustaw warto�ci
        indexText.text = index.ToString();
        scoreText.text = score.ToString();
        nicknameText.text = nickname;

        if(menuType != 0){
            Button rowButton = row.AddComponent<Button>();
            rowButton.onClick.AddListener(() => OnRowClicked(nickname, date));
        }
    }
}
