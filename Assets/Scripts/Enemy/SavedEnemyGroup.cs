using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

#if UNITY_EDITOR
using Scene = UnityEngine.SceneManagement.Scene;
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteAlways]
public class SavedEnemyGroup : SavedObject
{
    private const string POSITION_NAME = "position";
    private const string ROTATION_NAME = "rotation";
    private const string HEALTH_NAME = "health";

#if UNITY_EDITOR
    [SerializeField] private bool _refresh;
#endif

    [SerializeField] private List<EnemyController> _enemies = new();

    private StringBuilder _persistStringBuilder = new();

    private void Awake()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorSceneManager.sceneSaved += OnSceneSaved;
        }
#endif
        if (!Application.isPlaying) return;
        Init();
    }

#if UNITY_EDITOR
    private void OnDestroy()
    {
        if (!Application.isPlaying)
        {
            EditorSceneManager.sceneSaved -= OnSceneSaved;
        }
    }
#endif

    public override void Save(int id, int sceneId)
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            SaveEnemy(i);
        }
    }

    public override void Load(int id, int sceneId)
    {
        EnemyData data = new();
        for (int i = 0; i < _enemies.Count; i++)
        {
            data.Clear();

            if (!SaveSystem.TryGetString(GetEnemySavedValueName(i, POSITION_NAME), out string posStr)) continue;
            if (!VectorExtensions.TryParse(posStr, out data.position)) continue;

            if (!SaveSystem.TryGetString(GetEnemySavedValueName(i, ROTATION_NAME), out string rotStr)) continue;
            if (!QuaternionExtensions.TryParse(rotStr, out data.rotation)) continue;

            if (!SaveSystem.TryGetFloat(GetEnemySavedValueName(i, HEALTH_NAME), out data.health)) continue;

            _enemies[i].LoadProgress(data);
        }
    }

    protected override void Init()
    {
        GameLoader.Instance.OnSaving.AddListener(OnSaving);

        for (int i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].Init();
            _enemies[i].Health.OnDestroyed.AddListener(SaveEnemy);
        }
    }

    private void OnSaving()
    {
        Save(Saver.GetId(this), Saver.GetSceneId());
    }

    private void SaveEnemy(CreatureHealth creature)
    {
        int findIndex = _enemies.IndexOf(creature.Controller as EnemyController);
        if (findIndex < 0) return;

        SaveEnemy(findIndex);
    }

    private void SaveEnemy(int index)
    {
        SaveSystem.SetFormatValue(GetEnemySavedValueName(index, POSITION_NAME), _enemies[index].transform.position);
        SaveSystem.SetFormatValue(GetEnemySavedValueName(index, ROTATION_NAME), _enemies[index].transform.rotation);
        SaveSystem.SetConvertibleValue(GetEnemySavedValueName(index, HEALTH_NAME), _enemies[index].Health.Value);
    }

    private string GetEnemySavedValueName(int index, string value)
    {
        _persistStringBuilder.Clear();
        _persistStringBuilder.Append(GetSaveName(Saver.GetId(this), Saver.GetSceneId()));
        _persistStringBuilder.Append(index);
        _persistStringBuilder.Append(value);
        return _persistStringBuilder.ToString();
    }

#if UNITY_EDITOR
    private bool InitEnemiesList()
    {
        bool changed = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).TryGetComponent(out EnemyController obj)) continue;

            string curName = $"{PrefabUtility.GetCorrespondingObjectFromSource(obj).gameObject.name} ({i})";
            int findIndex = _enemies.IndexOf(obj);
            if (findIndex > -1)
            {
                if (_enemies[findIndex].gameObject.name == curName) continue;

                _enemies[findIndex].gameObject.name = curName;
                changed = true;
                continue;
            }

            _enemies.Add(obj);
            obj.gameObject.name = curName;
            changed = true;
        }

        return changed;
    }

    private bool ClearNullEnemies()
    {
        bool changed = false;
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            if (_enemies[i] != null && _enemies[i].transform != null) continue;

            _enemies.RemoveAt(i);
            changed = true;
        }

        return changed;
    }

    [ContextMenu("Refresh Saved Objects")]
    private void RefreshList()
    {
        Undo.RecordObject(this, "Enemy group change");
        if (!InitEnemiesList() && !ClearNullEnemies())
        {
            Undo.ClearUndo(this);
            return;
        }

        SetObjectDirty();
    }

    private void SetObjectDirty()
    {
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
