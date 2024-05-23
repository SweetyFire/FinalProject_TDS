using UnityEngine;

public abstract class SavedObject : MonoBehaviour
{
    public ObjectSaver Saver { get; protected set; }

    public void InitSaver(ObjectSaver saver)
    {
        Saver = saver;
        Init();
    }

    protected abstract void Init();
    public abstract void Save(int id, int sceneId);
    public abstract void Load(int id, int sceneId);

    protected string GetSaveName(int id, int sceneId)
    {
        return gameObject.name + id.ToString() + sceneId.ToString();
    }

    protected string GetSaveValueName(int id, int sceneId, string value)
    {
        return GetSaveName(id, sceneId) + value;
    }
}
