using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum UIButtonAnimation
{
    None,
    Enter,
    Exit,
    Pressed
}

public class AnimatedButton : UIAutoNameElement, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    public UnityEvent _onClick;
    [SerializeField] private RectTransform _animRect;
    [SerializeField] private Vector2 _enterPosition;
    [SerializeField] private Vector2 _pressedPosition;
    [SerializeField] private float _animSpeed = 0.18f;
    [SerializeField] private LeanTweenType _animType;

    [Header("Sound")]
    [SerializeField] private AudioClip _enterSound;
    [SerializeField] private AudioClip _clickSound;

    private bool _isEnter;
    private bool _isPressed;
    private UIButtonAnimation _animation;
    private Vector2 _defaultPosition;

    private void Awake()
    {
        _defaultPosition = _animRect.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
        SetAnim(UIButtonAnimation.Pressed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isEnter = true;
        SetAnim(UIButtonAnimation.Enter);
        SoundManager.Instance.PlaySound(_enterSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isEnter = false;
        _isPressed = false;
        SetAnim(UIButtonAnimation.Exit);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_isPressed) return;

        SoundManager.Instance.PlaySound(_clickSound, randomizePitch: false);
        _isPressed = false;
        _isEnter = false;
        _onClick?.Invoke();
        SetAnim(UIButtonAnimation.Enter);
    }

    private void SetAnim(UIButtonAnimation animation)
    {
        if (_animation == animation) return;

        StopAnim();
        _animation = animation;
        Vector2 goalPos = animation switch
        {
            UIButtonAnimation.Enter => _enterPosition,
            UIButtonAnimation.Pressed => _pressedPosition,
            _ => _defaultPosition,
        };

        LeanTween.move(_animRect, goalPos, _animSpeed).setEase(_animType).setIgnoreTimeScale(true).setOnComplete(() =>
        {
            if (_animation == UIButtonAnimation.Exit)
                _animation = UIButtonAnimation.None;
        });
    }

    private void StopAnim()
    {
        LeanTween.cancel(_animRect);
    }
}
