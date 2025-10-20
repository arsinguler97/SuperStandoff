using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameManager gameManager;

    [Header("Timing Settings")]
    [SerializeField] private float minWaitTime = 1.5f;
    [SerializeField] private float maxWaitTime = 3f;

    private float _attackSpeed = 1f;
    private Coroutine _attackRoutine;

    public void BeginAttackCycle()
    {
        if (_attackRoutine != null)
            StopCoroutine(_attackRoutine);

        _attackRoutine = StartCoroutine(AttackCycle());
    }

    private IEnumerator AttackCycle()
    {
        yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));

        animator.speed = _attackSpeed;
        animator.SetTrigger("Attack");

        gameManager.OnEnemyAttackStarted();

        yield return new WaitForSeconds(1f / _attackSpeed);
        animator.speed = 1f;
        animator.SetTrigger("Idle");

        BeginAttackCycle();
    }

    public void StartIdle()
    {
        animator.speed = 1f;
        animator.SetTrigger("Idle");
    }

    public void PlayAttack()
    {
        animator.speed = _attackSpeed;
        animator.SetTrigger("Attack");
    }

    // Returns the duration of the current animation in seconds
    public float GetCurrentAnimationLength()
    {
        return animator.GetCurrentAnimatorStateInfo(0).length / animator.speed;
    }

    public void SetAttackSpeed(float speed)
    {
        _attackSpeed = speed;
    }

    public float GetAttackSpeed()
    {
        return _attackSpeed;
    }
}