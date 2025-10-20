using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text roundText;

    [Header("Settings")]
    [SerializeField] private float baseReactionWindow = 1f;
    [SerializeField] private float restartDelayBuffer = 0.2f; // small buffer after animation ends
    [SerializeField] private float difficultyStep = 0.1f;

    private bool _roundActive;
    private bool _canReact;
    private int _currentRound = 1;
    private float _currentReactionWindow;

    private void Start()
    {
        ResetGame();
    }

    private void ResetGame()
    {
        _currentRound = 1;
        _currentReactionWindow = baseReactionWindow;
        enemy.SetAttackSpeed(1f);
        StartRound();
    }

    public void StartRound()
    {
        resultText.text = "";
        roundText.text = "ROUND " + _currentRound;

        _roundActive = true;
        _canReact = false;

        player.PlayIdle();
        enemy.StartIdle();
        enemy.BeginAttackCycle();
    }

    public void OnEnemyAttackStarted()
    {
        _canReact = true;
        CancelInvoke(nameof(EndReactionWindow));
        Invoke(nameof(EndReactionWindow), _currentReactionWindow);
    }

    private void EndReactionWindow()
    {
        _canReact = false;
        if (_roundActive)
        {
            PlayerDies();
        }
    }

    public void OnPlayerPressedButton()
    {
        if (!_roundActive) return;

        // Player attacks immediately
        player.PlayAttack();

        if (!_canReact)
        {
            // Enemy also attacks if player presses too early
            enemy.PlayAttack();
            PlayerDies();
        }
        else
        {
            PlayerWins();
        }
    }

    private void PlayerWins()
    {
        _roundActive = false;
        resultText.text = "YOU WIN!";

        // Increase difficulty
        _currentRound++;
        _currentReactionWindow = Mathf.Max(0.2f, _currentReactionWindow - difficultyStep);
        enemy.SetAttackSpeed(enemy.GetAttackSpeed() + difficultyStep);

        // Wait until player attack animation fully ends before starting next round
        float animLength = player.GetCurrentAnimationLength();
        float delay = animLength + restartDelayBuffer;
        Invoke(nameof(StartRound), delay);
    }

    private void PlayerDies()
    {
        _roundActive = false;
        resultText.text = "YOU DIED";

        // Wait until enemy attack animation fully ends before resetting
        float animLength = enemy.GetCurrentAnimationLength();
        float delay = animLength + restartDelayBuffer;
        Invoke(nameof(ResetGame), delay);
    }

    public bool IsRoundActive() => _roundActive;
}
