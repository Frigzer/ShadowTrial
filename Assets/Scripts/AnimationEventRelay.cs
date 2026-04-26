using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public void OnFootstep()
    {
        if (playerMovement != null)
        {
            playerMovement.OnFootstep();
        }
    }
}