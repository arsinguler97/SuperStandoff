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
    [SerializeField] private Image fakeSkullIcon;

    private bool _isAttacking;

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
        ShowFakeWarning(true);
        animator.speed = 1f;
        animator.SetTrigger("FakeAttack");

        yield return new WaitForSeconds(GetCurrentAnimationLength());

        ShowFakeWarning(false);
        StartIdle();

        BeginAttackCycle();
    }

    public void ShowAttackWarning(bool active)
    {
        if (skullIcon != null)
            skullIcon.gameObject.SetActive(active);
    }

    private void ShowFakeWarning(bool active)
    {
        if (fakeSkullIcon != null)
            fakeSkullIcon.gameObject.SetActive(active);
    }

    public void StartIdle()
    {
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Death");
        animator.ResetTrigger("FakeAttack");
        animator.Play("Idle", 0, 0f);
        ShowAttackWarning(false);
        ShowFakeWarning(false);
    }

    public void PlayAttack()
    {
        animator.speed = 1f;
        animator.SetTrigger("Attack");
        ShowAttackWarning(false);
        ShowFakeWarning(false);
    }

    public void PlayVictory()
    {
        animator.speed = 1f;
        animator.Play("Victory", 0, 0f);
        ShowAttackWarning(false);
        ShowFakeWarning(false);
    }

    public void PlayDeath()
    {
        animator.speed = 1f;
        animator.SetTrigger("Death");
        ShowAttackWarning(false);
        ShowFakeWarning(false);
    }

    public float GetCurrentAnimationLength()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length / animator.speed;
    }
}
