using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float minWaitTime = 1.5f;
    [SerializeField] private float maxWaitTime = 3f;
    [SerializeField, Range(0f, 1f)] private float fakeAttackChance = 0.25f;

    [Header("Icons")]
    [SerializeField] private Image skullIcon;

    [Header("Audio")]
    [SerializeField] private AudioClip attackSfx;
    [SerializeField] private AudioClip deathSfx;
    [SerializeField] private AudioClip fakeAttackSfx;

    public void BeginAttackCycle()
    {
        StopAllCoroutines();
        StartCoroutine(AttackCycle());
    }

    private IEnumerator AttackCycle()
    {
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        if (Random.value < fakeAttackChance)
        {
            yield return StartCoroutine(PlayFakeAttack());
        }
        else
        {
            ShowAttackWarning(true);
            gameManager.OnEnemyAttackSignal();
        }
    }

    private IEnumerator PlayFakeAttack()
    {
        animator.speed = 1f;
        animator.SetTrigger("FakeAttack");
        AudioManager.Instance.PlaySfx(fakeAttackSfx);

        yield return new WaitForSeconds(GetCurrentAnimationLength());

        StartIdle();
        BeginAttackCycle();
    }

    public void ShowAttackWarning(bool active)
    {
        if (skullIcon != null)
            skullIcon.gameObject.SetActive(active);
    }

    public void StartIdle()
    {
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("FakeAttack");
        animator.Play("Idle", 0, 0f);
        ShowAttackWarning(false);
    }

    public void PlayAttack()
    {
        animator.speed = 1f;
        animator.SetTrigger("Attack");
        AudioManager.Instance.PlaySfx(attackSfx);
        ShowAttackWarning(false);
    }

    public void PlayVictory()
    {
        animator.speed = 1f;
        animator.Play("Victory", 0, 0f);
        ShowAttackWarning(false);
    }

    public void PlayDeath()
    {
        animator.speed = 1f;
        animator.SetTrigger("Death");
        AudioManager.Instance.PlaySfx(deathSfx);
        ShowAttackWarning(false);
    }

    public float GetCurrentAnimationLength()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length / animator.speed;
    }
}
