using UnityEngine;
using TMPro;

public class ScoreTableManager : MonoBehaviour
{
    public Transform content; // Content w Scroll View
    public GameObject rowPrefab; // Prefab wiersza tabeli (RowScoreLabel)

    public void LoadScores(ScoreEntry[] scores)
    {
        // Usuñ istniej¹ce wiersze (jeœli odœwie¿asz tabelê)
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // Dodaj nowe wiersze
        for (int i = 0; i < scores.Length; i++)
        {
            AddRow(i + 1, scores[i].Score, scores[i].Nickname);
        }
    }

    private void AddRow(int index, int score, string nickname)
    {
        // Stwórz nowy wiersz
        GameObject row = Instantiate(rowPrefab, content);

        // ZnajdŸ pola w prefabie
        TMP_Text indexText = row.transform.Find("Index").GetComponent<TMP_Text>();
        TMP_Text scoreText = row.transform.Find("Score").GetComponent<TMP_Text>();
        TMP_Text nicknameText = row.transform.Find("Nickname").GetComponent<TMP_Text>();

        // Ustaw wartoœci
        indexText.text = index.ToString();
        scoreText.text = score.ToString();
        nicknameText.text = nickname;
    }
}

[System.Serializable]
public class ScoreEntry
{
    public int Score;
    public string Nickname;

    public ScoreEntry(int score, string nickname)
    {
        Score = score;
        Nickname = nickname;
    }
}
