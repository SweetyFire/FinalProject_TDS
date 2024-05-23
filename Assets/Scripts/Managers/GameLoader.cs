using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameLoader : InitializableBehavior
{
    private const string SCENE_INDEX_NAME = "sceneIndex";

    public static GameLoader Instance { get; private set; }
    public static bool CanGameLoaded
    {
        get
        {
            int sceneIndex = GetLastSavedSceneIndex();
            if (sceneIndex < 0)
                return false;

            return true;
        }
    }
    private static bool _isGameContinue;

    [SerializeField] private ObjectSaver _saver;
    [SerializeField] private PlayerController _player;
    [SerializeField] private bool _notLoadThisScene;

#if UNITY_EDITOR
    [Header("Editor only")]
    [SerializeField] private bool _loadThisScene;
#endif

    [Header("Events")]
    public UnityEvent OnSaving;
    public UnityEvent OnSaved;
    public UnityEvent OnLoading;
    public UnityEvent OnLoaded;

    public bool GameLoaded { get; private set; }
    public bool GameSaving { get; private set; }

    private int _currentSceneIndex;
    private float _maxSavingTime = 0.25f;
    private float _maxLoadingTime = 0.25f;
    private float _timeToLoad = -1f;
    private float _timeToSave = -1f;

    public override void Initialize()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        enabled = false;

#if UNITY_EDITOR
        _isGameContinue = _loadThisScene;
#endif

        _currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        InitObjects();
    }

    private void Update()
    {
        SavingUpdate();
        LoadingUpdate();
    }

    public void NewGame()
    {
        _currentSceneIndex = 1;
        SaveSceneIndex();
        _isGameContinue = false;
        SceneManager.LoadScene(_currentSceneIndex);
    }

    public void ContinueGame()
    {
        int lastIndex = GetLastSavedSceneIndex();
        if (lastIndex < 0) return;

        _isGameContinue = true;
        SceneManager.LoadScene(lastIndex);
    }

    public void Save()
    {
        OnSaving?.Invoke();
        GameSaving = true;
        SaveWithoutInvoke();
        StartSaving();
    }

    public void SaveWithoutInvoke()
    {
        SaveSceneIndex();
        PlayerData.SetValues(_player);

        SaveSystem.Save();
    }

    public void LoadCurrentScene()
    {
        if (_notLoadThisScene) return;

        if (!_isGameContinue)
        {
            Loaded();
            return;
        }

        if (GetLastSavedSceneIndex() != _currentSceneIndex)
        {
            throw new Exception($"Current scene {_currentSceneIndex} is not the last saved scene {GetLastSavedSceneIndex()}");
        }

        OnLoading?.Invoke();

        _isGameContinue = false;
        if (_saver != null)
        {
            _saver.Load();
        }

        if (_player != null)
        {
            PlayerData data = new();
            data.Load(_player);
        }

        StartLoading();
    }

    private void InitObjects()
    {
        if (_notLoadThisScene) return;

        if (_player == null)
        {
            Debug.LogWarning($"Player on this scene is null. Check the \"NotLoadThisScene\" box to not show this warning");
        }
        else
        {
            _player.Init();
        }

        if (_saver == null)
        {
            Debug.LogWarning($"Saver on this scene is null. Check the \"NotLoadThisScene\" box to not show this warning");
        }
        else
        {
            _saver.InitSavedObjects();
        }
    }

    private void StartSaving()
    {
        _timeToSave = _maxSavingTime;
        enabled = true;
    }

    private void StartLoading()
    {
        _timeToLoad = _maxLoadingTime;
        enabled = true;
    }

    private void Saved()
    {
        OnSaved?.Invoke();
        GameSaving = false;
    }

    private void Loaded()
    {
        OnLoaded?.Invoke();
        GameLoaded = true;
    }

    private void SavingUpdate()
    {
        if (_timeToSave <= -1f) return;
        if (_timeToSave > 0f)
        {
            _timeToSave -= Time.deltaTime;
            return;
        }

        _timeToSave = -1f;
        Saved();
    }

    private void LoadingUpdate()
    {
        if (GameLoaded && _timeToSave <= -1f)
        {
            enabled = false;
            return;
        }

        if (_timeToLoad <= -1f) return;
        if (_timeToLoad > 0f)
        {
            _timeToLoad -= Time.deltaTime;
            return;
        }

        _timeToLoad = -1f;
        Loaded();
    }

    private void SaveSceneIndex()
    {
        SaveSystem.SetValue(SCENE_INDEX_NAME, _currentSceneIndex);
    }

    private static int GetLastSavedSceneIndex()
    {
        if (!SaveSystem.TryGetInt(SCENE_INDEX_NAME, out int sceneIndex))
            return -1;

        return sceneIndex;
    }
}
