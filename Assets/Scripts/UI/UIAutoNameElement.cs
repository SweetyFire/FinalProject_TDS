using TMPro;
using UnityEngine;

public class UIAutoNameElement : MonoBehaviour
{
#if UNITY_EDITOR
    protected virtual void UpdateText()
    {
        if (!TryGetComponent(out TextMeshProUGUI text))
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (text == null) return;

        text.text = name;
    }

    private void OnValidate()
    {
        UpdateText();
    }
#endif
}
