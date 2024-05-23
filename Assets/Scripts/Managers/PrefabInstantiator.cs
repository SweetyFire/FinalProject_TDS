using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class PrefabInstantiator : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private bool _onlyOneInstance = true;

    public void CreatePrefab()
    {
        if (_prefab == null)
        {
            Debug.LogError($"Prefab is NULL");
            return;
        }

        if (_onlyOneInstance)
        {
            if (_parent == null)
            {
                Debug.LogWarning("Parent is NULL you can't make only one instance");
            }
            else
            {
                DestroyParentChildren();
            }
        }

        Undo.RecordObject(_parent, $"Added {_prefab.name} to {_parent.gameObject.name}");

        GameObject go = PrefabUtility.InstantiatePrefab(_prefab, _parent) as GameObject;
        go.name = _prefab.name;

        EditorUtility.SetDirty(_parent);
        if (PrefabUtility.IsPartOfAnyPrefab(_parent))
            PrefabUtility.RecordPrefabInstancePropertyModifications(_parent);
    }

    private void DestroyParentChildren()
    {
        if (_parent.childCount <= 0) return;

        Undo.RecordObject(_parent, $"Removed gameObjects from {_parent.gameObject.name}");

        for (int i = _parent.childCount; i > 0; --i)
        {
            DestroyImmediate(_parent.GetChild(0).gameObject);
        }
    }
#endif
}
