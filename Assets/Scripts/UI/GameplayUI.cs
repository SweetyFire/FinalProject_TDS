using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameplayUI : InitializableBehavior
{
    [SerializeField] private InteractibleHint _interactibleHint;
    [SerializeField] private GameSaveUI _gameSave;
    [SerializeField] private GameOverUI _gameOverUI;
    [SerializeField] private RectTransform _gameplayUI;
    [SerializeField] private Image _pauseBackgroundImage;
    [SerializeField] private float _animationSpeed = 1f;

    [Header("Events")]
    public UnityEvent OnPaused;
    public UnityEvent OnContinue;

    public InteractibleHint InteractibleHint => _interactibleHint;

    private bool _initialized;
    private float _maxPauseBackgroundAlpha;

    public override void Initialize()
    {
        _interactibleHint.Initialize();
        _gameSave.Initialize();
        _gameOverUI.Initialize();
        GameManager.Instance.OnGameOver.AddListener(GameOver);

        _maxPauseBackgroundAlpha = _pauseBackgroundImage.color.a;

        Color baseColor = _pauseBackgroundImage.color;
        _pauseBackgroundImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        _initialized = true;
    }

    private void Start()
    {
        if (_initialized) return;

        throw new System.Exception($"Please add GameplayUI to Bootstrap's list");
    }

    public void ContinueGame()
    {
        if (GameManager.Instance.IsGameOver) return;

        _pauseBackgroundImage.rectTransform.LeanCancel();
        _pauseBackgroundImage.rectTransform.LeanAlpha(0f, _animationSpeed).setIgnoreTimeScale(true);

        GameManager.Instance.ContinueGame();
    }

    public void PauseGame()
    {
        if (GameManager.Instance.IsGameOver) return;

        _pauseBackgroundImage.rectTransform.LeanCancel();
        _pauseBackgroundImage.rectTransform.LeanAlpha(_maxPauseBackgroundAlpha, _animationSpeed).setIgnoreTimeScale(true);

        GameManager.Instance.PauseGame();
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    public void TogglePause()
    {
        if (GameManager.Instance.IsGameOver) return;

        if (GameManager.GamePaused)
        {
            OnContinue?.Invoke();
            ContinueGame();
        }
        else
        {
            OnPaused?.Invoke();
            PauseGame();
        }
    }

    public void Restart()
    {
        GameManager.Instance.RestartScene();
    }

    public void BackToMainMenu()
    {
        GameManager.Instance.OpenScene(0);
    }

    private void GameOver()
    {
        if (_gameplayUI.gameObject.activeSelf)
            _gameplayUI.gameObject.SetActive(false);

        _gameOverUI.gameObject.SetActive(true);
        //_gameOverUI.Enable();
    }
}
