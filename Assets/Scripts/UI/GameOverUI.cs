using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : InitializableBehavior
{
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private TextMeshProUGUI _gameOverText;
    [SerializeField] private RectTransform _buttons;
    [SerializeField] private float _animationSpeed = 3f;
    [SerializeField] private float _textAnimationSpeed = 1f;
    [SerializeField] private float _maxBackgroundAlpha;
    [SerializeField] private Vector2 _defaultButtonSize;
    [SerializeField] private Vector2 _defaultGameOverTextSize;

    public override void Initialize()
    {
        ////_maxBackgroundAlpha = _backgroundImage.color.a;

        //Color baseColor = _backgroundImage.color;
        //_backgroundImage.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);

        ////_defaultButtonSize = _buttons.sizeDelta;
        ////_buttons.sizeDelta = new Vector2(0f, _buttons.sizeDelta.y);
        //_gameOverText.rectTransform.sizeDelta = new Vector2(0f, _gameOverText.rectTransform.sizeDelta.y);
    }

    public void Enable()
    {
        gameObject.SetActive(true);

        _backgroundImage.rectTransform.LeanAlpha(_maxBackgroundAlpha, _animationSpeed).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
        LeanTween.size(_gameOverText.rectTransform, _defaultGameOverTextSize, _textAnimationSpeed).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
        //_buttons.LeanSize(_defaultButtonSize, _textAnimationSpeed).setEase(LeanTweenType.linear).setIgnoreTimeScale(true);
    }
}
