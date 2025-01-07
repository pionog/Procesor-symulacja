using UnityEngine;
using TMPro;

public class ScoreTableManager : MonoBehaviour
{
    public Transform content; // Content w Scroll View
    public GameObject rowPrefab; // Prefab wiersza tabeli (RowScoreLabel)

    public void LoadScores(ScoreEntry[] scores)
    {
        // Usu� istniej�ce wiersze (je�li od�wie�asz tabel�)
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
