using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;

    [Header("Input")]
    public InputActionReference pauseAction;

    [Header("Player Control")]
    public MouseLook mouseLookScript;

    [Header("Scenes")]
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    private void OnEnable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed += OnPausePressed;
        }
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.action.performed -= OnPausePressed;
        }
    }

    private void Start()
    {
        ShadowUIStyle.StyleSceneCanvases();
        ShadowUIStyle.StylePanel(pausePanel);

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Time.timeScale = 1f;
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
        ShadowUIStyle.StylePanel(pausePanel);
    }
#endif

    private void OnPausePressed(InputAction.CallbackContext context)
    {
        if (GameManager.Instance != null && GameManager.Instance.IsDead)
        {
            return;
        }

        TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            ShadowUIStyle.StylePanel(pausePanel);
        }

        if (mouseLookScript != null)
        {
            mouseLookScript.enabled = false;
        }

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeGame()
    {
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (mouseLookScript != null)
        {
            mouseLookScript.enabled = true;
        }

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
