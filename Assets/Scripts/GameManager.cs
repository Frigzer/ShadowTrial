using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI deathsText;
    public TextMeshProUGUI timeText;
    public GameObject deathPanel;

    [Header("Start Hint")]
    public bool showStartHint = true;
    public float startHintDuration = 6f;

    [Header("Respawn")]
    public Transform currentSpawnPoint;
    public bool autoRespawn = true;
    public float autoRespawnDelay = 1.5f;

    [Header("Score")]
    public int DeathCount => deathCount;
    public float ElapsedTime => elapsedTime;
    public bool IsDead => isDead;

    private int deathCount = 0;
    private int currentCheckpointIndex = -1;
    private float elapsedTime = 0f;
    private bool timerRunning = true;
    private bool isDead = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        UpdateDeathUI();
        UpdateTimeUI();
        ShadowUIStyle.StyleHud(deathsText, timeText);

        if (showStartHint)
        {
            CreateStartHint();
        }
    }

    private void Update()
    {
        if (!timerRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateTimeUI();
    }

    public void RegisterDeath(GameObject player)
    {
        if (isDead) return;

        StartCoroutine(DeathSequence(player));
    }

    private IEnumerator DeathSequence(GameObject player)
    {
        isDead = true;
        timerRunning = false;

        deathCount++;
        UpdateDeathUI();

        CharacterController controller = player.GetComponent<CharacterController>();
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        Animator animator = player.GetComponentInChildren<Animator>();
        MouseLook mouseLook = FindFirstObjectByType<MouseLook>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        if (movement != null)
        {
            movement.enabled = false;
        }

        if (animator != null)
        {
            animator.speed = 0f;
        }

        if (mouseLook != null)
        {
            mouseLook.enabled = false;
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        Time.timeScale = 0f;

        if (autoRespawn)
        {
            yield return new WaitForSecondsRealtime(autoRespawnDelay);
            RespawnPlayer(player);
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        Time.timeScale = 1f;

        CharacterController controller = player.GetComponent<CharacterController>();
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        Animator animator = player.GetComponentInChildren<Animator>();
        MouseLook mouseLook = FindFirstObjectByType<MouseLook>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        player.transform.position = currentSpawnPoint.position;

        ResetRespawnObjects();

        if (controller != null)
        {
            controller.enabled = true;
        }

        if (movement != null)
        {
            movement.enabled = true;
        }

        if (animator != null)
        {
            animator.speed = 1f;
        }

        if (mouseLook != null)
        {
            mouseLook.enabled = true;
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        timerRunning = true;
        isDead = false;
    }

    public void RespawnPlayerFromButton()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            RespawnPlayer(player);
        }
    }

    private void ResetRespawnObjects()
    {
        FallingPlatform[] fallingPlatforms = FindObjectsByType<FallingPlatform>(FindObjectsSortMode.None);
        foreach (FallingPlatform platform in fallingPlatforms)
        {
            if (platform.resetOnPlayerRespawn)
            {
                platform.ResetPlatform();
            }
        }

        PushingWall[] pushingWalls = FindObjectsByType<PushingWall>(FindObjectsSortMode.None);
        foreach (PushingWall wall in pushingWalls)
        {
            if (wall.resetOnPlayerRespawn)
            {
                wall.ResetWall();
            }
        }
    }

    public void TrySetCheckpoint(Transform newSpawn, int checkpointIndex)
    {
        if (checkpointIndex > currentCheckpointIndex)
        {
            currentCheckpointIndex = checkpointIndex;
            currentSpawnPoint = newSpawn;
            Debug.Log("Activated checkpoint: " + checkpointIndex);
        }
    }

    private void UpdateDeathUI()
    {
        if (deathsText != null)
        {
            deathsText.text = "Deaths: " + deathCount;
        }
    }

    private void CreateStartHint()
    {
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null || FindChildByName(canvas.transform, "StartHintPanel") != null)
        {
            return;
        }

        GameObject panel = CreateRectObject(canvas.transform, "StartHintPanel", new Vector2(0f, -155f), new Vector2(680f, 104f));
        panel.AddComponent<CanvasRenderer>();
        panel.AddComponent<Image>();

        TextMeshProUGUI text = CreateText(
            panel.transform,
            "StartHintText",
            "Reach the finish.\nWASD - Move    Space - Jump    Mouse - Look    Esc - Pause",
            Vector2.zero,
            new Vector2(620f, 76f),
            20f
        );
        text.alignment = TextAlignmentOptions.Center;

        ShadowUIStyle.StyleRoot(panel);
        StartCoroutine(HideStartHintAfterDelay(panel));
    }

    private IEnumerator HideStartHintAfterDelay(GameObject panel)
    {
        yield return new WaitForSecondsRealtime(startHintDuration);

        if (panel != null)
        {
            panel.SetActive(false);
        }
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
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
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

    private void UpdateTimeUI()
    {
        if (timeText == null) return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

        timeText.text = $"Time: {minutes:00}:{seconds:00}.{milliseconds:00}";
    }

    public void StopTimer()
    {
        timerRunning = false;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

        return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
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

        ShadowUIStyle.StyleHud(deathsText, timeText);
    }
#endif
}
