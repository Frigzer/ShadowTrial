using UnityEngine;

public static class ScoreManager
{
    private const string ScoresKey = "Scores";

    public static void SaveScore(ScoreData newScore)
    {
        ScoreList scoreList = LoadScores();

        scoreList.scores.Add(newScore);

        scoreList.scores.Sort((a, b) =>
        {
            int timeCompare = a.time.CompareTo(b.time);

            if (timeCompare != 0)
            {
                return timeCompare;
            }

            return a.deaths.CompareTo(b.deaths);
        });

        string json = JsonUtility.ToJson(scoreList);
        PlayerPrefs.SetString(ScoresKey, json);
        PlayerPrefs.Save();

        Debug.Log("Score saved: " + json);
    }

    public static ScoreList LoadScores()
    {
        if (!PlayerPrefs.HasKey(ScoresKey))
        {
            return new ScoreList();
        }

        string json = PlayerPrefs.GetString(ScoresKey);
        ScoreList scoreList = JsonUtility.FromJson<ScoreList>(json);

        if (scoreList == null)
        {
            return new ScoreList();
        }

        return scoreList;
    }

    public static void ClearScores()
    {
        PlayerPrefs.DeleteKey(ScoresKey);
        PlayerPrefs.Save();
    }
}