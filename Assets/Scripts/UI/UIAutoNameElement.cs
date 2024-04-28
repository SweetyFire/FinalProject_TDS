using TMPro;
using UnityEngine;

[ExecuteAlways]
public class UIAutoNameElement : MonoBehaviour
{
#if UNITY_EDITOR
    private TextMeshProUGUI _text;

    private void Update()
    {
        SetAutoName();
    }

    protected virtual void SetAutoName()
    {
        if (_text == null)
            InitComponents();

        if (_text == null) return;
        _text.text = name;
    }

    private void InitComponents()
    {
        if (!TryGetComponent(out _text))
            _text = GetComponentInChildren<TextMeshProUGUI>();
    }
#endif
}
