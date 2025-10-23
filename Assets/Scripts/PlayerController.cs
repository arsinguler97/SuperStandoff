using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameManager gameManager;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSfx;
    [SerializeField] private AudioClip deathSfx;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && gameManager.IsRoundActive())
            gameManager.OnPlayerPressedButton();
    }

    public void PlayIdle()
    {
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Death");
        animator.Play("Idle", 0, 0f);
    }

    public void PlayAttack()
    {
        animator.SetTrigger("Attack");
        AudioManager.Instance.PlaySfx(attackSfx);
    }

    public void PlayDeath()
    {
        animator.SetTrigger("Death");
        AudioManager.Instance.PlaySfx(deathSfx);
    }

    public void PlayVictory() => animator.Play("Victory", 0, 0f);

    public float GetCurrentAnimationLength()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length / animator.speed;
    }
}