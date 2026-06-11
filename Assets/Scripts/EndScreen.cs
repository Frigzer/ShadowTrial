using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    [Header("UI")]
    public GameObject finishPanel;
    public TextMeshProUGUI resultText;
    public TMP_InputField nickInputField;

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    private void Start()
    {
        ShadowUIStyle.StyleSceneCanvases();
        ShadowUIStyle.StylePanel(finishPanel);

        if (finishPanel != null)
        {
            finishPanel.SetActive(false);
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
        ShadowUIStyle.StylePanel(finishPanel);
    }
#endif

    public void ShowEndScreen()
    {
        if (finishPanel != null)
        {
            finishPanel.SetActive(true);
            ShadowUIStyle.StylePanel(finishPanel);
        }

        if (GameManager.Instance != null && resultText != null)
        {
            resultText.text =
                "Time: " + GameManager.Instance.GetFormattedTime() +
                "\nDeaths: " + GameManager.Instance.DeathCount;
        }
    }

    public void SaveScore()
    {
        string nickname = "Player";

        if (nickInputField != null && !string.IsNullOrWhiteSpace(nickInputField.text))
        {
            nickname = nickInputField.text.Trim();
        }

        if (GameManager.Instance != null)
        {
            ScoreData score = new ScoreData(
                nickname,
                GameManager.Instance.ElapsedTime,
                GameManager.Instance.DeathCount
            );

            ScoreManager.SaveScore(score);
        }

        ReturnToMainMenu();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
