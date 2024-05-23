using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class ObjectSaver : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private bool _refresh;
#endif

    [SerializeField] private List<SavedObject> _savedObjects = new();

#if UNITY_EDITOR
    private void Awake()
    {
        if (!Application.isPlaying)
        {
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }
    }

    private void OnDestroy()
    {
        if (!Application.isPlaying)
        {
            EditorSceneManager.sceneSaved -= OnSceneSaved;
        }
    }
#endif

    public void InitSavedObjects()
    {
        for (int i = 0; i < _savedObjects.Count; i++)
        {
            _savedObjects[i].InitSaver(this);
        }
    }

    public void Save(SavedObject savedObject)
    {
        int index = _savedObjects.IndexOf(savedObject);
        if (index < 0)
        {
            Debug.LogWarning($"Object \"{savedObject.name}\" doesn't exists in saved objects");
            return;
        }

        _savedObjects[index].Save(index, GetSceneId());
    }

    public void Save(int id)
    {
        if (_savedObjects.Count <= id)
        {
            Debug.LogWarning($"Object with id {id} doesn't exists in saved objects");
            return;
        }

        _savedObjects[id].Save(id, GetSceneId());
    }

    public int GetId(SavedObject savedObject)
    {
        return _savedObjects.IndexOf(savedObject);
    }

    public int GetSceneId()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public void Load()
    {
        int sceneId = GetSceneId();
        for (int i = 0; i < _savedObjects.Count; i++)
        {
            _savedObjects[i].Load(i, sceneId);
        }
    }

#if UNITY_EDITOR
    private bool InitSavedObjectsList()
    {
        bool changed = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).TryGetComponent(out SavedObject obj)) continue;
            if (_savedObjects.Contains(obj)) continue;

            _savedObjects.Add(obj);
            changed = true;
        }

        return changed;
    }

    private bool ClearNullSavedObjects()
    {
        bool changed = false;
        for (int i = _savedObjects.Count - 1; i >= 0; i--)
        {
            if (_savedObjects[i] != null && _savedObjects[i].transform != null) continue;

            _savedObjects.RemoveAt(i);
            changed = true;
        }

        return changed;
    }

    [ContextMenu("Refresh Saved Objects")]
    private void RefreshList()
    {
        Undo.RecordObject(this, "Saved objects change");
        if (!InitSavedObjectsList() && !ClearNullSavedObjects())
        {
            Undo.ClearUndo(this);
            return;
        }

        EditorUtility.SetDirty(this);
        if (PrefabUtility.IsPartOfAnyPrefab(gameObject))
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
    }

    private void OnSceneSaved(Scene scene)
    {
        RefreshList();
    }

    private void OnTransformChildrenChanged()
    {
        RefreshList();
    }

    private void OnValidate()
    {
        if (!_refresh) return;

        RefreshList();
        _refresh = false;
    }
#endif
}
