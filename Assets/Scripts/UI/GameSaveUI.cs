using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSaveUI : InitializableBehavior
{
    private const string SAVING_TEXT = "Saving...";
    private const string SAVED_TEXT = "Saved";
    private const string LOADING_TEXT = "Loading...";
    private const string LOADED_TEXT = "Loaded";

    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TextMeshProUGUI _savingText;
    [SerializeField] private float _savingAnimSpeed = 1f;
    [SerializeField] private float _loadingAnimSpeed = 1f;
    [SerializeField] private LeanTweenType _animationType;

    private float _maxBackgroundAlpha;

    public override void Initialize()
    {
        _maxBackgroundAlpha = _backgroundImage.color.a;

        Color baseColor = _backgroundImage.color;
        _backgroundImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        GameLoader.Instance.OnSaving.AddListener(OnSaving);
        GameLoader.Instance.OnSaved.AddListener(OnSaved);
        GameLoader.Instance.OnLoading.AddListener(OnLoading);
        GameLoader.Instance.OnLoaded.AddListener(OnLoaded);
    }

    private void OnSaving()
    {
        _savingText.text = SAVING_TEXT;
        AnimSavingScreen(true);
    }

    private void OnSaved()
    {
        _savingText.text = SAVED_TEXT;
        AnimSavingScreen(false);
    }

    private void OnLoading()
    {
        _savingText.text = LOADING_TEXT;
        AnimLoadingScreen(true);
    }

    private void OnLoaded()
    {
        _savingText.text = LOADED_TEXT;
        AnimLoadingScreen(false);
    }

    private void AnimLoadingScreen(bool enable)
    {
        LeanTween.cancel(_backgroundImage.rectTransform);

        if (enable)
        {
            Color baseColor = _backgroundImage.color;
            _backgroundImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);

            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
        }
        else
        {
            _backgroundImage.rectTransform.LeanAlpha(0f, _loadingAnimSpeed).setEase(_animationType).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                if (gameObject.activeSelf)
                    gameObject.SetActive(false);
            });
        }
    }

    private void AnimSavingScreen(bool enable)
    {
        LeanTween.cancel(_backgroundImage.rectTransform);

        if (enable)
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            _backgroundImage.rectTransform.LeanAlpha(_maxBackgroundAlpha, _savingAnimSpeed).setEase(_animationType).setIgnoreTimeScale(true);
        }
        else
        {
            _backgroundImage.rectTransform.LeanAlpha(0f, _savingAnimSpeed).setEase(_animationType).setIgnoreTimeScale(true).setOnComplete(() =>
            {
                if (gameObject.activeSelf)
                    gameObject.SetActive(false);
            });
        }
    }
}
