using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameManager gameManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gameManager.IsRoundActive())
        {
            gameManager.OnPlayerPressedButton();
        }
    }

    public void PlayIdle()
    {
        animator.SetTrigger("Idle");
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
    }

    // Returns the duration of the current animation in seconds
    public float GetCurrentAnimationLength()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length / animator.speed;
    }
}