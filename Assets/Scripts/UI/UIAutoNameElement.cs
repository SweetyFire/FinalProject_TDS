using TMPro;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class UIAutoNameElement : MonoBehaviour
{
#if UNITY_EDITOR
    private TextMeshProUGUI _text;

    private void Update()
    {
        SetAutoNameUpdate();
    }

    protected void SetAutoNameUpdate()
    {
        if (_text == null)
            InitAutoNameTextComponent();

        if (_text == null) return;
        if (_text.text == name) return;

        Undo.RecordObject(_text, "Auto name");
        _text.text = name;

        EditorUtility.SetDirty(_text);
        if (PrefabUtility.IsPartOfAnyPrefab(_text))
            PrefabUtility.RecordPrefabInstancePropertyModifications(_text);
    }

    protected virtual void InitAutoNameTextComponent()
    {
        if (!TryGetComponent(out _text))
            _text = GetComponentInChildren<TextMeshProUGUI>();
    }
#endif
}
