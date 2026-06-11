using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string gameSceneName = "Level01";

    [Header("Leaderboard UI")]
    public GameObject leaderboardPanel;
    public TextMeshProUGUI scoresText;

    private void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShadowUIStyle.StyleSceneCanvases();
        ShadowUIStyle.StylePanel(leaderboardPanel);

        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            UnityEditor.EditorApplication.delayCall += ApplyEditorStylePreview;
        }
    }

    private void ApplyEditorStylePreview()
    {
        if (this == null || Application.isPlaying)
        {
            return;
        }

        ShadowUIStyle.StyleSceneCanvases();
        ShadowUIStyle.StylePanel(leaderboardPanel);
    }
#endif

    public void PlayGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void ShowLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            ShadowUIStyle.StylePanel(leaderboardPanel);
        }

        RefreshLeaderboard();
    }

    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }

    public void ClearLeaderboard()
    {
        ScoreManager.ClearScores();
        RefreshLeaderboard();
    }

    private void RefreshLeaderboard()
    {
        if (scoresText == null) return;

        ScoreList scoreList = ScoreManager.LoadScores();

        if (scoreList == null || scoreList.scores == null || scoreList.scores.Count == 0)
        {
            scoresText.text = "No scores yet.";
            return;
        }

        string result = "";

        int maxScoresToShow = Mathf.Min(scoreList.scores.Count, 10);

        for (int i = 0; i < maxScoresToShow; i++)
        {
            ScoreData score = scoreList.scores[i];

            string formattedTime = FormatTime(score.time);

            result += $"{i + 1}. {score.nickname}  |  {formattedTime}  |  Deaths: {score.deaths}\n";
        }

        scoresText.text = result;
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        int milliseconds = Mathf.FloorToInt((time * 100f) % 100f);

        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
    }
}
