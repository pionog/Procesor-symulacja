using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
using TMPro;

public class MenuManager : MonoBehaviour
{
    private Dictionary<string, KeyValuePair<int, string>> playerScores;
    private Dictionary<string, KeyValuePair<int, string>> playerSaves;
    public Transform content;
    public GameObject scoreRowPrefab; 
    // Start is called before the first frame update
    void Start()
    {
        playerScores = new Dictionary<string, KeyValuePair<int, string>>();
        playerSaves = new Dictionary<string, KeyValuePair<int, string>>();

        loadScores();
        loadSaves();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Serializable]
    public class ScoreData {
        public int score;
        public string nickname;
        public string timestamp;

        public ScoreData(int score, string nickname){
            this.score = score;
            this.nickname = nickname;
            this.timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        }
    }

    public Dictionary<string, KeyValuePair<int, string>> getPlayerScores() {
        return playerScores;
    }

    public Dictionary<string, KeyValuePair<int, string>> getPlayerSaves() {
        return playerSaves;
    }

    public void loadScores() {
        string directoryPath = Application.persistentDataPath;
        string[] files = Directory.GetFiles(directoryPath, "score_*.json");

        foreach (string file in files)
        {
            string jsonContent = File.ReadAllText(file);

            ScoreData scoreData = JsonUtility.FromJson<ScoreData>(jsonContent);

            if (!string.IsNullOrEmpty(scoreData.nickname))
            {
                playerScores[scoreData.nickname] = new KeyValuePair<int, string>(scoreData.score, scoreData.timestamp);
            }
        }

        Debug.Log($"Loaded {playerScores.Count} scores.");
    }

    public void loadSaves() {
        string directoryPath = Application.persistentDataPath;
        string[] files = Directory.GetFiles(directoryPath, "save_*.json");

        foreach (string file in files)
        {
            string jsonContent = File.ReadAllText(file);

            ScoreData saveData = JsonUtility.FromJson<ScoreData>(jsonContent);

            if (!string.IsNullOrEmpty(saveData.nickname))
            {
                playerSaves[saveData.nickname] = new KeyValuePair<int, string>(saveData.score, saveData.timestamp);
            }
        }

        Debug.Log($"Loaded {playerSaves.Count} saves.");
    }
}
