using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Enemy enemy;
    [SerializeField] private TMP_Text roundText;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private CameraSwitcher cameraSwitcher;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button nextRoundButton;
    [SerializeField] private GameObject instructionPanel;

    [Header("Settings")]
    [SerializeField] private float baseReactionWindow = 1.5f;
    [SerializeField] private float restartDelayBuffer = 0.2f;
    [SerializeField] private float instructionDuration = 5f;
    [SerializeField] private float minReactionWindow = 0.1f;
    [SerializeField] private float difficultyModifier = 0.1f;

    [Header("Teleport Positions")]
    [SerializeField] private float playerZoomX = -0.75f;
    [SerializeField] private float enemyZoomX = 0.75f;

    private bool _roundActive;
    private bool _canReact;
    private int _currentRound = 1;
    private int _highScore;
    private float _currentReactionWindow;

    private Vector3 _playerDefaultPos;
    private Vector3 _enemyDefaultPos;

    private void Start()
    {
        _playerDefaultPos = player.transform.position;
        _enemyDefaultPos = enemy.transform.position;

        playAgainButton.gameObject.SetActive(false);
        nextRoundButton.gameObject.SetActive(false);
        enemy.ShowAttackWarning(false);

        UpdateHighScoreText();
        StartCoroutine(ShowInstructionsThenStart());
    }

    private IEnumerator ShowInstructionsThenStart()
    {
        Time.timeScale = 0f;
        instructionPanel.SetActive(true);

        yield return new WaitForSecondsRealtime(instructionDuration);

        instructionPanel.SetActive(false);
        Time.timeScale = 1f;

        ResetGame();
    }

    private void ResetGame()
    {
        _currentRound = 1;
        _currentReactionWindow = baseReactionWindow;
        playAgainButton.gameObject.SetActive(false);
        nextRoundButton.gameObject.SetActive(false);
        enemy.ShowAttackWarning(false);

        StartRound();
    }

    private void StartRound()
    {
        roundText.text = "ROUND " + _currentRound;
        UpdateHighScoreText();

        _roundActive = true;
        _canReact = false;

        player.PlayIdle();
        enemy.StartIdle();

        player.transform.position = _playerDefaultPos;
        enemy.transform.position = _enemyDefaultPos;

        cameraSwitcher.SwitchToDefault();
        enemy.BeginAttackCycle();
    }

    public void OnEnemyAttackSignal()
    {
        enemy.ShowAttackWarning(true);
        _canReact = true;

        CancelInvoke(nameof(EndReactionWindow));
        Invoke(nameof(EndReactionWindow), _currentReactionWindow);
    }

    private void EndReactionWindow()
    {
        if (!_roundActive) return;

        _canReact = false;
        enemy.ShowAttackWarning(false);
        HandleDeathScenario();
    }

    public void OnPlayerPressedButton()
    {
        if (!_roundActive) return;

        if (!_canReact)
        {
            HandleDeathScenario();
        }
        else
        {
            CancelInvoke(nameof(EndReactionWindow));
            enemy.ShowAttackWarning(false);
            HandleWinScenario();
        }
    }

    private void HandleWinScenario()
    {
        _roundActive = false;

        if (_currentRound > _highScore)
        {
            _highScore = _currentRound;
            UpdateHighScoreText();
        }

        TeleportToZoomPositions();
        cameraSwitcher.SwitchToZoom();

        player.PlayAttack();
        enemy.PlayDeath();

        float attackLength = player.GetCurrentAnimationLength();
        Invoke(nameof(PlayPlayerVictory), attackLength);
    }

    private void PlayPlayerVictory()
    {
        player.PlayVictory();
        float victoryLength = player.GetCurrentAnimationLength();
        Invoke(nameof(ShowNextRoundButton), victoryLength + restartDelayBuffer);
    }

    private void HandleDeathScenario()
    {
        _roundActive = false;

        TeleportToZoomPositions();
        cameraSwitcher.SwitchToZoom();

        enemy.PlayAttack();
        float attackLength = enemy.GetCurrentAnimationLength();
        Invoke(nameof(PlayEnemyVictory), attackLength);
    }

    private void PlayEnemyVictory()
    {
        enemy.PlayVictory();
        player.PlayDeath();
        float victoryLength = enemy.GetCurrentAnimationLength();
        Invoke(nameof(ShowPlayAgainButton), victoryLength + restartDelayBuffer);
    }

    private void ShowPlayAgainButton() => playAgainButton.gameObject.SetActive(true);
    private void ShowNextRoundButton() => nextRoundButton.gameObject.SetActive(true);

    public void OnPlayAgainButtonPressed() => ResetGame();

    public void OnNextRoundButtonPressed()
    {
        nextRoundButton.gameObject.SetActive(false);
        _currentRound++;

        _currentReactionWindow = Mathf.Max(minReactionWindow, baseReactionWindow - (_currentRound - 1) * difficultyModifier);

        StartRound();
    }

    private void TeleportToZoomPositions()
    {
        player.transform.position = new Vector3(playerZoomX, player.transform.position.y, player.transform.position.z);
        enemy.transform.position = new Vector3(enemyZoomX, enemy.transform.position.y, enemy.transform.position.z);
    }

    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
            highScoreText.text = "High Score: " + _highScore;
    }

    public bool IsRoundActive() => _roundActive;
}
