using TMPro;
using UnityEditor;
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

    protected void SetAutoName()
    {
        if (_text == null)
            InitAutoNameTextComponent();

        if (_text == null) return;

        Undo.RecordObject(_text, "Auto name");
        _text.text = name;
        EditorUtility.SetDirty(_text);
        PrefabUtility.RecordPrefabInstancePropertyModifications(_text);
    }

    protected virtual void InitAutoNameTextComponent()
    {
        if (!TryGetComponent(out _text))
            _text = GetComponentInChildren<TextMeshProUGUI>();
    }
#endif
}
