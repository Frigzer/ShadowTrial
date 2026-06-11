using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor.Events;
#endif

public class MainMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string gameSceneName = "Level01";

    [Header("Leaderboard UI")]
    public GameObject leaderboardPanel;
    public TextMeshProUGUI scoresText;

    [Header("How To Play UI")]
    public GameObject howToPanel;

    private Button howToButton;
    private Button closeHowToButton;

    private void Start()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ResolveSceneReferences();
        ConfigureHowToUi();
        ConfigureHowToButtons();
        ShadowUIStyle.StyleSceneCanvases();
        ShadowUIStyle.StylePanel(leaderboardPanel);
        ShadowUIStyle.StylePanel(howToPanel);

        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }

        if (howToPanel != null)
        {
            howToPanel.SetActive(false);
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
        ResolveSceneReferences();
        ConfigureHowToUi();
#if UNITY_EDITOR
        ConfigureHowToButtonsInEditor();
#endif
        ShadowUIStyle.StylePanel(howToPanel);
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
        HideHowTo();

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

    public void ShowHowTo()
    {
        HideLeaderboard();

        if (howToPanel != null)
        {
            howToPanel.SetActive(true);
            ShadowUIStyle.StylePanel(howToPanel);
        }
    }

    public void HideHowTo()
    {
        if (howToPanel != null)
        {
            howToPanel.SetActive(false);
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

    private void ResolveSceneReferences()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            return;
        }

        if (howToPanel == null)
        {
            Transform existingPanel = FindChildByName(canvas.transform, "HowToPlayPanel");
            if (existingPanel == null)
            {
                existingPanel = FindChildByName(canvas.transform, "HowToPanel");
            }

            if (existingPanel != null)
            {
                howToPanel = existingPanel.gameObject;
            }
        }

        Transform buttonTransform = FindChildByName(canvas.transform, "HowToPlayButton");
        if (buttonTransform == null)
        {
            buttonTransform = FindChildByName(canvas.transform, "HowToButton");
        }

        howToButton = buttonTransform != null ? buttonTransform.GetComponent<Button>() : null;
    }

    private void ConfigureHowToUi()
    {
        if (howToPanel == null)
        {
            return;
        }

        howToPanel.name = "HowToPlayPanel";
        PrepareCopiedHowToPanelChildren();

        RectTransform panelRect = howToPanel.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.sizeDelta = new Vector2(520f, 580f);
        }

        TextMeshProUGUI title = GetOrCreateText(howToPanel.transform, "HowToTitleText", new Vector2(0f, -50f), new Vector2(440f, 70f), 42f);
        title.text = "How to Play";
        title.alignment = TextAlignmentOptions.Center;

        string bodyText =
            "Goal:\n" +
            "Reach the end of the level as fast as possible.\n\n" +
            "Controls:\n" +
            "WASD - Move\n" +
            "Mouse - Look around\n" +
            "Space - Jump\n" +
            "Esc - Pause\n\n" +
            "Avoid traps and falling. Your score is based on time and deaths.";

        TextMeshProUGUI body = GetOrCreateText(howToPanel.transform, "HowToBodyText", new Vector2(0f, 20f), new Vector2(430f, 320f), 19f);
        body.text = bodyText;
        body.alignment = TextAlignmentOptions.Left;

        closeHowToButton = GetOrCreateCloseButton(howToPanel.transform);
        ShadowUIStyle.StylePanel(howToPanel);
    }

    private void PrepareCopiedHowToPanelChildren()
    {
        if (howToPanel == null)
        {
            return;
        }

        if (FindChildByName(howToPanel.transform, "HowToTitleText") == null)
        {
            Transform copiedTitle = FindChildByName(howToPanel.transform, "LeaderboardTitleText");
            if (copiedTitle != null)
            {
                copiedTitle.name = "HowToTitleText";

                RectTransform titleRect = copiedTitle.GetComponent<RectTransform>();
                if (titleRect != null)
                {
                    titleRect.anchoredPosition = new Vector2(0f, -50f);
                }
            }
        }

        if (FindChildByName(howToPanel.transform, "HowToBodyText") == null)
        {
            Transform copiedScores = FindChildByName(howToPanel.transform, "ScoresText");
            if (copiedScores != null)
            {
                copiedScores.name = "HowToBodyText";
            }
        }

        Transform clearButton = FindChildByName(howToPanel.transform, "ClearScoresButton");
        if (clearButton != null)
        {
            clearButton.gameObject.SetActive(false);
        }
    }

    private void ConfigureHowToButtons()
    {
        if (howToButton != null)
        {
            howToButton.onClick.AddListener(ShowHowTo);
        }

        if (closeHowToButton != null)
        {
            closeHowToButton.onClick.AddListener(HideHowTo);
        }
    }

#if UNITY_EDITOR
    private void ConfigureHowToButtonsInEditor()
    {
        if (howToButton != null)
        {
            UnityEventTools.RemovePersistentListener(howToButton.onClick, ShowLeaderboard);
            UnityEventTools.RemovePersistentListener(howToButton.onClick, HideLeaderboard);
            UnityEventTools.RemovePersistentListener(howToButton.onClick, ShowHowTo);
            UnityEventTools.AddPersistentListener(howToButton.onClick, ShowHowTo);
            UnityEditor.EditorUtility.SetDirty(howToButton);
        }

        if (closeHowToButton != null)
        {
            UnityEventTools.RemovePersistentListener(closeHowToButton.onClick, HideLeaderboard);
            UnityEventTools.RemovePersistentListener(closeHowToButton.onClick, ShowLeaderboard);
            UnityEventTools.RemovePersistentListener(closeHowToButton.onClick, HideHowTo);
            UnityEventTools.AddPersistentListener(closeHowToButton.onClick, HideHowTo);
            UnityEditor.EditorUtility.SetDirty(closeHowToButton);
        }
    }
#endif

    private TextMeshProUGUI GetOrCreateText(Transform parent, string objectName, Vector2 anchoredPosition, Vector2 size, float fontSize)
    {
        Transform existing = FindChildByName(parent, objectName);
        TextMeshProUGUI text = existing != null ? existing.GetComponent<TextMeshProUGUI>() : null;
        if (text != null)
        {
            text.fontSize = fontSize;
            return text;
        }

        return CreateText(parent, objectName, "", anchoredPosition, size, fontSize);
    }

    private Button GetOrCreateCloseButton(Transform parent)
    {
        Transform existing = FindChildByName(parent, "CloseHowToButton");
        if (existing == null)
        {
            existing = FindChildByName(parent, "CloseLeaderboardButton");
        }

        Button button = existing != null ? existing.GetComponent<Button>() : null;
        if (button != null)
        {
            bool renamedFromLeaderboard = button.gameObject.name != "CloseHowToButton";
            button.gameObject.name = "CloseHowToButton";

            if (renamedFromLeaderboard)
            {
                RectTransform rect = button.GetComponent<RectTransform>();
                if (rect != null)
                {
                    rect.anchoredPosition = new Vector2(260f, 70f);
                }
            }

            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>(true);
            if (label != null)
            {
                label.text = "Close";
            }

            return button;
        }

        GameObject buttonObject = CreateRectObject(parent, "CloseHowToButton", new Vector2(260f, 70f), new Vector2(300f, 70f));
        buttonObject.AddComponent<CanvasRenderer>();
        buttonObject.AddComponent<Image>();
        button = buttonObject.AddComponent<Button>();

        TextMeshProUGUI text = CreateText(buttonObject.transform, "Text (TMP)", "Close", Vector2.zero, Vector2.zero, 24f);
        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        text.alignment = TextAlignmentOptions.Center;

        ShadowUIStyle.StyleRoot(buttonObject);
        return button;
    }

    private TextMeshProUGUI CreateText(Transform parent, string objectName, string textValue, Vector2 anchoredPosition, Vector2 size, float fontSize)
    {
        GameObject textObject = CreateRectObject(parent, objectName, anchoredPosition, size);
        textObject.AddComponent<CanvasRenderer>();

        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.text = textValue;
        text.fontSize = fontSize;
        text.color = Color.white;
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }

    private GameObject CreateRectObject(Transform parent, string objectName, Vector2 anchoredPosition, Vector2 size)
    {
        GameObject gameObject = new GameObject(objectName, typeof(RectTransform));
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;
        return gameObject;
    }

    private Transform FindChildByName(Transform root, string objectName)
    {
        foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == objectName)
            {
                return child;
            }
        }

        return null;
    }
}
