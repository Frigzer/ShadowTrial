using UnityEngine;

public class LevelFinish : MonoBehaviour
{
    public EndScreen endScreen;

    private bool finished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (finished) return;
        if (!other.CompareTag("Player")) return;

        finished = true;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.StopTimer();
        }

        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        MouseLook mouseLook = other.GetComponent<MouseLook>();
        CharacterController controller = other.GetComponent<CharacterController>();
        Animator animator = other.GetComponentInChildren<Animator>();

        if (movement != null)
        {
            movement.enabled = false;
        }

        if (mouseLook != null)
        {
            mouseLook.enabled = false;
        }

        if (controller != null)
        {
            controller.enabled = false;
        }

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsGrounded", true);
            animator.ResetTrigger("Jump");
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (endScreen != null)
        {
            endScreen.ShowEndScreen();
        }

        Time.timeScale = 0f;
    }
}