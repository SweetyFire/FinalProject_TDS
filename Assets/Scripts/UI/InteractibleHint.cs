using UnityEngine;

public class InteractibleHint : InitializableBehavior
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Camera _camera;

    private GameObject _followObject;
    private RectTransform _rectTransform;
    private RectTransform _canvasTransform;

    public override void Initialize()
    {
        InitComponents();
    }

    private void Update()
    {
        FollowObjectUpdate();
    }


    public void SetFollow(GameObject followObject)
    {
        _followObject = followObject;
        FollowObjectUpdate();

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
        _followObject = null;
    }

    private void InitComponents()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasTransform = _canvas.GetComponent<RectTransform>();
    }

    private void FollowObjectUpdate()
    {
        if (_followObject == null) return;

        Vector2 newPosition = _camera.WorldToViewportPoint(_followObject.transform.position);
        newPosition = new(
            newPosition.x * _canvasTransform.sizeDelta.x,
            newPosition.y * _canvasTransform.sizeDelta.y);
        _rectTransform.anchoredPosition = newPosition;
    }
}
